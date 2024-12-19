using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BackgroundData
{
    public string interactableName;
    public string sceneToGo;
    public string backgroundToGo;
    public KeyToPress keyToPress;

    public BackgroundData Copy()
    {
        BackgroundData result = new BackgroundData();

        result.interactableName = interactableName;
        result.sceneToGo = sceneToGo;
        result.backgroundToGo = backgroundToGo;
        result.keyToPress = keyToPress;

        return result;
    }
}

public enum KeyToPress
{
    Right,
    Left,
    Up,
    Down,
    Question
}
