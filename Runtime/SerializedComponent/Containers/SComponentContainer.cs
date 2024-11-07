using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace LTX.Tools.SerializedComponent
{

    [System.Serializable]
    public struct SComponentContainer<T> where T : class, ISComponent
    {
        [field: SerializeReference]
        public T Component { get; private set; }

        public bool HasComponent => Component != null;

        public SComponentContainer(T component)
        {
            this.Component = component;
        }


        public void SetBehaviour(T behaviour)
        {
            this.Component = behaviour;
        }

        public static implicit operator bool(SComponentContainer<T> container) => container.HasComponent;
    }
}