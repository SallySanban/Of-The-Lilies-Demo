using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class NameContainer
{
    [SerializeField] private GameObject root;
    [SerializeField] private Image nameBorder;
    [SerializeField] private TextMeshProUGUI nameText;

    public void Show(string name = "")
    {
        root.SetActive(true);

        if(name != string.Empty)
        {
            nameText.text = name;
        }
    }

    public void Hide()
    {
        root.SetActive(false);
    }

    public void SetBorderColor(Color color) => nameBorder.color = color;
}
