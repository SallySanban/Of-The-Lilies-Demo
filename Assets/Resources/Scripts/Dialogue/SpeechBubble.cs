using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SpeechBubble : DialogueContainer
{
    private const string SPEECHBUBBLE_OBJECT = "SpeechBubble";

    public string speakerSpriteName = "";

    public SpeechBubble(GameObject prefab, GameObject speakerSprite, string speakerName = "", string[] listOfChoices = null)
    {
        root = Object.Instantiate(prefab, speakerSprite.transform.Find(SPEECHBUBBLE_OBJECT).transform.position, Quaternion.identity, speakerSprite.transform);

        rootCanvasGroup = root.GetComponent<CanvasGroup>();

        speakerSpriteName = speakerSprite.name;

        textboxImage = root.transform.Find(TEXTBOX_OBJECTNAME).gameObject;
        name = textboxImage.transform.Find(NAMEPLATE_OBJECTNAME).Find(NAME_OBJECTNAME).GetComponent<TextMeshProUGUI>();
        dialogue = textboxImage.transform.Find(DIALOGUE_OBJECTNAME).GetComponent<TextMeshProUGUI>();
        choices = textboxImage.transform.Find(CHOICES_OBJECTNAME)?.transform;

        textboxType = ContainerType.SpeechBubble;
        name.text = speakerName.ToUpper();

        dialogue.gameObject.SetActive(true);

        if(choices != null)
        {
            choiceTemplate = choices.Find(CHOICETEMPLATE_OBJECTNAME).gameObject;
            choices.gameObject.SetActive(false);
        }

        if (listOfChoices != null)
        {
            ShowChoices(listOfChoices);
        }

        rootCanvasGroup.alpha = 1f;
        root.SetActive(true);
    }

    public override void ShowChoices(string[] listOfChoices)
    {
        base.ShowChoices(listOfChoices);
    }

    public override void HideChoices()
    {
        base.HideChoices();
    }

    public override string SpeakerSpriteName
    {
        get { return speakerSpriteName; }
    }
}