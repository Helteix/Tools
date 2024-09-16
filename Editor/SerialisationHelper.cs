using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace LTX.Editor
{
    public static class SerialisationHelper
    {
       private static void SetValue(this SerializedProperty p, object value)
        {
            switch (p.propertyType)
            {
                case SerializedPropertyType.Generic:
                    Debug.LogWarning((object)"Get/Set of Generic SerializedProperty not supported");
                    break;
                case SerializedPropertyType.Integer:
                    p.intValue = (int)value;
                    break;
                case SerializedPropertyType.Boolean:
                    p.boolValue = (bool)value;
                    break;
                case SerializedPropertyType.Float:
                    p.floatValue = (float)value;
                    break;
                case SerializedPropertyType.String:
                    p.stringValue = (string)value;
                    break;
                case SerializedPropertyType.Color:
                    p.colorValue = (Color)value;
                    break;
                case SerializedPropertyType.ObjectReference:
                    p.objectReferenceValue = value as UnityEngine.Object;
                    break;
                case SerializedPropertyType.LayerMask:
                    p.intValue = (int)value;
                    break;
                case SerializedPropertyType.Enum:
                    if(value.GetType().GetCustomAttributes(typeof(FlagsAttribute)).Any())
                        p.enumValueFlag = (int)value;
                    else
                        p.enumValueIndex = (int)value;
                    break;
                case SerializedPropertyType.Vector2:
                    p.vector2Value = (Vector2)value;
                    break;
                case SerializedPropertyType.Vector3:
                    p.vector3Value = (Vector3)value;
                    break;
                case SerializedPropertyType.Vector4:
                    p.vector4Value = (Vector4)value;
                    break;
                case SerializedPropertyType.Rect:
                    p.rectValue = (Rect)value;
                    break;
                case SerializedPropertyType.ArraySize:
                    p.intValue = (int)value;
                    break;
                case SerializedPropertyType.Character:
                    p.stringValue = (string)value;
                    break;
                case SerializedPropertyType.AnimationCurve:
                    p.animationCurveValue = value as AnimationCurve;
                    break;
                case SerializedPropertyType.Bounds:
                    p.boundsValue = (Bounds)value;
                    break;
                case SerializedPropertyType.Gradient:
                    Debug.LogWarning((object)"Get/Set of Gradient SerializedProperty not supported");
                    break;
                case SerializedPropertyType.Quaternion:
                    p.quaternionValue = (Quaternion)value;
                    break;
            }
        }


        public static string GetBackingFieldPath(this string fieldName) => $"<{fieldName}>k__BackingField";

        public static SerializedProperty FindBackingFieldProperty(this SerializedObject serializedObject, string propertyName) => serializedObject.FindProperty(GetBackingFieldPath(propertyName));
        public static SerializedProperty FindBackingFieldPropertyRelative(this SerializedProperty serializedProperty, string propertyName) => serializedProperty.FindPropertyRelative(GetBackingFieldPath(propertyName));
    }
}