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
    public TextMeshProUGUI choiceText;
    public GameObject leftArrow;
    public GameObject rightArrow;

    protected string TEXTBOX_OBJECTNAME = "Textbox Image";
    protected string NAMEPLATE_OBJECTNAME = "Name Plate";
    protected string NAME_OBJECTNAME = "Name Text";
    protected string DIALOGUE_OBJECTNAME = "Dialogue Text";
    protected string CHOICES_OBJECTNAME = "Choice";
    protected string CHOICETEXT_OBJECTNAME = "Choice Text";
    protected string LEFTARROW_OBJECTNAME = "Right Arrow";
    protected string RIGHTARROW_OBJECTNAME = "Left Arrow";

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

    public virtual void ShowChoices(string[] listOfChoices)
    {
        dialogue.gameObject.SetActive(false);
        choices.gameObject.SetActive(true);
        dialogue.text = "";

        currentChoiceContainer = new ChoiceContainer(this, listOfChoices, choiceText, leftArrow, rightArrow);
    }

    public virtual void HideChoices()
    {
        dialogue.gameObject.SetActive(true);
        choices.gameObject.SetActive(false);
    }

    public Coroutine Hide()
    {
        if (isContainerHiding) return hidingContainerCoroutine;

        hidingContainerCoroutine = dialogueManager.StartCoroutine(Hiding());

        return hidingContainerCoroutine;
    }

    public virtual IEnumerator Hiding()
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

    public enum ContainerType
    {
        MainTextbox,
        LeftTextbox,
        RightTextbox,
        SpeechBubble
    }
}
