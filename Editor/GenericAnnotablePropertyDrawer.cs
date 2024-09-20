using LTX.Tools;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace LTX.Editor
{
    [CustomPropertyDrawer(typeof(Annotable<>)), System.Serializable]
    public class GenericAnnotablePropertyDrawer : AnnotablePropertyDrawer
    {
        protected override string VisualAssetTreePath => "Packages/com.ltx.tools/Editor/UIToolkit/GenericAnnotableUXML.uxml";

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement container = base.CreatePropertyGUI(property);
            SerializedProperty valueProperty = property.FindPropertyRelative(nameof(Annotable<object>.value));


            PropertyField propertyField = container.Q<PropertyField>("Property");
            propertyField.BindProperty(valueProperty);
            propertyField.label = property.displayName;

            return container;
        }
    }
}