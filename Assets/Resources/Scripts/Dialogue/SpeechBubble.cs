using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SpeechBubble : DialogueContainer
{
    public Vector3 playerOffset = new Vector2(0f, 4f);

    private const string SPEECHBUBBLE_OBJECT = "SpeechBubble";

    public string speakerSpriteName = "";

    public SpeechBubble(GameObject speakerSprite, string speakerName = "", string[] listOfChoices = null)
    {
        root = speakerSprite.transform.Find(SPEECHBUBBLE_OBJECT).gameObject;

        rootCanvasGroup = root.GetComponent<CanvasGroup>();

        speakerSpriteName = speakerSprite.name;

        textboxImage = root.transform.Find(TEXTBOX_OBJECTNAME).gameObject;
        name = textboxImage.transform.Find(NAME_OBJECTNAME).GetComponent<TextMeshProUGUI>();
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

    public override IEnumerator Hiding()
    {
        CanvasGroup self = rootCanvasGroup;

        while (self.alpha != 0)
        {
            self.alpha = Mathf.MoveTowards(self.alpha, 0, FADE_SPEED * Time.deltaTime);

            if (self.alpha == 0f)
            {
                self.gameObject.SetActive(false);
                break;
            }

            yield return null;
        }

        hidingContainerCoroutine = null;
    }

    public override string SpeakerSpriteName
    {
        get { return speakerSpriteName; }
    }
}