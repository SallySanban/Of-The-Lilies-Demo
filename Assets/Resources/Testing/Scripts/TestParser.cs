using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;

namespace Testing
{
    public class TestParser : MonoBehaviour
    {
        [SerializeField] private TextAsset file;

        void Start()
        {
            SendFileToParse();
        }

        void SendFileToParse()
        {
            List<string> lines = FileManager.ReadTextAsset(file, false);

            foreach(string line in lines)
            {
                DialogueLine dl = DialogueParser.Parse(line);
            }
        }
    }
}

