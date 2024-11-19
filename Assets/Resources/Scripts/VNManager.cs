using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;

public class VNManager : MonoBehaviour
{
    public static VNManager Instance { get; private set; }

    private const string dialogueFile = "Scene 1";

    private void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        LoadFile(FilePaths.storyPath + dialogueFile);
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

        DialogueSystem.Instance.Say(lines, filePath);
    }
}
