using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;
using Dialogue;

namespace Dialogue.LogicalLines
{
    public class LogicalLineManager
    {
        private DialogueManager dialogueManager => DialogueManager.Instance;
        private List<ILogicalLine> logicalLines = new List<ILogicalLine>();

        public LogicalLineManager() => LoadLogicalLines();

        private void LoadLogicalLines()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            Type[] lineTypes = assembly.GetTypes().Where(t => typeof(ILogicalLine).IsAssignableFrom(t) && !t.IsInterface).ToArray();

            foreach(Type lineType in lineTypes)
            {
                ILogicalLine line = (ILogicalLine)Activator.CreateInstance(lineType);
                logicalLines.Add(line);
            }
        }

        public bool TryGetLogicalLine(DialogueLine line, out Coroutine logic)
        {
            foreach(var logicalLine in logicalLines)
            {
                if (logicalLine.Matches(line))
                {
                    logic = dialogueManager.StartCoroutine(logicalLine.Execute(line));
                    return true;
                }
            }

            logic = null;
            return false;
        }
    }
}

