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
        private readonly SerializedProperty property;
        private readonly Type genericArgument;
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


        private Button addButton;
        private Button clearButton;
        private Label label;
        private Label typeInfos;
        private VisualElement container;

        private DropdownField dropdownField;
        private PropertyField componentField;
        private HelpBox helpBox;

        public SComponentElement(SerializedProperty property, Type genericArgument)
        {
            this.property = property;
            this.genericArgument = genericArgument;

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

            SerializedProperty propertyRelative = property.FindBackingFieldPropertyRelative(nameof(SComponentContainer<ISComponent>.Component));
            propertyRelative.isExpanded = true;

            componentField = new PropertyField(propertyRelative, string.Empty);
            componentField.RegisterCallback<ChangeEvent<string>>(evt =>
            {
                componentField.Q<Toggle>().style.display = DisplayStyle.None;
            });
            container.Add(componentField);



            label.text = property.displayName;

            addButton.clickable.clickedWithEventInfo += OnAdd;
            clearButton.clicked += OnClear;
            RefreshLayout();
        }


        public void SetValueWithoutNotify(ISComponent newValue)
        {
            mValue = newValue;
        }

        private void OnAdd(EventBase eventBase)
        {
            AddComponentDropdown addComponentDropdown = new AddComponentDropdown(string.Empty, genericArgument);
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

                componentField.RemoveFromClassList(PANEL_ENABLE_CLASS);
                componentField.AddToClassList(PANEL_DISABLE_CLASS);
            }
            else
            {
                clearButton.AddToClassList(PANEL_ENABLE_CLASS);
                clearButton.RemoveFromClassList(PANEL_DISABLE_CLASS);
                addButton.AddToClassList(PANEL_DISABLE_CLASS);
                addButton.RemoveFromClassList(PANEL_ENABLE_CLASS);

                helpBox.AddToClassList(PANEL_DISABLE_CLASS);
                helpBox.RemoveFromClassList(PANEL_ENABLE_CLASS);

                componentField.AddToClassList(PANEL_ENABLE_CLASS);
                componentField.RemoveFromClassList(PANEL_DISABLE_CLASS);

                Type type = propertyRelative.managedReferenceValue.GetType();
                typeInfos.text = type.Name;

            }
        }


    }
}