using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue
{
    public class PlayerInputManager : MonoBehaviour
    {
        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                NextLine();
            }
        }

        public void NextLine()
        {
            DialogueSystem.Instance.OnUserNext();
        }
    }
}

