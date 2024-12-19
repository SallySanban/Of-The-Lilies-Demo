using UnityEngine;
using TMPro;
using Characters;
using UnityEngine.UI;
using System.Collections;
using Dialogue;

public class DialogueContainer
{
    DialogueManager dialogueManager => DialogueManager.Instance;

    public GameObject root;
    public CanvasGroup rootCanvasGroup;

    public GameObject textboxImage;
    public ContainerType textboxType;
    public TextMeshProUGUI name;
    public TextMeshProUGUI dialogue;
    public Transform choices;
    public GameObject choiceTemplate;

    protected string TEXTBOX_OBJECTNAME = "Textbox Image";
    protected string NAMEPLATE_OBJECTNAME = "Name Plate";
    protected string NAME_OBJECTNAME = "Name Text";
    protected string DIALOGUE_OBJECTNAME = "Dialogue Text";
    protected string CHOICES_OBJECTNAME = "Choices";
    protected string CHOICETEMPLATE_OBJECTNAME = "Choice Template";

    protected const float FADE_SPEED = 3f;

    public ChoiceContainer currentChoiceContainer;

    protected Coroutine showingContainerCoroutine, hidingContainerCoroutine;
    public bool isContainerShowing => showingContainerCoroutine != null;
    public bool isContainerHiding => hidingContainerCoroutine != null;

    protected void ApplySpeakerDataToDialogueContainer(string speakerName)
    {
        Character character = CharacterManager.Instance.GetCharacter(speakerName);

        CharacterConfigData config = character.config != null ? character.config : CharacterManager.Instance.GetCharacterConfig(speakerName);
    }

    protected virtual void ShowChoices(string[] listOfChoices)
    {
        dialogue.gameObject.SetActive(false);
        choices.gameObject.SetActive(true);

        currentChoiceContainer = new ChoiceContainer(this, listOfChoices, choices, choiceTemplate);
    }

    public virtual void HideChoices()
    {
        dialogue.gameObject.SetActive(true);
        choices.gameObject.SetActive(false);
    }

    public enum ContainerType
    {
        Textbox,
        LeftTextbox,
        RightTextbox,
        SpeechBubble
    }

    public Coroutine Hide()
    {
        if (isContainerHiding) return hidingContainerCoroutine;

        hidingContainerCoroutine = dialogueManager.StartCoroutine(ShowingOrHiding());

        return hidingContainerCoroutine;
    }

    public virtual IEnumerator ShowingOrHiding()
    {
        CanvasGroup self = rootCanvasGroup;

        while (self.alpha != 0)
        {
            self.alpha = Mathf.MoveTowards(self.alpha, 0, FADE_SPEED * Time.deltaTime);

            if (self.alpha == 0f)
            {
                Object.Destroy(self.gameObject);
                break;
            }

            yield return null;
        }

        hidingContainerCoroutine = null;
    }
}
