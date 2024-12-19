using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Textbox : DialogueContainer
{
    private const int dialoguePadding = 45;
    private const int choicePadding = 20;

    public Textbox(GameObject prefab, Transform parent, ContainerType textboxType, string speakerName = "", string[] listOfChoices = null)
    {
        root = Object.Instantiate(prefab, parent);

        rootCanvasGroup = root.GetComponent<CanvasGroup>();

        textboxImage = root.transform.Find(TEXTBOX_OBJECTNAME).gameObject;
        name = root.transform.Find(NAMEPLATE_OBJECTNAME).Find(NAME_OBJECTNAME).GetComponent<TextMeshProUGUI>();
        dialogue = root.transform.Find(TEXTBOX_OBJECTNAME).Find(DIALOGUE_OBJECTNAME).GetComponent<TextMeshProUGUI>();
        choices = root.transform.Find(TEXTBOX_OBJECTNAME).Find(CHOICES_OBJECTNAME).transform;
        choiceTemplate = choices.Find(CHOICETEMPLATE_OBJECTNAME).gameObject;

        this.textboxType = textboxType;
        name.text = speakerName.ToUpper();

        dialogue.gameObject.SetActive(true);
        choices.gameObject.SetActive(false);

        textboxImage.GetComponent<VerticalLayoutGroup>().padding.top = dialoguePadding;
        textboxImage.GetComponent<VerticalLayoutGroup>().padding.bottom = dialoguePadding;

        if (speakerName != "") ApplySpeakerDataToDialogueContainer(speakerName);

        if (listOfChoices != null)
        {
            ShowChoices(listOfChoices);
        }
    }

    protected override void ShowChoices(string[] listOfChoices)
    {
        base.ShowChoices(listOfChoices);

        textboxImage.GetComponent<VerticalLayoutGroup>().padding.top = choicePadding;
        textboxImage.GetComponent<VerticalLayoutGroup>().padding.bottom = choicePadding;
    }

    public override void HideChoices()
    {
        base.HideChoices();

        textboxImage.GetComponent<VerticalLayoutGroup>().padding.top = dialoguePadding;
        textboxImage.GetComponent<VerticalLayoutGroup>().padding.bottom = dialoguePadding;
    }
}
