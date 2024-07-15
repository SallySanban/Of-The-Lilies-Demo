using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commands;

namespace Testing
{
    public class TestCommand : MonoBehaviour
    {
        void Start()
        {
            StartCoroutine(Running());
        }

        IEnumerator Running()
        {
            yield return CommandManager.Instance.Execute("Print");
            yield return CommandManager.Instance.Execute("Print_1P", "Hello!");
            yield return CommandManager.Instance.Execute("Print_MP", "Line1", "Line2", "Line3");

            yield return CommandManager.Instance.Execute("lambda");
            yield return CommandManager.Instance.Execute("lambda_1P", "Hello!");
            yield return CommandManager.Instance.Execute("lambda_MP", "Line1", "Line2", "Line3");

            yield return CommandManager.Instance.Execute("Process");
            yield return CommandManager.Instance.Execute("Process_1P", "3");
            yield return CommandManager.Instance.Execute("Process_MP", "ProcessLine1", "ProcessLine2", "ProcessLine3");
        }
    }
}

