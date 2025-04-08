using UnityEngine;

namespace LTX.Tools.Settings
{
    public abstract class SettingsAsset<T> : ScriptableObject where T : SettingsAsset<T>
    {
        public static T Current
        {
            get
            {
                if (current != null)
                    return current;

                if (SettingsCollection.Current == null)
                {
                    Debug.LogError("Please access settings at least after assembly loading. Settings are not loaded before this stage.");
                    return null;
                }

                if (SettingsCollection.Current.TryGetSettings(out current))
                    return current;

                Debug.LogError($"Couldn't load settings of type {typeof(T).Name}");
                return null;
            }
        }

        private static T current;
    }
}