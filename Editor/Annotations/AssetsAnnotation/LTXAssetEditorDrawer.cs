using LTX.Tools;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


namespace LTX.Editor.Annotations.FoldersAnnotation
{
    [InitializeOnLoad]
    public static class LTXAssetEditorDrawer
    {
        static LTXAssetEditorDrawer()
        {
            EditorApplication.projectWindowItemOnGUI += DrawFolderIcon;
            UnityEditor.Editor.finishedDefaultHeaderGUI += OnPostHeaderGUI;
        }


        private static bool IsEditing;
        private static Vector2 scroll;
        private static string currentText;

        private static void OnPostHeaderGUI(UnityEditor.Editor editor)
        {
            const float buttonWidth = 150f;

            LTXAssetLibrary library = LTXAssetLibrary.instance;
            SerializedObject librarySerializedObject = new SerializedObject(library);
            EditorGUI.BeginChangeCheck();
            foreach (var t in editor.targets)
            {
                string path = AssetDatabase.GetAssetPath(t);
                if (string.IsNullOrEmpty(path))
                    continue;

                GUILayout.Space(20);

                if (library.Exists(path, out var result, out int index))
                {
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Remove", GUILayout.ExpandWidth(false), GUILayout.Width(buttonWidth)))
                    {
                        RemoveFolderData(library, path);
                        return;
                    }

                    string listName = library.ListName;
                    SerializedProperty property = librarySerializedObject
                        .FindProperty(listName)
                        .GetArrayElementAtIndex(index)
                        .FindPropertyRelative(nameof(LTXAssetData.annotation));


                    SerializedProperty fontSizeProperty = property.FindPropertyRelative("fontSize");
                    SerializedProperty annotationProperty = property.FindPropertyRelative("annotation");

                    bool buttonPressed = GUILayout.Button(IsEditing ? "Save" : "Edit", GUILayout.ExpandWidth(false), GUILayout.Width(buttonWidth));
                    int fontSize = EditorGUILayout.IntSlider((int)fontSizeProperty.floatValue, 10, 30, GUILayout.ExpandWidth(true));
                    fontSizeProperty.floatValue = fontSize;

                    EditorGUILayout.EndHorizontal();

                    if (buttonPressed)
                        IsEditing = !IsEditing;

                    if (IsEditing)
                    {
                        GUIStyle style = EditorStyles.textArea;
                        style.fontSize = fontSize;
                        float height = 36f + Mathf.CeilToInt(style.CalcSize(new GUIContent(annotationProperty.stringValue)).y);
                        annotationProperty.stringValue = EditorGUILayout.TextArea(annotationProperty.stringValue, style, GUILayout.Height(height));
                    }
                    else
                    {
                        GUIStyle style = EditorStyles.helpBox;
                        style.fontSize = fontSize;
                        style.richText = true;

                        GUILayout.Box(new GUIContent(annotationProperty.stringValue), style);
                    }
                }
                else
                {
                    if (GUILayout.Button("Add Note"))
                        AddFolderData(library, path);
                }

            }

            if (EditorGUI.EndChangeCheck())
            {
                librarySerializedObject.ApplyModifiedProperties();
            }
        }

        private static void DrawFolderIcon(string guid, Rect rect)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            if(!LTXAssetLibrary.instance.IsValid(path))
                return;

            if (!LTXAssetLibrary.instance.Exists(path, out var result))
                return;

            if (!string.IsNullOrEmpty(result.annotation.Annotation))
            {
                float iconWidth = 15;
                EditorGUIUtility.SetIconSize(new Vector2(iconWidth, iconWidth));
                var padding = new Vector2(5, 0);
                var iconDrawRect = new Rect(
                    rect.xMax - (iconWidth + padding.x),
                    rect.yMin,
                    rect.width,
                    rect.height);

                EditorGUI.LabelField(iconDrawRect, EditorGUIUtility.IconContent("TextAsset Icon"));
                EditorGUIUtility.SetIconSize(Vector2.zero);
            }

        }

        internal static void AddFolderData(LTXAssetLibrary library, string path)
        {
            library.AddFolderData(new LTXAssetData() { annotation = new Annotable("Write a new note..."), path = path, });

            Reload();

        }
        internal static void RemoveFolderData(LTXAssetLibrary library, string path)
        {
            library.DeleteFolderData(path, out _);
            Reload();
        }

        private static void Reload()
        {
        }
    }
}