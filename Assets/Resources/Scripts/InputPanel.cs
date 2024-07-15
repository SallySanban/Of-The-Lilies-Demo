using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InputPanel : MonoBehaviour
{
    [SerializeField] private GameObject inputPanelGroup;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Button acceptButton;
    [SerializeField] private TMP_InputField inputField;

    public string lastInput { get; private set; } = "";

    public bool isWaitingForUserInput { get; private set; }

    private void Awake()
    {
        inputPanelGroup.SetActive(false);
        acceptButton.gameObject.SetActive(false);

        inputField.onValueChanged.AddListener(OnInputChanged);

        acceptButton.onClick.AddListener(OnAcceptInput);
    }

    public void Show(string title)
    {
        titleText.text = title;
        inputField.text = string.Empty;

        inputPanelGroup.SetActive(true);

        isWaitingForUserInput = true;
    }

    public void Hide()
    {
        inputPanelGroup.SetActive(false);

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
