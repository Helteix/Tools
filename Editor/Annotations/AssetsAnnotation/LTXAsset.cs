using System.IO;
using LTX.Tools;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace LTX.Editor.Annotations.FoldersAnnotation
{
    [InitializeOnLoad, CustomEditor(typeof(Object), true)]
    public class LTXAsset : UnityEditor.Editor
    {
        private VisualElement container;

        static LTXAsset()
        {
            EditorApplication.projectWindowItemOnGUI += DrawFolderIcon;
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

        protected override void OnHeaderGUI()
        {
            base.OnHeaderGUI();

        }

        public override VisualElement CreateInspectorGUI()
        {
            var path = AssetDatabase.GetAssetPath(serializedObject.targetObject);
            LTXAssetLibrary library = LTXAssetLibrary.instance;
            if(container== null)
                container = new VisualElement();

            if (library.Exists(path, out var result, out int index))
            {
                VisualElement innerContainer = new VisualElement();

                innerContainer.Add(new Button(() => RemoveFolderData(library, path)){
                    text = "Remove Note"
                });
                string listName = library.ListName;
                SerializedObject librarySerializedObject = new SerializedObject(library);
                SerializedProperty property = librarySerializedObject
                    .FindProperty(listName)
                    .GetArrayElementAtIndex(index)
                    .FindPropertyRelative(nameof(LTXAssetData.annotation));

                innerContainer.Add(new AnnotableElement(property));

                container.Add(innerContainer);
            }
            else
            {
                container.Add(new Button(() => AddFolderData(library, path))
                {
                    text = "Add Note"
                });
            }
            return container;
        }

        public override bool RequiresConstantRepaint() => true;

        void AddFolderData(LTXAssetLibrary library, string path)
        {
            library.AddFolderData(new LTXAssetData() { annotation = new Annotable("Write a new note..."), path = path, });
            EditorUtility.SetDirty(this);
            EditorUtility.SetDirty(target);
            Reload();

        }
        void RemoveFolderData(LTXAssetLibrary library, string path)
        {
            library.DeleteFolderData(path, out _);
            EditorUtility.SetDirty(this);
            EditorUtility.SetDirty(target);
            Reload();
        }

        private void Reload()
        {
            container.Clear();
            CreateInspectorGUI();
        }
    }
}