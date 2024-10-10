using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LTX.Editor.Annotations.FoldersAnnotation
{
    public class LTXAssetsPostProcessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            LTXAssetLibrary library = LTXAssetLibrary.instance;

            for (int i = 0; i < deletedAssets.Length; i++)
            {
                string path = deletedAssets[i];
                if(string.IsNullOrEmpty(path))
                    continue;

                string extension = Path.GetExtension(path);
                if (string.IsNullOrWhiteSpace(extension))
                {
                    library.DeleteFolderData(path, out _);
                }
            }
            int min = Mathf.Min(movedAssets.Length, movedFromAssetPaths.Length);

            for (int i = 0; i < min; i++)
            {
                string fromAssetPath = movedFromAssetPaths[i];
                string toAssetPath = movedAssets[i];
                string extension = Path.GetExtension(fromAssetPath);

                if (string.IsNullOrWhiteSpace(extension) && library.DeleteFolderData(fromAssetPath, out LTXAssetData[] deleted))
                {

                    library.AddFolderData(
                        deleted.Select(ctx => new LTXAssetData()
                            {
                                annotation = ctx.annotation,
                                path = ctx.path.Replace(fromAssetPath, toAssetPath),
                            }).ToArray());
                }
            }

            EditorUtility.SetDirty(library);
            AssetDatabase.SaveAssetIfDirty(library);
        }
    }
}