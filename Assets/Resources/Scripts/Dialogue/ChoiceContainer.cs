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
    private int currentChoiceSetIndex = 0;

    private List<string[]> choiceSets = new List<string[]>();
    private string[] listOfChoices;

    private Transform choicesContainer;
    private GameObject choiceTemplate;

    private const string nextString = "(Next)";
    private const string backString = "(Back)";

    private const int fullChoiceSet = 3;

    public ChoiceContainer(DialogueContainer dialogueContainer, string[] listOfChoices, Transform choicesContainer, GameObject choiceTemplate)
    {
        this.dialogueContainer = dialogueContainer;
        this.choicesContainer = choicesContainer;
        this.choiceTemplate = choiceTemplate;
        this.listOfChoices = listOfChoices;

        lastChoicePicked = new ChoicePanelDecision(listOfChoices);

        for (int i = 0; i < listOfChoices.Length; )
        {
            int remainingChoices = listOfChoices.Length - i;
            int groupSize;

            if(listOfChoices.Length > 3)
            {
                groupSize = System.Math.Min(2, remainingChoices);
            }
            else
            {
                groupSize = System.Math.Min(3, remainingChoices);
            }
            

            string[] choiceSet = new string[groupSize];
            System.Array.Copy(listOfChoices, i, choiceSet, 0, groupSize);
            choiceSets.Add(choiceSet);

            i += groupSize;
        }

        DisplayCurrentChoiceSet(this.choicesContainer, this.choiceTemplate);
    }

    private void DisplayCurrentChoiceSet(Transform choices, GameObject choiceTemplate)
    {
        ClearButtons();

        string[] currentChoices = choiceSets[currentChoiceSetIndex];

        for (int i = 0; i < currentChoices.Length; i++)
        {
            CreateChoiceButton(currentChoices[i], i, GetIndexFromListOfChoices(currentChoices[i]), choices, choiceTemplate);
        }

        if (currentChoiceSetIndex < choiceSets.Count - 1)
        {
            CreateChoiceButton(nextString, currentChoices.Length, -1, choices, choiceTemplate);
        }
        
        if (currentChoiceSetIndex == choiceSets.Count - 1 && currentChoiceSetIndex != 0)
        {
            CreateChoiceButton(backString, currentChoices.Length, -1, choices, choiceTemplate);
        }

        isWaitingForUserChoice = true;
        currentSelectedIndex = 0;
    }

    private int GetIndexFromListOfChoices(string choiceText)
    {
        int index = 0;

        foreach(string choice in listOfChoices)
        {
            if (choice.Equals(choiceText))
            {
                return index;
            }

            index++;
        }

        return -1;
    }

    private void CreateChoiceButton(string choiceText, int choiceSetIndex, int choiceIndex, Transform choices, GameObject choiceTemplate)
    {
        GameObject newChoice = Object.Instantiate(choiceTemplate, choices);
        newChoice.SetActive(true);

        GameObject arrow = newChoice.transform.Find(ARROW_OBJECTNAME).gameObject;
        TextMeshProUGUI choiceTextComponent = newChoice.transform.Find(CHOICETEXT_OBJECTNAME).GetComponent<TextMeshProUGUI>();

        ChoiceButton choiceButton = new ChoiceButton
        {
            root = newChoice,
            choiceIndex = choiceIndex,
            choiceText = choiceTextComponent,
            arrow = arrow
        };

        choiceButton.choiceText.text = choiceText;
        choiceButton.arrow.SetActive(choiceSetIndex == 0);

        if (choiceSetIndex == 0)
        {
            choiceButton.choiceText.color = new Color(0.95f, 0.7f, 0.05f);
        }
        else
        {
            choiceButton.choiceText.color = Color.white;
        }

        buttons.Add(choiceButton);
    }

    private void ClearButtons()
    {
        foreach (ChoiceButton choice in buttons)
        {
            Object.DestroyImmediate(choice.root);
        }

        buttons.Clear();
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

        string selectedChoice = buttons[currentSelectedIndex].choiceText.text;

        Debug.Log(selectedChoice);

        if (selectedChoice == nextString)
        {
            currentChoiceSetIndex++;
            DisplayCurrentChoiceSet(choicesContainer, choiceTemplate);
            return;
        }
        else if (selectedChoice == backString)
        {
            currentChoiceSetIndex = 0;
            DisplayCurrentChoiceSet(choicesContainer, choiceTemplate);
            return;
        }

        isWaitingForUserChoice = false;
        buttons[currentSelectedIndex].arrow.SetActive(false);

        lastChoicePicked.answerIndex = buttons[currentSelectedIndex].choiceIndex;
        isWaitingForUserChoice = false;

        dialogueContainer.HideChoices();
        ClearButtons();
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
        public int choiceIndex;
        public GameObject arrow;
        public TextMeshProUGUI choiceText;
    }
}