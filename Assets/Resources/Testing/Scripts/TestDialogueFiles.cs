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

        //foreach(string line in lines)
        //{
        //    if(string.IsNullOrEmpty(line)) continue;

        //    Debug.Log($"Segmenting line: {line}");
        //    DialogueLine dialogueLine = DialogueParser.Parse(line);

        //    int i = 0;

        //    foreach(DialogueData.DialogueSegment segment in dialogueLine.dialogue.lineSegments)
        //    {
        //        Debug.Log($"{i++} '{segment.dialogue}' signal = {segment.startSignal.ToString()} signalDelay = {segment.signalDelay.ToString()}");
        //    }
        //}

        DialogueSystem.Instance.Say(lines);
    }
}
