using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Dialogue;

public class ChoiceContainer
{
    private DialogueContainer dialogueContainer;

    private List<ChoiceButton> buttons = new List<ChoiceButton>();

    public ChoicePanelDecision lastChoicePicked { get; private set; } = null;
    public bool isWaitingForUserChoice { get; private set; } = false;

    private int currentSelectedIndex = 0;

    public ChoiceContainer(DialogueContainer dialogueContainer, string[] listOfChoices, TextMeshProUGUI choiceText)
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
                choiceButton = new ChoiceButton {
                    choiceText = choiceText
                };

                buttons.Add(choiceButton);
            }

            choiceButton.choiceText.text = listOfChoices[i];
        }

        if (buttons.Count > 0)
        {
            buttons[0].choiceText.text = listOfChoices[0];
        }

        isWaitingForUserChoice = true;
        currentSelectedIndex = 0;
    }

    public void MoveSelection(int direction)
    {
        if (!isWaitingForUserChoice) return;
        
        currentSelectedIndex += direction;
        if (currentSelectedIndex < 0) currentSelectedIndex = buttons.Count - 1;
        if (currentSelectedIndex >= buttons.Count) currentSelectedIndex = 0;

        for (int i = 0; i < buttons.Count; i++)
        {
            if (i == currentSelectedIndex)
            {
                buttons[i].choiceText.text = lastChoicePicked.choices[i];
            }
        }
    }

    public void AcceptChoice()
    {
        if (currentSelectedIndex < 0 || currentSelectedIndex > buttons.Count - 1) return;

        isWaitingForUserChoice = false;

        lastChoicePicked.answerIndex = currentSelectedIndex;
        isWaitingForUserChoice = false;

        dialogueContainer.HideChoices();

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
        public TextMeshProUGUI choiceText;
    }
}
