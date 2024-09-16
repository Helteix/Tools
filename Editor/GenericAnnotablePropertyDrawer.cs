using LTX.Tools;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace LTX.Editor
{
    [CustomPropertyDrawer(typeof(Annotable<>)), System.Serializable]
    public class GenericAnnotablePropertyDrawer : AnnotablePropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement container = base.CreatePropertyGUI(property);

            SerializedProperty valueProperty = property.FindPropertyRelative(nameof(Annotable<object>.value));
            PropertyField propertyField = new PropertyField(valueProperty)
            {
                label = property.displayName,
            };

            container.Add(propertyField);

            return container;
        }
    }
}