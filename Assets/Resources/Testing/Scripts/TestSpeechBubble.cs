using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;

public class TestSpeechBubble : MonoBehaviour
{
    [ContextMenu("Test Speech Bubble")]
    public void Test()
    {
        PixelSprite Seiji = SpriteManager.Instance.CreateSprite("Seiji", new Vector2(3.05f, 0.56f), new Vector2(1f, 1f), BackgroundConfigData.PlayerDirection.right, BackgroundManager.Instance.currentBackground.root, "Scene 2");
        Seiji.root.name = "Seiji";
        Seiji.Show();

        List<(string key, string value, string parameter)> actions = new List<(string key, string value, string parameter)>
        {
            ("Speech Bubble", "Ahlai", "The door is locked"),
            ("Speech Bubble", "Ahlai", "WASD or Arrow Keys to move"),
            ("Speech Bubble", "Ahlai", "Z to interact with objects"),
            ("Speech Bubble", "Ahlai", "Space or enter to go to next dialogue"),
            ("Speech Bubble", "Ahlai", "The door is locked..."),
            ("Speech Bubble", "Seiji", "Remember, Mr. Quan's house is the only one with the red walls."),
            ("Speech Bubble", "Seiji", "He likes to stand out... It's not that hard to miss."),
            ("Speech Bubble", "Ahlai", "Excuse me, can we have some drinks to go?"),
            ("Speech Bubble", "Ahlai", "Around three will do."),
            ("Speech Bubble", "Seiji", "Three's more than the usual, pal. It'll take me some time."),
            ("Speech Bubble", "Ahlai", "No problem."),
        };

        DialogueSystem.Instance.SaySpeechBubble(actions);
    }
}
