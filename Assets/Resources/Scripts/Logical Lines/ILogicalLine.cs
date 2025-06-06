using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue.LogicalLines
{
    public interface ILogicalLine
    {
        string keyword { get; }

        bool Matches(DialogueLine line);

        IEnumerator Execute(DialogueLine line);
    }
}

