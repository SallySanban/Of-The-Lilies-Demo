using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Characters;

namespace Dialogue
{
    [CreateAssetMenu(fileName = "Dialogue System Configuration", menuName = "Dialogue System/Dialogue Configuration Asset")]
    public class DialogueSystemConfig : ScriptableObject
    {
        public CharacterConfig characterConfigurationAsset;

        public Color defaultBorderColor = Color.white;
    }
}

