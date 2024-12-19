using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SpeechBubble : DialogueContainer
{
    public Vector3 playerOffset = new Vector2(0f, 4f);

    private const int dialogueSpacing = 20;
    private const int choiceSpacing = 45;

    private const string SPEECHBUBBLE_OBJECT = "SpeechBubble";

    public SpeechBubble(GameObject speakerSprite, string speakerName = "", string[] listOfChoices = null)
    {
        root = speakerSprite.transform.Find(SPEECHBUBBLE_OBJECT).gameObject;

        rootCanvasGroup = root.GetComponent<CanvasGroup>();

        textboxImage = root.transform.Find(TEXTBOX_OBJECTNAME).gameObject;
        name = textboxImage.transform.Find(NAME_OBJECTNAME).GetComponent<TextMeshProUGUI>();
        dialogue = textboxImage.transform.Find(DIALOGUE_OBJECTNAME).GetComponent<TextMeshProUGUI>();
        choices = textboxImage.transform.Find(CHOICES_OBJECTNAME).transform;
        choiceTemplate = choices.Find(CHOICETEMPLATE_OBJECTNAME).gameObject;

        textboxType = ContainerType.SpeechBubble;
        name.text = speakerName.ToUpper();

        dialogue.gameObject.SetActive(true);
        choices.gameObject.SetActive(false);

        textboxImage.GetComponent<VerticalLayoutGroup>().spacing = dialogueSpacing;

        if (listOfChoices != null)
        {
            ShowChoices(listOfChoices);
        }

        rootCanvasGroup.alpha = 1f;
        root.SetActive(true);
    }

    protected override void ShowChoices(string[] listOfChoices)
    {
        base.ShowChoices(listOfChoices);

        textboxImage.GetComponent<VerticalLayoutGroup>().spacing = choiceSpacing;
    }

    public override void HideChoices()
    {
        base.HideChoices();

        textboxImage.GetComponent<VerticalLayoutGroup>().spacing = dialogueSpacing;
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
}
