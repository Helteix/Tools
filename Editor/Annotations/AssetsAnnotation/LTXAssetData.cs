using LTX.Tools;
using UnityEngine;
using UnityEngine.Serialization;

namespace LTX.Editor.Annotations.FoldersAnnotation
{
    [System.Serializable]
    public struct LTXAssetData
    {
        [SerializeField]
        public Annotable annotation;

        [SerializeField]
        public string path;

    }
}