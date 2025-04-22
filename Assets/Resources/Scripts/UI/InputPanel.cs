using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InputPanel
{
    public UIManager uiManager => UIManager.Instance;

    public GameObject root;

    private Button acceptButton;
    private TMP_InputField inputField;

    private List<Toggle> pronounToggles = new List<Toggle>();

    private const string ACCEPTBUTTON_OBJECTNAME = "Accept Button";
    private const string INPUTFIELD_OBJECTNAME = "Input Field";
    private const string PRONOUNS_OBJECTNAME = "Pronouns";

    public bool isWaitingForUserInput { get; private set; }


    private string pronouns;

    public string name { get; private set; }
    public string subjectPronoun { get; private set; }
    public string objectPronoun { get; private set; }
    public string possessivePronoun { get; private set; }

    private const int CHARACTER_LIMIT = 10;

    public InputPanel(GameObject prefab)
    {
        root = Object.Instantiate(prefab, uiManager.graphicsContainer);

        inputField = root.transform.Find(INPUTFIELD_OBJECTNAME).GetComponent<TMP_InputField>();
        acceptButton = root.transform.Find(ACCEPTBUTTON_OBJECTNAME).GetComponent<Button>();

        inputField.characterLimit = CHARACTER_LIMIT;

        Transform pronounsObject = root.transform.Find(PRONOUNS_OBJECTNAME);
        for (int i = 0; i < pronounsObject.childCount; i++)
        {
            Toggle pronounToggle = pronounsObject.GetChild(i).GetComponent<Toggle>();
            pronounToggles.Add(pronounToggle);

            if(i == 0)
            {
                pronouns = pronounToggle.name;
            }
        }

        EventTrigger eventTrigger = acceptButton.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry pointerEnterEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        pointerEnterEntry.callback.AddListener((eventData) => OnButtonHover());
        eventTrigger.triggers.Add(pointerEnterEntry);

        EventTrigger.Entry pointerExitEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        pointerExitEntry.callback.AddListener((eventData) => OnButtonExit());
        eventTrigger.triggers.Add(pointerExitEntry);

        acceptButton.gameObject.SetActive(false);

        inputField.onValueChanged.AddListener(OnInputChanged);
        acceptButton.onClick.AddListener(OnAcceptInput);

        foreach (Toggle toggle in pronounToggles)
        {
            toggle.isOn = true;

            toggle.onValueChanged.AddListener(state =>
            {
                if (state == true)
                {
                    pronouns = toggle.name;
                }
            });
        }

        Show();
    }

    public void Show()
    {
        inputField.text = string.Empty;

        isWaitingForUserInput = true;
    }

    public void Hide()
    {
        isWaitingForUserInput = false;
    }

    private void OnAcceptInput()
    {
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

        name = inputField.text;
        Hide();
    }

    private void OnInputChanged(string value)
    {
        acceptButton.gameObject.SetActive(HasValidInput());
    }

    private bool HasValidInput()
    {
        return pronouns != string.Empty && inputField.text != string.Empty && inputField.text.Length <= CHARACTER_LIMIT;
    }

    private void OnButtonHover()
    {
        acceptButton.GetComponentInChildren<TextMeshProUGUI>().text = "Begin your story";
    }

    private void OnButtonExit()
    {
        acceptButton.GetComponentInChildren<TextMeshProUGUI>().text = "This is who you are";
    }
}