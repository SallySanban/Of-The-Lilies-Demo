using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;
using Commands;

public class VNManager : MonoBehaviour
{
    public static VNManager Instance { get; private set; }

    private const string DIALOGUE_FILE = "Test";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        LoadFile(DIALOGUE_FILE);
    }

    public Coroutine LoadFile(string filename)
    {
        string filePath = FilePaths.storyPath + filename;

        List<string> lines = new List<string>();
        TextAsset file = Resources.Load<TextAsset>(filePath);

        try
        {
            lines = FileManager.ReadTextAsset(file);

            return DialogueManager.Instance.Say(lines, filePath);
        }
        catch
        {
            Debug.LogError($"Dialogue file at path Resources/{filePath} does not exist");
            return null;
        }
    }

    public IEnumerator PlayCollidingInteractableStory(string storyToPlay, Vector2 moveToInteractPosition = default)
    {
        InteractableManager.Instance.SetInteractablesAfterInteraction();

        if(moveToInteractPosition != Vector2.zero)
        {
            yield return Player.Instance.MoveToInteract(moveToInteractPosition);
        }

        yield return LoadFile(storyToPlay);

        InteractableManager.Instance.SetInteractablesAfterInteraction(true);
    }
}
