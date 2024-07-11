using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;

namespace Characters
{
    public class CharacterManager : MonoBehaviour
    {
        public static CharacterManager Instance { get; private set; }
        private Dictionary<string, Character> characters = new Dictionary<string, Character>();

        private CharacterConfig config => DialogueSystem.Instance.config.characterConfigurationAsset;

        private const string characterCastingDelimiter = " as ";
        private const string characterNameId = "<characterName>";
        private string characterRootPath => $"Art/Portraits/{characterNameId}";
        private string characterPrefabPath => $"{characterRootPath}/Character - [{characterNameId}]";

        [SerializeField] private RectTransform _characterPanel = null;
        public RectTransform characterPanel => _characterPanel;

        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
            else
            {
                DestroyImmediate(gameObject);
            }
        }

        public CharacterConfigData GetCharacterConfig(string characterName)
        {
            return config.GetConfig(characterName);
        }

        public Character GetCharacter(string characterName)
        {
            if (characters.ContainsKey(characterName.ToLower()))
            {
                return characters[characterName.ToLower()];
            }
            else
            {
                return CreateCharacter(characterName);
            }
        }

        public Character CreateCharacter(string characterName)
        {
            if (characters.ContainsKey(characterName.ToLower()))
            {
                Debug.LogError($"{characterName} already exists");
                return null;
            }

            CharacterInfo info = GetCharacterInfo(characterName);

            Character character = CreateCharacterFromInfo(info);

            characters.Add(characterName.ToLower(), character);

            return character;
        }

        private CharacterInfo GetCharacterInfo(string characterName)
        {
            CharacterInfo result = new CharacterInfo();

            string[] nameData = characterName.Split(characterCastingDelimiter, System.StringSplitOptions.RemoveEmptyEntries);

            result.name = nameData[0];
            result.castingName = nameData.Length > 1 ? nameData[1] : result.name;
            result.rootCharacterPath = FormatCharacterPath(characterRootPath, result.castingName);
            result.config = config.GetConfig(result.castingName);
            result.prefab = GetPrefabForCharacter(result.castingName);

            return result;
        }

        private GameObject GetPrefabForCharacter(string characterName)
        {
            string prefabPath = FormatCharacterPath(characterPrefabPath, characterName);

            return Resources.Load<GameObject>(prefabPath);
        }

        private string FormatCharacterPath(string path, string characterName) => path.Replace(characterNameId, characterName);

        private Character CreateCharacterFromInfo(CharacterInfo info)
        {
            CharacterConfigData config = info.config;

            switch (config.characterType)
            {
                case Character.CharacterType.Text:
                    return new TextCharacter(info.name, config);
                case Character.CharacterType.Sprite:
                    return new SpriteCharacter(info.name, config, info.prefab, info.rootCharacterPath);
                default:
                    return null;
            }
        }

        private class CharacterInfo
        {
            public string name = "";
            public string castingName = "";
            public string rootCharacterPath = "";
            public CharacterConfigData config = null;
            public GameObject prefab = null;
        }
    }
}

