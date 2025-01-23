using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BackgroundData
{
    public string interactableName;
    public string backgroundToGo;
    public KeyToPress keyToPress;
    public Vector2 playerPositionInNextBackground;
    public int playerDirectionInNextBackground;
    public string followPlayer;
    public Vector2 followPlayerPosition;

    public BackgroundData Copy()
    {
        BackgroundData result = new BackgroundData();

        result.interactableName = interactableName;
        result.backgroundToGo = backgroundToGo;
        result.keyToPress = keyToPress;
        result.playerPositionInNextBackground = playerPositionInNextBackground;
        result.playerDirectionInNextBackground = playerDirectionInNextBackground;
        result.followPlayer = followPlayer;
        result.followPlayerPosition = followPlayerPosition;

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
