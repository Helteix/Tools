using System;
using System.Collections.Generic;
using UnityEngine;

namespace LTX.Tools
{
    [System.Serializable]
    public class Annotable
    {
#if UNITY_EDITOR
        [field: SerializeField]
        public string Annotation { get; private set; }
#endif
    }
    [System.Serializable]
    public class Annotable<T> : Annotable, IEquatable<T>
    {
        [SerializeField]
        public T value;

        public override string ToString() => value.ToString();

        public static implicit operator T(Annotable<T> annotable) => annotable.value;

        bool IEquatable<T>.Equals(T other) => EqualityComparer<T>.Default.Equals(value, other);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(value, obj))
                return true;

            if (obj.GetType() != typeof(T))
                return false;

            return Equals((T)obj);
        }

        // ReSharper disable once NonReadonlyMemberInGetHashCode
        public override int GetHashCode() => value.GetHashCode();
    }
}