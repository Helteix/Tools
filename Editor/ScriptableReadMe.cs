using System.Collections;
using System.Collections.Generic;
using LTX.Tools;
using UnityEngine;

namespace LTX.Editor
{
    [CreateAssetMenu(menuName = "Inu/ReadMe", fileName = "New ReadMe")]
    public class ScriptableReadMe : ScriptableObject
    {
        [SerializeField]
        private Annotable readMeText;
    }
}