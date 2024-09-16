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
        private HelpBox helpBox;
        private TextField textField;
        private VisualElement textFieldContainer;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var container = new VisualElement();

            SerializedProperty annotationProperty = property.FindBackingFieldPropertyRelative(nameof(Annotable.Annotation));

            //Help box
            helpBox = new HelpBox(annotationProperty.stringValue, HelpBoxMessageType.None)
            {
                focusable = true,
            };
            helpBox.AddManipulator(new Clickable(OpenTextField));
            container.Add(helpBox);

            //Text field
            textField = new TextField(string.Empty)
            {
                multiline = true,
                style = { flexShrink = 1 }
            };
            textField.RegisterValueChangedCallback(UpdateHelpBox);
            textField.BindProperty(annotationProperty);

            textFieldContainer = new VisualElement()
            {
                style = { flexDirection = FlexDirection.Row }
            };
            textFieldContainer.Add(textField);
            textFieldContainer.Add(new Button(CloseTextField)
            {
                text = "Save",
                style = { width = 150, flexShrink = 0, flexGrow = 1},
            });

            container.Add(textFieldContainer);

            Show(helpBox);
            Hide(textFieldContainer);
            return container;
        }


        private void UpdateHelpBox(ChangeEvent<string> evt)
        {
            helpBox.text = $"[Click to edit] : \n " + evt.newValue;
        }

        private void OpenTextField(EventBase obj)
        {
            Hide(helpBox);
            Show(textFieldContainer);
        }

        private void CloseTextField()
        {
            Show(helpBox);
            Hide(textFieldContainer);
        }

        private void Hide(VisualElement visualElement) => visualElement.style.display = DisplayStyle.None;
        private void Show(VisualElement visualElement) => visualElement.style.display = DisplayStyle.Flex;
    }
}