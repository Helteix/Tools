using System.Collections;
using System.Collections.Generic;
using LTX.Tools.Editor.SerializedComponent.UIToolkit;
using LTX.Tools.SerializedComponent;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace LTX.Tools.Editor
{
    [CustomPropertyDrawer(typeof(SComponentContainer<>))]
    public class SComponentContainerDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return new SComponentElement(property, fieldInfo.FieldType.GetGenericArguments()[0]);

        }
    }
}