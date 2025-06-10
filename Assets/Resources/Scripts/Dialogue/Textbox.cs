using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Textbox : DialogueContainer
{
    private const string LEFTTEXTBOX_OBJECT = "LeftTextbox";
    private const string RIGHTTEXTBOX_OBJECT = "RightTextbox";

    public Textbox(GameObject prefab, Transform parent, ContainerType textboxType, GameObject speakerSprite = null, string speakerName = "", string[] listOfChoices = null)
    {
        switch (textboxType)
        {
            case ContainerType.MainTextbox:
                root = Object.Instantiate(prefab, parent);
                break;
            case ContainerType.LeftTextbox:
                root = Object.Instantiate(prefab, speakerSprite.transform.Find(LEFTTEXTBOX_OBJECT).transform.position, Quaternion.identity, parent);
                break;
            case ContainerType.RightTextbox:
                root = Object.Instantiate(prefab, speakerSprite.transform.Find(RIGHTTEXTBOX_OBJECT).transform.position, Quaternion.identity, parent);
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