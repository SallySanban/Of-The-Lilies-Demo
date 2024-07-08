using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Dialogue
{
    [System.Serializable]
    public class DialogueContainer
    {
        public GameObject root;
        public NameContainer nameContainer;
        public Image textboxBorder;
        public TextMeshProUGUI dialogueText;

        public void SetBorderColor(Color color) => textboxBorder.color = color;
    }
}

