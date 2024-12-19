using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BackgroundData
{
    public string interactableName;
    public string backgroundToGo;
    public KeyToPress keyToPress;

    public BackgroundData Copy()
    {
        BackgroundData result = new BackgroundData();

        result.interactableName = interactableName;
        result.backgroundToGo = backgroundToGo;
        result.keyToPress = keyToPress;

        return result;
    }

    public enum KeyToPress
    {
        None,
        Right,
        Left,
        Up,
        Down
    }
}
