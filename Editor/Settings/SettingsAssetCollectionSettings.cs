using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace LTX.Tools.Settings
{
    [FilePath("ProjectSettings/LTX/SettingsAssetCollectionSettings.Asset", FilePathAttribute.Location.ProjectFolder)]
    public class SettingsAssetCollectionSettings : ScriptableSingleton<SettingsAssetCollectionSettings>
    {
        [field: SerializeField]
        public string CollectionAssetPath { get; private set; } =
            "Assets/Settings/LTXSettingsCollection.asset";

        public SettingsCollection GetCollection() => AssetDatabase.LoadAssetAtPath<SettingsCollection>(CollectionAssetPath);

        [InitializeOnLoadMethod]
        private static void Load()
        {
            TypeCache.TypeCollection typesDerivedFrom = TypeCache.GetTypesDerivedFrom(typeof(SettingsAsset<>));
            instance.Setup(typesDerivedFrom);

            AssetDatabase.SaveAssets();
        }

        private void Setup(TypeCache.TypeCollection types)
        {
            EnsureValidCollection();

            SettingsCollection collection = GetCollection();

            CreateOrDeleteEmbedSettings(types, collection);
            UpdateSerializedObject(collection);

            SetAsPreloadedAsset(collection);
            Save(true);
        }

        private void UpdateSerializedObject(SettingsCollection collection)
        {
            using (SerializedObject serializedObject = new SerializedObject(collection))
            {
                Object[] subAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(CollectionAssetPath);
                SerializedProperty property = serializedObject.FindProperty("settingsAssets");
                property.arraySize = subAssets.Length;
                for (int i = 0; i < subAssets.Length; i++)
                    property.GetArrayElementAtIndex(i).objectReferenceValue = subAssets[i];

                serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        private void CreateOrDeleteEmbedSettings(TypeCache.TypeCollection types, SettingsCollection collection)
        {
            Object[] subAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(CollectionAssetPath);

            Dictionary<Type, Object> existingSettings = subAssets.ToDictionary(
                ctx => ctx.GetType(),
                ctx => ctx);

            foreach (Type type in types)
            {
                if (!existingSettings.Remove(type))
                {
                    ScriptableObject scriptableObject = CreateInstance(type);
                    scriptableObject.name = type.Name;
                    AssetDatabase.AddObjectToAsset(scriptableObject, collection);
                }
            }

            foreach (var obj in existingSettings.Values)
                AssetDatabase.RemoveObjectFromAsset(obj);
        }

        private static void SetAsPreloadedAsset(SettingsCollection collection)
        {
            Object[] preloadedAssets = PlayerSettings.GetPreloadedAssets();
            if (preloadedAssets.All(ctx => ctx != collection))
            {
                PlayerSettings.SetPreloadedAssets(
                    new List<Object>(preloadedAssets.Where(ctx => ctx!=null))
                    {
                        collection
                    }.ToArray()
                );
            }
        }

        private void ValidateFolder()
        {
            string[] folders = CollectionAssetPath.Split('/')[..^1];
            string current = folders[0];
            for (int i = 1; i < folders.Length; i++)
            {
                var folder = Path.Combine(current, folders[i]);
                if (!AssetDatabase.IsValidFolder(folder))
                    AssetDatabase.CreateFolder(current, folders[i]);

                current = folder;
            }

            AssetDatabase.Refresh();
        }

        internal void Save() => Save(true);
        internal void EnsureValidCollection()
        {
            ValidateFolder();
            string[] assets = AssetDatabase.FindAssets($"t:{nameof(SettingsCollection)}");

            SettingsCollection collection;
            if (assets.Length == 0)
            {
                collection = CreateInstance<SettingsCollection>();
                collection.hideFlags = HideFlags.NotEditable;
                AssetDatabase.CreateAsset(collection, CollectionAssetPath);
            }
            else
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(assets[0]);
                if (assetPath != CollectionAssetPath)
                {
                    string msg = AssetDatabase.ValidateMoveAsset(assetPath, CollectionAssetPath);
                    if (!string.IsNullOrEmpty(msg))
                        Debug.LogError(msg);
                    else
                    {
                        msg = AssetDatabase.MoveAsset(assetPath, CollectionAssetPath);
                        if (!string.IsNullOrEmpty(msg))
                            Debug.LogError(msg);
                    }
                }

                if (assets.Length > 1)
                {
                    for (int i = 1; i < assetPath.Length; i++)
                    {
                        var guidToAssetPath = AssetDatabase.GUIDToAssetPath(assets[i]);
                        AssetDatabase.DeleteAsset(guidToAssetPath);
                    }
                }
            }
        }
    }
}