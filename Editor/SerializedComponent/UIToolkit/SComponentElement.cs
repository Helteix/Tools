using System.Collections.Generic;
using LTX.Tools.SerializedComponent;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace LTX.Tools.Editor.SerializedComponent.UIToolkit
{
    public class SComponentElement : BindableElement, INotifyValueChanged<ISComponent>
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


        private Button addButton;
        private Button clearButton;
        private Label label;
        private PropertyField propertyField;

        private DropdownField dropdownField;

        public SComponentElement(SerializedProperty property)
        {
            VisualTreeAsset visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML_PATH);

            VisualElement root = visualTreeAsset.Instantiate();
            Add(root);

            addButton = root.Q<Button>("Add");
            clearButton = root.Q<Button>("Clear");
            label = root.Q<Label>("Label");
            propertyField = root.Q<PropertyField>("Property");


            this.BindProperty(property);
            propertyField.BindProperty(property);

            label.text = property.displayName;

            AddComponentDropdown addComponentDropdown = new AddComponentDropdown(addButton);
            addButton.clickable.clickedWithEventInfo += OnAdd;
            clearButton.clicked += OnClear;
        }


        public void SetValueWithoutNotify(ISComponent newValue)
        {
            mValue = newValue;
        }

        private void OnAdd(EventBase eventBase)
        {
            AddComponentDropdown addComponentDropdown = new AddComponentDropdown(this);
            addComponentDropdown.OnTypeSelected += SetComponent;


            addComponentDropdown.PrepareForDisplay(eventBase);
        }

        private void SetComponent(SerializedComponentLibrary.TypeInfos typeInfos)
        {
            Debug.Log("Adding component");
        }

        private void OnClear()
        {
            Debug.Log("Clear component");
        }


    }
}