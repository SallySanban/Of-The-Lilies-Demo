using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Dialogue;

public class ChoiceContainer
{
    private const string ARROW_OBJECTNAME = "Arrow";
    private const string CHOICETEXT_OBJECTNAME = "Choice Text";

    private DialogueContainer dialogueContainer;

    private List<ChoiceButton> buttons = new List<ChoiceButton>();

    public ChoicePanelDecision lastChoicePicked { get; private set; } = null;
    public bool isWaitingForUserChoice { get; private set; } = false;

    private int currentSelectedIndex = 0;

    public ChoiceContainer(DialogueContainer dialogueContainer, string[] listOfChoices, Transform choices, GameObject choiceTemplate)
    {
        this.dialogueContainer = dialogueContainer;
        lastChoicePicked = new ChoicePanelDecision(listOfChoices);

        for (int i = 0; i < listOfChoices.Length; i++)
        {
            ChoiceButton choiceButton;

            if (i < buttons.Count)
            {
                choiceButton = buttons[i];
            }
            else
            {
                GameObject newChoice = Object.Instantiate(choiceTemplate, choices);
                newChoice.SetActive(true);

                GameObject arrow = newChoice.transform.Find(ARROW_OBJECTNAME).gameObject;
                TextMeshProUGUI choiceText = newChoice.transform.Find(CHOICETEXT_OBJECTNAME).GetComponent<TextMeshProUGUI>();

                choiceButton = new ChoiceButton {
                    root = newChoice,
                    choiceText = choiceText,
                    arrow = arrow
                };

                buttons.Add(choiceButton);
            }

            choiceButton.choiceText.text = listOfChoices[i];
            choiceButton.arrow.SetActive(i == 0);

            if (i == 0)
            {
                choiceButton.choiceText.color = new Color(0.95f, 0.7f, 0.05f);
            }
            else
            {
                choiceButton.choiceText.color = Color.white;
            }
        }

        GridLayoutGroup gridLayout = choices.GetComponent<GridLayoutGroup>();
        if (gridLayout != null)
        {
            int cellsPerRow = Mathf.Max(1, Mathf.FloorToInt(((RectTransform)choices).rect.width / gridLayout.cellSize.x));
            int numberOfRows = Mathf.CeilToInt((float)listOfChoices.Length / cellsPerRow);

            float totalHeight = (numberOfRows * gridLayout.cellSize.y) +
                               (numberOfRows - 1) * gridLayout.spacing.y;

            choices.GetComponent<LayoutElement>().preferredHeight = totalHeight;
        }

        isWaitingForUserChoice = true;
        currentSelectedIndex = 0;
    }

    public void MoveSelection(int direction)
    {
        if (!isWaitingForUserChoice) return;

        buttons[currentSelectedIndex].arrow.SetActive(false);
        buttons[currentSelectedIndex].choiceText.color = Color.white;
        
        currentSelectedIndex += direction;
        if (currentSelectedIndex < 0) currentSelectedIndex = buttons.Count - 1;
        if (currentSelectedIndex >= buttons.Count) currentSelectedIndex = 0;

        buttons[currentSelectedIndex].arrow.SetActive(true);
        buttons[currentSelectedIndex].choiceText.color = new Color(0.95f, 0.7f, 0.05f);
    }

    public void AcceptChoice()
    {
        if (currentSelectedIndex < 0 || currentSelectedIndex > buttons.Count - 1) return;

        isWaitingForUserChoice = false;
        buttons[currentSelectedIndex].arrow.SetActive(false);

        lastChoicePicked.answerIndex = currentSelectedIndex;
        isWaitingForUserChoice = false;

        dialogueContainer.HideChoices();

        foreach (ChoiceButton choice in buttons)
        {
            Object.DestroyImmediate(choice.root);
        }

        buttons.Clear();
    }

    public class ChoicePanelDecision
    {
        public int answerIndex = -1;
        public string buttonText;
        public string[] choices = new string[0];

        public ChoicePanelDecision(string[] choices)
        {
            this.choices = choices;
            answerIndex = -1;
        }
    }

    private struct ChoiceButton
    {
        public GameObject root;
        public GameObject arrow;
        public TextMeshProUGUI choiceText;
    }
}
