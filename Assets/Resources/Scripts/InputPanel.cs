using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InputPanel : MonoBehaviour
{
    public static InputPanel Instance { get; private set; }

    [SerializeField] private GameObject inputPanel;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Button acceptButton;
    [SerializeField] private TMP_InputField inputField;

    public string lastInput { get; private set; } = "";

    public bool isWaitingForUserInput { get; private set; }

    private void Awake()
    {
        Instance = this;

        inputPanel.SetActive(false);
        acceptButton.gameObject.SetActive(false);

        inputField.onValueChanged.AddListener(OnInputChanged);

        acceptButton.onClick.AddListener(OnAcceptInput);
    }

    public void Show(string title)
    {
        if (string.IsNullOrEmpty(title))
        {
            titleText.gameObject.SetActive(false);
        }
        else
        {
            titleText.text = title;
            titleText.gameObject.SetActive(true);
        }

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

        lastInput = inputField.text;
        Hide();
    }

    public void OnInputChanged(string value)
    {
        acceptButton.gameObject.SetActive(HasValidText());
    }

    private bool HasValidText()
    {
        return inputField.text != string.Empty;
    }
}
