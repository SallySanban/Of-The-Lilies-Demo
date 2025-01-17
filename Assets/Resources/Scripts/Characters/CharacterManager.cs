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

        [SerializeField] private CharacterConfig _config;
        public CharacterConfig config => _config;

        private const string CHARACTER_CASTING_DELIMITER = " as ";

        [SerializeField] private RectTransform _characterPanel = null;
        public RectTransform characterPanel => _characterPanel;

        private void Awake()
        {
            if (Instance == null)
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

        public void RemoveCharacterFromList(GameObject character, bool destroyImmediate = true)
        {
            characters.Remove(character.name.ToLower());

            if (destroyImmediate) DestroyCharacter(character);
        }

        public void DestroyCharacter(GameObject character)
        {
            Object.Destroy(character);
        }

        private CharacterInfo GetCharacterInfo(string characterName)
        {
            CharacterInfo result = new CharacterInfo();

            string[] nameData = characterName.Split(CHARACTER_CASTING_DELIMITER, System.StringSplitOptions.RemoveEmptyEntries);

            result.name = nameData[0];
            result.castingName = nameData.Length > 1 ? nameData[1] : result.name;
            result.rootCharacterPath = FilePaths.FormatPath(FilePaths.portraitRootPath, characterName);
            result.config = config.GetConfig(result.castingName);
            result.prefab = FilePaths.GetPrefabFromPath(FilePaths.portraitPrefabPath, result.castingName);

            return result;
        }

        private Character CreateCharacterFromInfo(CharacterInfo info)
        {
            CharacterConfigData config = info.config;

            switch (config.characterType)
            {
                case Character.CharacterType.Text:
                    return new TextCharacter(info.name, config);
                case Character.CharacterType.Sprite:
                    return new PortraitCharacter(info.name, config, info.prefab, info.rootCharacterPath);
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

