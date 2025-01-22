using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            Type fieldType = fieldInfo.FieldType;

            Type typeConstraint = fieldType.GetGenericArguments()[0];

            string pathConstraint = string.Empty;
            bool showNonCompatible = false;

            foreach (var o in fieldInfo.GetCustomAttributes())
            {
                if (o is FilterPathSComponentsAttribute filterPathSComponentsAttribute)
                    pathConstraint = filterPathSComponentsAttribute.pathConstraint;
                if (o is ShowNonCompatibleSComponentsAttribute )
                    showNonCompatible = true;
            }

            return new SComponentElement(property, typeConstraint, pathConstraint, showNonCompatible);
        }
    }
}