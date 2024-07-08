using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;

namespace Characters
{
    [System.Serializable]
    public class CharacterConfigData
    {
        public string name;
        public string alias;
        public Character.CharacterType characterType;

        public Color nameBorderColor;
        public Color textboxBorderColor;

        private static Color defaultColor => DialogueSystem.Instance.config.defaultBorderColor;

        public CharacterConfigData Copy()
        {
            CharacterConfigData result = new CharacterConfigData();

            result.name = name;
            result.alias = alias;
            result.characterType = characterType;
            result.nameBorderColor = new Color(nameBorderColor.r, nameBorderColor.g, nameBorderColor.b, nameBorderColor.a);
            result.textboxBorderColor = new Color(textboxBorderColor.r, textboxBorderColor.g, textboxBorderColor.b, textboxBorderColor.a);

            return result;
        }

        public static CharacterConfigData Default
        {
            get
            {
                CharacterConfigData result = new CharacterConfigData();

                result.name = "";
                result.alias = "";
                result.characterType = Character.CharacterType.Text;
                result.nameBorderColor = defaultColor;
                result.textboxBorderColor = defaultColor;

                return result;
            }
        }
    }
}

