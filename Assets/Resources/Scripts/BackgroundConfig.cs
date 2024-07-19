using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Background Configuration Asset", menuName = "Configuration Assets/Background Configuration Asset")]
public class BackgroundConfig : ScriptableObject
{
    public BackgroundConfigData[] backgrounds;

    public BackgroundConfigData GetConfig(string backgroundName)
    {
        backgroundName = backgroundName.ToLower();

        for (int i = 0; i < backgrounds.Length; i++)
        {
            BackgroundConfigData data = backgrounds[i];

            if (string.Equals(backgroundName, data.backgroundName.ToLower()))
            {
                return data.Copy();
            }
        }

        return null;
    }
}

