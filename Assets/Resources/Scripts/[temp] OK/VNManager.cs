using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;
using Commands;

public class VNManager : MonoBehaviour
{
    public static VNManager Instance { get; private set; }

    private const string DIALOGUE_FILE = "Test";

    public void Start()
    {
        LoadFile(FilePaths.storyPath + DIALOGUE_FILE);
    }

    public void LoadFile(string filePath)
    {
        List<string> lines = new List<string>();
        TextAsset file = Resources.Load<TextAsset>(filePath);

        try
        {
            lines = FileManager.ReadTextAsset(file);
        }
        catch
        {
            Debug.LogError($"Dialogue file at path Resources/{filePath} does not exist");
            return;
        }

        DialogueManager.Instance.Say(lines, filePath);
    }
}
