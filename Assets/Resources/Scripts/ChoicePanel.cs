using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChoicePanel : MonoBehaviour
{
    private const float buttonMinWidth = 455.9f;
    private const float buttonMaxWidth = 700f;
    private const float buttonPadding = 100f;
    private const int layoutGroupPadding = 318;

    public static ChoicePanel Instance { get; private set; }

    [SerializeField] private GameObject choicePanel;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private GameObject choiceButtonPrefab;
    [SerializeField] private VerticalLayoutGroup buttonLayoutGroup;

    private List<ChoiceButton> buttons = new List<ChoiceButton>();
    public ChoicePanelDecision lastChoicePicked { get; private set; } = null;

    public bool isWaitingForUserChoice { get; private set; } = false;

    private void Awake()
    {
        Instance = this;

        choicePanel.SetActive(false);
    }

    public void Show(string[] choices, ChoicePosition choicePosition, string title = "")
    {
        lastChoicePicked = new ChoicePanelDecision(choices, choicePosition, title);

        isWaitingForUserChoice = true;

        choicePanel.SetActive(true);

        if(choicePosition == ChoicePosition.Left)
        {
            buttonLayoutGroup.childAlignment = TextAnchor.UpperLeft;
            buttonLayoutGroup.padding.top = layoutGroupPadding;
        }
        else if(choicePosition == ChoicePosition.Right)
        {
            buttonLayoutGroup.childAlignment = TextAnchor.UpperRight;
            buttonLayoutGroup.padding.top = layoutGroupPadding;
        }
        else if(choicePosition == ChoicePosition.Center)
        {
            buttonLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
            buttonLayoutGroup.padding.top = 0;
        }

        if (string.IsNullOrEmpty(title))
        {
            titleText.gameObject.SetActive(false);
        }
        else
        {
            titleText.text = title;
            titleText.gameObject.SetActive(true);
        }
        
        GenerateChoices(choices);
    }

    private void GenerateChoices(string[] choices)
    {
        float maxWidth = 0f;

        for(int i  = 0; i < choices.Length; i++)
        {
            ChoiceButton choiceButton;
            if(i < buttons.Count)
            {
                choiceButton = buttons[i];
            }
            else
            {
                GameObject newButtonObject = Instantiate(choiceButtonPrefab, buttonLayoutGroup.transform);
                newButtonObject.SetActive(true);

                Button newButton = newButtonObject.GetComponent<Button>();
                TextMeshProUGUI newChoiceText = newButton.GetComponentInChildren<TextMeshProUGUI>();
                LayoutElement newLayout = newButton.GetComponent<LayoutElement>();

                choiceButton = new ChoiceButton { button = newButton, choiceText = newChoiceText, layout = newLayout };

                buttons.Add(choiceButton);
            }

            choiceButton.button.onClick.RemoveAllListeners();
            int buttonIndex = i;
            choiceButton.button.onClick.AddListener(() => AcceptChoice(buttonIndex));
            choiceButton.choiceText.text = choices[i];

            float buttonWidth = Mathf.Clamp(buttonPadding + choiceButton.choiceText.preferredWidth, buttonMinWidth, buttonMaxWidth);

            maxWidth = Mathf.Max(maxWidth, buttonWidth);
        }

        foreach(var button in buttons)
        {
            button.layout.preferredWidth = maxWidth;
        }

        for(int i = 0; i < buttons.Count; i++)
        {
            bool show = i < choices.Length;

            buttons[i].button.gameObject.SetActive(show);
        }
    }

    public void Hide()
    {
        choicePanel.SetActive(false);
    }

    private void AcceptChoice(int index)
    {
        if (index < 0 || index > lastChoicePicked.choices.Length - 1) return;

        lastChoicePicked.answerIndex = index;
        isWaitingForUserChoice = false;

        Hide();
    }

    public class ChoicePanelDecision
    {
        public int answerIndex = -1;
        public string buttonText;
        public string[] choices = new string[0];
        public string titleText;
        public ChoicePosition choicePosition = ChoicePosition.Center;

        public ChoicePanelDecision(string[] choices, ChoicePosition choicePosition, string title)
        {
            this.choices = choices;
            this.choicePosition = choicePosition;
            titleText = title;
            answerIndex = -1;
        }
    }

    private struct ChoiceButton
    {
        public Button button;
        public TextMeshProUGUI choiceText;
        public LayoutElement layout;
    }

    public enum ChoicePosition
    {
        Left, //-1
        Right, //1
        Center //0
    };
}
