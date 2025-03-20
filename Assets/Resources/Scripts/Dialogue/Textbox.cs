using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Textbox : DialogueContainer
{
    private Vector2 mainPosition = new Vector2(16.82f, -464.20f);

    public Textbox(GameObject prefab, Transform parent, ContainerType textboxType, string speakerName = "", string[] listOfChoices = null)
    {
        root = Object.Instantiate(prefab, parent);

        switch (textboxType)
        {
            case ContainerType.MainTextbox:
                root.transform.localPosition = mainPosition;
                break;
            case ContainerType.LeftTextbox:
                break;
            case ContainerType.RightTextbox:
                break;
        }

        rootCanvasGroup = root.GetComponent<CanvasGroup>();

        textboxImage = root.transform.Find(TEXTBOX_OBJECTNAME).gameObject;
        name = textboxImage.transform.Find(NAMEPLATE_OBJECTNAME).Find(NAME_OBJECTNAME).GetComponent<TextMeshProUGUI>();
        dialogue = textboxImage.transform.Find(DIALOGUE_OBJECTNAME).GetComponent<TextMeshProUGUI>();
        choices = textboxImage.transform.Find(CHOICES_OBJECTNAME).transform;
        choiceTemplate = choices.Find(CHOICETEMPLATE_OBJECTNAME).gameObject;

        this.textboxType = textboxType;
        name.text = speakerName.ToUpper();

        dialogue.gameObject.SetActive(true);
        choices.gameObject.SetActive(false);

        if (speakerName != "") ApplySpeakerDataToDialogueContainer(speakerName);

        if (listOfChoices != null)
        {
            ShowChoices(listOfChoices);
        }

        //FOR DEBUGGING PURPOSES
        //root.AddComponent<PositionDebugger>();
    }

    public override void ShowChoices(string[] listOfChoices)
    {
        base.ShowChoices(listOfChoices);
    }

    public override void HideChoices()
    {
        base.HideChoices();
    }
}