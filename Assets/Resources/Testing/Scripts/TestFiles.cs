using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Testing
{
    public class TestFiles : MonoBehaviour
    {
        [SerializeField] private TextAsset fileName;

        private void Start()
        {
            StartCoroutine(Run());
        }

        IEnumerator Run()
        {
            List<string> lines = FileManager.ReadTextAsset(fileName, true);

            foreach (string line in lines)
            {
                Debug.Log(line);
            }

            yield return null;
        }
    }
}
