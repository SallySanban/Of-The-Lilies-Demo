using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;

public class TestDialogueFiles : MonoBehaviour
{
    [SerializeField] TextAsset textAsset;

    private void Start()
    {
        StartConversation();
    }

    private void StartConversation()
    {
        List<string> lines = FileManager.ReadTextAsset(textAsset);

        DialogueSystem.Instance.Say(lines);
    }
}
