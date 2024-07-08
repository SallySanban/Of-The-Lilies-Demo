using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Characters;
using Dialogue;

namespace Testing
{
    public class TestCharacters : MonoBehaviour
    {
        void Start()
        {
            
            //Character Ahlai = CharacterManager.Instance.CreateCharacter("Ahlai");
            //Character Sabina = CharacterManager.Instance.CreateCharacter("Lady Sabina");

            StartCoroutine(Test());
        }

        IEnumerator Test()
        {
            Character Seiji1 = CharacterManager.Instance.CreateCharacter("Random as Seiji");
            Character Seiji2 = CharacterManager.Instance.CreateCharacter("Random2 as Seiji");
            Character Seiji3 = CharacterManager.Instance.CreateCharacter("Random3 as Seiji");

            Seiji1.Show();
            Seiji2.Show();
            Seiji3.Show();

            yield return null;
        }
    }
}

