using System.Collections;
using System.Collections.Generic;
using LTX.Tools;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace LTX.Editor
{
    [CustomPropertyDrawer(typeof(Annotable), false), System.Serializable]
    public class AnnotablePropertyDrawer : PropertyDrawer
    {
        protected virtual string VisualAssetTreePath => "Packages/com.ltx.tools/Editor/UIToolkit/AnnotableUXML.uxml";

        private const string PANEL_ENABLE_CLASS = "panel-enable";
        private const string PANEL_DISABLE_CLASS = "panel-disable";


        private VisualTreeAsset visualTreeAsset;

        private VisualElement editPanel;
        private VisualElement showPanel;

        private Slider textSizeSlider;
        private HelpBox helpBox;
        private TextField textField;
        private ToolbarButton saveButton;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            if (visualTreeAsset == null)
                visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(VisualAssetTreePath);


            SerializedProperty annotationProperty = property.FindBackingFieldPropertyRelative(nameof(Annotable.Annotation));
            SerializedProperty fontSizeProperty = property.FindBackingFieldPropertyRelative(nameof(Annotable.FontSize));
            var root = visualTreeAsset.Instantiate(property.propertyPath);

            editPanel = root.Query("EditMode").First();
            showPanel = root.Query("DisplayMode").First();

            //Slider
            textSizeSlider = root.Q<Slider>("TextSizeSlider");

            textSizeSlider.BindProperty(fontSizeProperty);
            textSizeSlider.RegisterCallback<ChangeEvent<float>>((evt) =>
            {
                helpBox.Q<Label>().style.fontSize = evt.newValue;
                textField.style.fontSize = evt.newValue;
            });

            //Help box
            helpBox = root.Q<HelpBox>();
            helpBox.AddManipulator(new Clickable(Edit));

            //Text field
            textField = root.Q<TextField>("InputField");
            textField.RegisterValueChangedCallback(UpdateHelpBox);
            textField.RegisterCallback(new EventCallback<FocusOutEvent>(ctx => Show()));
            textField.BindProperty(annotationProperty);

            saveButton = root.Q<ToolbarButton>("Save");
            saveButton.clicked += Show;

            Show();
            return root;
        }

        private Label GetHelpBoxLabel() => helpBox.Q<Label>();

        private void UpdateHelpBox(ChangeEvent<string> evt)
        {
            helpBox.text = evt.newValue;

        }


        public void Edit()
        {
            editPanel.RemoveFromClassList(PANEL_DISABLE_CLASS);
            editPanel.AddToClassList(PANEL_ENABLE_CLASS);

            showPanel.RemoveFromClassList(PANEL_ENABLE_CLASS);
            showPanel.AddToClassList(PANEL_DISABLE_CLASS);

            saveButton.RemoveFromClassList(PANEL_DISABLE_CLASS);
            saveButton.AddToClassList(PANEL_ENABLE_CLASS);
        }

        public void Show()
        {
            showPanel?.RemoveFromClassList(PANEL_DISABLE_CLASS);
            showPanel?.AddToClassList(PANEL_ENABLE_CLASS);

            editPanel?.RemoveFromClassList(PANEL_ENABLE_CLASS);
            editPanel?.AddToClassList(PANEL_DISABLE_CLASS);

            saveButton.RemoveFromClassList(PANEL_ENABLE_CLASS);
            saveButton.AddToClassList(PANEL_DISABLE_CLASS);
        }

    }
}