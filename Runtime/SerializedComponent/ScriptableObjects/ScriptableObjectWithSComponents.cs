using UnityEngine;

namespace LTX.Tools.SerializedComponent.ScriptableObjects
{
    public abstract class ScriptableObjectWithSComponents<T> : ScriptableObject where T : ISComponent
    {
        [field: SerializeField]
        public SComponentsContainer<T> Components { get; private set; } = new();
    }
}