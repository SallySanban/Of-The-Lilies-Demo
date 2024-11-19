using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;
using TMPro;
using Characters;

public class DialogueContainer
{
    public GameObject root;
    public DialogueSystem.ContainerToUse textboxType;
    public TextMeshProUGUI name;
    public TextMeshProUGUI dialogue;

    private const string IMAGE_OBJECTNAME = "Textbox Image";
    private const string NAME_OBJECTNAME = "Name Text";
    private const string DIALOGUE_OBJECTNAME = "Dialogue Text";

    public DialogueContainer(GameObject prefab, Transform parent, DialogueSystem.ContainerToUse textboxType, string speakerName = "")
    {
        GameObject textbox = Object.Instantiate(prefab, parent);

        root = textbox;
        name = textbox.transform.Find(IMAGE_OBJECTNAME).Find(NAME_OBJECTNAME).GetComponent<TextMeshProUGUI>();
        dialogue = textbox.transform.Find(IMAGE_OBJECTNAME).Find(DIALOGUE_OBJECTNAME).GetComponent<TextMeshProUGUI>();

        this.textboxType = textboxType;
        name.text = speakerName;

        if(speakerName != "") ApplySpeakerDataToDialogueContainer(speakerName);
    }

    private void ApplySpeakerDataToDialogueContainer(string speakerName)
    {
        Character character = CharacterManager.Instance.GetCharacter(speakerName);

        CharacterConfigData config = character.config != null ? character.config : CharacterManager.Instance.GetCharacterConfig(speakerName);

        name.color = config.nameColor;
    }
}
