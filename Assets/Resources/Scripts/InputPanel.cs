using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InputPanel : MonoBehaviour
{
    public static InputPanel Instance { get; private set; }

    [SerializeField] private GameObject inputPanel;
    [SerializeField] private Button acceptButton;
    [SerializeField] private TMP_InputField inputField;

    [SerializeField] private List<Toggle> pronounToggles = new List<Toggle>();

    public string lastInput { get; private set; } = "";

    public bool isWaitingForUserInput { get; private set; }

    private string pronouns = "";
    public string subjectPronoun = "";
    public string objectPronoun = "";
    public string possessivePronoun = "";

    private void Awake()
    {
        Instance = this;

        inputPanel.SetActive(false);
        acceptButton.gameObject.SetActive(false);

        inputField.onValueChanged.AddListener(OnInputChanged);
        acceptButton.onClick.AddListener(OnAcceptInput);

        foreach(Toggle toggle in pronounToggles)
        {
            toggle.isOn = false;

            toggle.onValueChanged.AddListener(state =>
            {
                if(state == true)
                {
                    pronouns = toggle.name;
                }
            });
        }
    }

    public void Show()
    {
        inputField.text = string.Empty;

        inputPanel.SetActive(true);

        isWaitingForUserInput = true;
    }

    public void Hide()
    {
        inputPanel.SetActive(false);

        isWaitingForUserInput = false;
    }

    public void OnAcceptInput()
    {
        if(inputField.text == string.Empty)
        {
            return;
        }

        if (pronouns == "She/Her")
        {
            subjectPronoun = "she";
            objectPronoun = "her";
            possessivePronoun = "her";
        }
        else if (pronouns == "He/Him")
        {
            subjectPronoun = "he";
            objectPronoun = "him";
            possessivePronoun = "his";
        }
        else if (pronouns == "They/Them")
        {
            subjectPronoun = "they";
            objectPronoun = "them";
            possessivePronoun = "their";
        }

        lastInput = inputField.text;
        Hide();
    }

    public void OnInputChanged(string value)
    {
        acceptButton.gameObject.SetActive(HasValidInput());
    }

    private bool HasValidInput()
    {
        return pronouns != string.Empty && inputField.text != string.Empty;
    }
}
