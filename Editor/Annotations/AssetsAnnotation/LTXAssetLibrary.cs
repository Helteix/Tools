using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LTX.Tools;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace LTX.Editor.Annotations.FoldersAnnotation
{
    [FilePath("ProjectSettings/LTX/LTXAssets.asset", FilePathAttribute.Location.ProjectFolder)]
    public class LTXAssetLibrary : ScriptableSingleton<LTXAssetLibrary>
    {
        [SerializeField]
        private List<LTXAssetData> foldersData = new();

        private DynamicBuffer<LTXAssetData> buffer = new(128);

        public string ListName => nameof(foldersData);

        private void OnValidate()
        {
            //Ensure folders validity
            buffer.CopyFrom(foldersData);

            for (int i = 0; i < buffer.Length; i++)
            {
                LTXAssetData data = buffer[i];
                if (!IsValid(data.path))
                    foldersData.Remove(data);
            }
        }

        public void Save() => Save(true);
        internal bool Exists(string folderPath, out LTXAssetData result, out int index)
        {
            index = GetIndex(folderPath);
            if (index == -1)
            {
                result = default;
                return false;
            }

            result = foldersData[index];
            return true;
        }

        internal bool Exists(string folderPath, out LTXAssetData result) => Exists(folderPath, out result, out _);

        internal bool TryGetSerializedPropertyPath(string folderPath, out string property)
        {
            int idx = GetIndex(folderPath);
            if (idx == -1)
            {
                property = default;
                return false;
            }

            property = $"foldersData.data[{idx}]";
            return true;
        }
        internal bool DeleteFolderData(string folderPath, out LTXAssetData[] result)
        {
            var subFolders = AssetDatabase.GetSubFolders(folderPath);

            List<LTXAssetData> removed = new();

            int idx;
            for (int i = 0; i < subFolders.Length; i++)
            {
                idx = GetIndex(subFolders[i]);
                if (idx != -1)
                {
                    removed.Add(foldersData[idx]);
                    foldersData.RemoveAt(idx);
                }
            }
            idx = GetIndex(folderPath);
            if (idx != -1)
            {
                removed.Add(foldersData[idx]);
                foldersData.RemoveAt(idx);
            }
            result = removed.ToArray();
            Save();
            return result.Length > 0;
        }

        internal void AddFolderData(params LTXAssetData[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                if(foldersData.Any(ctx => ctx.path == data[i].path))
                    continue;

                foldersData.Add(data[i]);
            }

            Save();
        }
        public int GetIndex(string folderPath)
        {
            return foldersData.FindIndex(ctx => ctx.path == folderPath);
        }

        public bool IsValid(string path)
        {
            if (!Path.HasExtension(path))
                return AssetDatabase.IsValidFolder(path);
            else
                return AssetDatabase.AssetPathToGUID(path, AssetPathToGUIDOptions.OnlyExistingAssets) != null;
        }
    }
}