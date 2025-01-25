using System;
using System.Collections.Generic;
using System.Reflection;
using LTX.Editor;
using LTX.Tools.SerializedComponent;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace LTX.Tools.Editor.SerializedComponent.UIToolkit
{
    public class SComponentElement : VisualElement, INotifyValueChanged<ISComponent>
    {

        private const string PANEL_ENABLE_CLASS = "panel-enable";
        private const string PANEL_DISABLE_CLASS = "panel-disable";

        public const string UXML_PATH = "Packages/com.ltx.tools/Editor/SerializedComponent/UIToolkit/SerializedComponentContainerField.uxml";


        private ISComponent mValue;
        public ISComponent value
        {
            get => mValue;
            set
            {
                if (EqualityComparer<ISComponent>.Default.Equals(mValue, value))
                    return;

                using ChangeEvent<ISComponent> evt = ChangeEvent<ISComponent>.GetPooled(mValue, value);

                evt.target = this;
                SetValueWithoutNotify(value);
                SendEvent(evt);
            }
        }

        private readonly SerializedProperty property;
        private readonly Type typeConstraint;
        private readonly string pathConstraint;
        private readonly bool showNonCompatible;

        private Button addButton;
        private Button clearButton;
        private Label label;
        private Label typeInfos;
        private VisualElement container;

        private DropdownField dropdownField;
        private HelpBox helpBox;
        private SerializedProperty componentProperty;

        public SComponentElement(SerializedProperty property, Type typeConstraint, string pathConstraint,
            bool showNonCompatible)
        {
            this.property = property;
            this.typeConstraint = typeConstraint;
            this.pathConstraint = pathConstraint;
            this.showNonCompatible = showNonCompatible;

            VisualTreeAsset visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML_PATH);

            VisualElement root = visualTreeAsset.Instantiate();
            Add(root);

            addButton = root.Q<Button>("Add");
            clearButton = root.Q<Button>("Clear");
            label = root.Q<Label>("Label");
            typeInfos = root.Q<Label>("Type");
            container = root.Q<VisualElement>("Property");


            helpBox = new HelpBox("No components assigned yet.", HelpBoxMessageType.Error);
            container.Add(helpBox);

            componentProperty = property.FindBackingFieldPropertyRelative(nameof(SComponentContainer<ISComponent>.Component));
            componentProperty.isExpanded = true;

            FillContainerForProperty();

            label.text = property.displayName;

            addButton.clickable.clickedWithEventInfo += OnAdd;
            clearButton.clicked += OnClear;
            RefreshLayout();
        }

        private void FillContainerForProperty()
        {
            container.Clear();
            SerializedProperty copy = componentProperty.Copy();
            copy.Next(true);
            do
            {
                if (!copy.propertyPath.Contains(componentProperty.propertyPath))
                    break;

                PropertyField propertyField = new PropertyField(copy)
                {
                    name = copy.name
                };
                container.Add(propertyField);

            } while (copy.NextVisible(false));

            container.Bind(copy.serializedObject);
        }
        public void SetValueWithoutNotify(ISComponent newValue)
        {
            mValue = newValue;
        }

        private void OnAdd(EventBase eventBase)
        {
            AddComponentDropdown addComponentDropdown = new AddComponentDropdown(pathConstraint, typeConstraint, showNonCompatible);
            addComponentDropdown.OnTypeSelected += SetComponent;

            addComponentDropdown.Show(eventBase);
        }

        private void SetComponent(Type type)
        {
            SerializedProperty propertyRelative = property.FindBackingFieldPropertyRelative(nameof(SComponentContainer<ISComponent>.Component));
            propertyRelative.managedReferenceValue = Activator.CreateInstance(type);

            property.serializedObject.ApplyModifiedProperties();
            // Debug.Log("Adding component");

            RefreshLayout();
        }

        private void OnClear()
        {
            SerializedProperty propertyRelative = property.FindBackingFieldPropertyRelative(nameof(SComponentContainer<ISComponent>.Component));
            propertyRelative.managedReferenceValue = null;

            property.serializedObject.ApplyModifiedProperties();
            // Debug.Log("Clear component");

            RefreshLayout();
        }
        public void RefreshLayout()
        {
            if (SerializationUtility.HasManagedReferencesWithMissingTypes(property.serializedObject.targetObject))
                SerializationUtility.ClearAllManagedReferencesWithMissingTypes(property.serializedObject.targetObject);

            SerializedProperty propertyRelative = property.FindBackingFieldPropertyRelative(nameof(SComponentContainer<ISComponent>.Component));

            if (propertyRelative.managedReferenceValue == null)
            {
                addButton.AddToClassList(PANEL_ENABLE_CLASS);
                addButton.RemoveFromClassList(PANEL_DISABLE_CLASS);
                clearButton.AddToClassList(PANEL_DISABLE_CLASS);
                clearButton.RemoveFromClassList(PANEL_ENABLE_CLASS);

                typeInfos.text = string.Empty;
                helpBox.RemoveFromClassList(PANEL_DISABLE_CLASS);
                helpBox.AddToClassList(PANEL_ENABLE_CLASS);

                container.Clear();
            }
            else
            {
                clearButton.AddToClassList(PANEL_ENABLE_CLASS);
                clearButton.RemoveFromClassList(PANEL_DISABLE_CLASS);
                addButton.AddToClassList(PANEL_DISABLE_CLASS);
                addButton.RemoveFromClassList(PANEL_ENABLE_CLASS);

                helpBox.AddToClassList(PANEL_DISABLE_CLASS);
                helpBox.RemoveFromClassList(PANEL_ENABLE_CLASS);

                FillContainerForProperty();

                Type type = propertyRelative.managedReferenceValue.GetType();
                typeInfos.text = type.Name;
            }
        }


    }
}