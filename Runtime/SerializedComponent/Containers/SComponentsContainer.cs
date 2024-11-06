using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace LTX.Tools.SerializedComponent
{

    /// <summary>
    /// A container for multiple <see cref="T"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [System.Serializable]
    public struct SComponentsContainer<T> : IEnumerable<T> where T : ISComponent
    {
        [SerializeReference]
        private List<T> components;

        public int ComponentsCount => components.Count;

        public SComponentsContainer(params T[] behaviours)
        {
            this.components = new List<T>(behaviours);
        }

        public T this[int index]
        {
            get
            {
                if (TryGetComponent(index, out T component))
                    return component;

                return default;
            }
        }

        public void AddBehaviour(T behaviour)
        {
            if(!components.Contains(behaviour))
                components.Add(behaviour);
        }

        public bool RemoveBehaviour(T behaviour) => components.Remove(behaviour);


        /// <summary>
        /// Get an <see cref="ISComponent"/> at a specific index with a specific type
        /// </summary>
        /// <param name="index">Index of the <see cref="ISComponent"/></param>
        /// <param name="component">Output if found. null if not.</param>
        /// <typeparam name="TU">Type of the <see cref="ISComponent"/> wanted </typeparam>
        /// <returns>True if a valid <see cref="ISComponent"/> was found </returns>
        public bool TryGetComponent<TU>(int index, out TU component)
        {
            if (TryGetComponent(index, out T c) && c is TU tu)
            {
                component = tu;
                return true;
            }

            component = default;
            return false;
        }
        /// <summary>
        /// Get an <see cref="ISComponent"/> at a specific index
        /// </summary>
        /// <param name="index">Index of the <see cref="ISComponent"/></param>
        /// <param name="component">Output if found. null if not.</param>
        /// <returns>True if a valid <see cref="ISComponent"/> was found </returns>
        public bool TryGetComponent(int index, out T component)
        {
            if (index >= 0 && index < components.Count)
            {
                component = components[index];
                return true;
            }

            component = default;
            return false;
        }

        /// <summary>
        /// Get an <see cref="ISComponent"/> with a specific type
        /// </summary>
        /// <param name="component">Output if found. null if not.</param>
        /// <typeparam name="TU">Type of the <see cref="ISComponent"/> wanted </typeparam>
        /// <returns>True if a valid <see cref="ISComponent"/> was found </returns>
        public bool TryGetComponent<TU>(out TU component) where TU : T
        {
            foreach (var behaviour in components)
            {
                if (behaviour is TU compatibleBehaviour)
                {
                    component = compatibleBehaviour;
                    return true;
                }
            }

            component = default;
            return false;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => components.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => components.GetEnumerator();
    }
}