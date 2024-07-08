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

        //foreach (string line in lines)
        //{
        //    if (string.IsNullOrEmpty(line)) continue;

        //    Debug.Log($"Segmenting line: {line}");
        //    DialogueLine dialogueLine = DialogueParser.Parse(line);

        //    int i = 0;

        //    if (dialogueLine.dialogueData != null)
        //    {
        //        foreach (DialogueData.DialogueSegment segment in dialogueLine.dialogueData.lineSegments)
        //        {
        //            Debug.Log($"Segment {i++} '{segment.dialogue}' signal = {segment.startSignal.ToString()} signalDelay = {segment.signalDelay.ToString()}");
        //        }
        //    }

        //}

        //foreach (string line in lines)
        //{
        //    if (string.IsNullOrEmpty(line)) continue;

        //    DialogueLine dialogueLine = DialogueParser.Parse(line);

        //    List<(int l, string ex)> expr = dialogueLine.speaker.castExpressions;

        //    for(int c = 0; c < expr.Count; c++)
        //    {
        //        Debug.Log($"Layer {expr[c].l} = {expr[c].ex}");
        //    }
        //}

        //foreach (string line in lines)
        //{
        //    if (string.IsNullOrEmpty(line)) continue;

        //    Debug.Log("THIS IS THE LINE " + line);

        //    DialogueLine dialogueLine = DialogueParser.Parse(line);

        //    if(dialogueLine.commandData != null)
        //    {
        //        for (int i = 0; i < dialogueLine.commandData.commands.Count; i++)
        //        {
        //            CommandData.Command command = dialogueLine.commandData.commands[i];
        //            Debug.Log($"Command {i}: {command.name} has arguments {string.Join(", ", command.arguments)}");
        //        }
        //    }

        //}

        DialogueSystem.Instance.Say(lines);
    }
}
