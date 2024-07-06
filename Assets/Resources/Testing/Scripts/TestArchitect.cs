using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;

namespace Testing
{
    public class TestArchitect : MonoBehaviour
    {
        DialogueSystem ds;
        TextArchitect architect;

        string[] lines = new string[5]
        {
            "HELLO THIS WORKS",
            "ASKDJABHLSDHALKSDHLAKSDHKJASDHLKSAHDLKJASHDLKJASHDLKASHDLJKSHDLKJASD",
            "AKLSJDHLKASHDKLJAHSDLKJAHSDLKASHDLKSAHDLKASHDLKJHASLKDHALKSD",
            "HASJDHLASHDLKASHDLKASJDHLJSHDLSKAHDLKASHDLJSA",
            "ALKSJDHLKASHDLKASDHLKASHDLKASHDLKAJSHDLKASJHDKJAS"
        };

        private void Start()
        {
            ds = DialogueSystem.Instance;
            architect = new TextArchitect(ds.dialogueContainer.dialogueText);
            architect.buildMethod = TextArchitect.BuildMethod.Typewriter;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (architect.isBuilding)
                {
                    architect.ForceComplete();
                }
                else
                {
                    architect.Build(lines[Random.Range(0, lines.Length)]);
                }
                
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                architect.Append(lines[Random.Range(0, lines.Length)]);
            }
        }
    }
}
