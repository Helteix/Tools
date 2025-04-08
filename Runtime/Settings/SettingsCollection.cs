using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace LTX.Tools.Settings
{
    public partial class SettingsCollection : ScriptableObject
    {
        internal static SettingsCollection Current
        {
            get
            {
#if UNITY_EDITOR
                if (!IsPlaying && current == null)
                    current = SettingsCollectionEditorPointer.instance.collection;
#endif
                return current;
            }
        }

        private static SettingsCollection current;

        [SerializeField]
        private ScriptableObject[] settingsAssets;


        public IEnumerable<ScriptableObject> Assets => settingsAssets;

        private void OnEnable()
        {
            if (current == null)
                current = this;
        }

        internal bool TryGetSettings<T>(out T settings) where T : SettingsAsset<T>
        {
            for (int i = 0; i < settingsAssets.Length; i++)
            {
                if (settingsAssets[i] is T t)
                {
                    settings = t;
                    return true;
                }

            }

            settings = null;
            return false;
        }

    }
}