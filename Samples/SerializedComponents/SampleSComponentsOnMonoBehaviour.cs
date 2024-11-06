using LTX.Tools.SerializedComponent;
using UnityEngine;

namespace LTX.Tools.Samples.SerializedComponents
{
    public class SampleSComponentsOnMonoBehaviour : MonoBehaviour
    {
        [SerializeField]
        public SComponentsContainer<SamplesSComponent> components;


        [SerializeField]
        public SComponentContainer<SamplesSComponent> component;

    }
}