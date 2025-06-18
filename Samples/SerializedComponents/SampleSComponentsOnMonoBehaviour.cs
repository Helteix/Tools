using System;
using LTX.Tools.SerializedComponent;
using LTX.Tools.SerializedComponent.Containers;
using UnityEngine;
using UnityEngine.Serialization;

namespace LTX.Tools.Samples.SerializedComponents
{
    public class SampleSComponentsOnMonoBehaviour : MonoBehaviour
    {
        [SerializeField]
        public SComponentContainer<ISampleSComponent> container;


        private void Awake()
        {
            if(container)
                container.Component.Log();
        }
    }
}