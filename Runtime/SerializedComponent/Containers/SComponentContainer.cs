using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace LTX.Tools.SerializedComponent
{

    [System.Serializable]
    public struct SComponentContainer<T> where T : ISComponent
    {
        [SerializeReference]
        private T component;

        public SComponentContainer(T component)
        {
            this.component = component;
        }

        public T Component => component;

        public void SetBehaviour(T behaviour)
        {
            this.component = behaviour;
        }
    }
}