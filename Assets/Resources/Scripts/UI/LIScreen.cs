using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Dialogue;
using System.Collections;
using FMODUnity;

public class LIScreen
{
    public GameObject root;
    public UIManager uiManager => UIManager.Instance;

    private List<Button> liButtons = new List<Button>();

    private bool isRunningConversation => DialogueManager.Instance.conversationManager.isRunning;

    private static readonly string liName = "<liName>";
    private string liButtonName = $"Main 8 - {liName} Paper Interaction";

    public static bool liScreenVisible = false;

    public LIScreen(GameObject prefab)
    {
        if (prefab != null)
        {
            liScreenVisible = true;

            root = Object.Instantiate(prefab, uiManager.graphicsContainer);
            root.SetActive(true);

            root.transform.SetSiblingIndex(0);

            liButtons.AddRange(root.GetComponentsInChildren<Button>());

            foreach(Button button in liButtons)
            {
                button.onClick.AddListener(() => UIManager.Instance.StartCoroutine(OnButtonClick(button)));
            }
        }
    }

    private IEnumerator OnButtonClick(Button button)
    {
        if (!isRunningConversation)
        {
            RuntimeManager.PlayOneShot("event:/SFX/SFX_UI LISelect"); // Plays paper shuffle sfx

            SetButtonsInteractable(false);

            string filename = FormatStoryFilename(liButtonName, button.name);
            Debug.Log(filename);
            yield return VNManager.Instance.LoadFile(filename);

            SetButtonsInteractable(true);
        }
    }

    private void SetButtonsInteractable(bool interactable)
    {
        foreach (Button btn in liButtons)
        {
            btn.interactable = interactable;
        }
    }

    public string FormatStoryFilename(string filename, string textToReplace) => filename.Replace(liName, textToReplace);
}
