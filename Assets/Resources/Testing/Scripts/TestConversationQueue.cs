using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;

namespace Testing
{
    public class TestConversationQueue : MonoBehaviour
    {
        void Start()
        {
            StartCoroutine(Running());
        }

        IEnumerator Running()
        {
            List<string> lines = new List<string>()
            {
                "\"This is line 1 from the original conversation.\"",
                "\"This is line 2 from the original conversation.\"",
                "\"This is line 3 from the original conversation.\""
            };

            yield return DialogueSystem.Instance.Say(lines);

            DialogueSystem.Instance.Hide();
        }

        private void Update()
        {
            List<string> lines = new List<string>();
            Conversation conversation = null;

            if (Input.GetKeyDown(KeyCode.Q))
            {
                lines = new List<string>()
                {
                    "\"This is the start of an enqueued conversation.\"",
                    "\"We can keep it going\""
                };

                conversation = new Conversation(lines);
                DialogueSystem.Instance.conversationManager.Enqueue(conversation);
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                lines = new List<string>()
                {
                    "\"YOOOOOO\"",
                    "\"NEWWWWWWWWW\""
                };

                conversation = new Conversation(lines);
                DialogueSystem.Instance.conversationManager.EnqueuePriority(conversation);
            }
        }
    }
}

