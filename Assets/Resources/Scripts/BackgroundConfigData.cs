using Dialogue;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BackgroundConfigData
{
    public string backgroundName;
    public InteractableToBackgroundMap[] map;

    public BackgroundConfigData Copy()
    {
        BackgroundConfigData result = new BackgroundConfigData();

        result.backgroundName = backgroundName;
        result.map = map;

        return result;
    }


    [System.Serializable]
    public class InteractableToBackgroundMap
    {
        public string interactableName;
        public GameObject backgroundPrefab;
        public KeyToPress keyToPress;
        public Vector2 playerPositionInNextBackground;
        public Vector2 playerScaleInNextBackground;
        public PlayerDirection playerDirectionInNextBackground;
    }

    public enum KeyToPress
    {
        Right,
        Left,
        Up,
        Down,
        Question
    }

    public enum PlayerDirection
    {
        left,
        right
    }
}
