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
            CharacterSprite Myo = CharacterManager.Instance.CreateCharacter("Myo") as CharacterSprite;
            CharacterSprite Seiji = CharacterManager.Instance.CreateCharacter("Seiji") as CharacterSprite;

            Myo.MoveToPosition(Vector2.zero);

            Seiji.Show();

            yield return new WaitForSeconds(1f);

            //yield return Seiji1.MoveToPosition(Vector2.zero);
            //yield return Seiji2.MoveToPosition(Vector2.one);

            //Sprite bodySeiji = Seiji2.GetSprite("Seiji");
            Sprite faceSeiji = Seiji.GetSprite("Pout");

            //Seiji.SetSprite(faceSeiji, 1);
            //Seiji2.TransitionSprite(bodySeiji);
            yield return Seiji.TransitionSprite(faceSeiji, 1);

            yield return new WaitForSeconds(1f);
            //Seiji.SetSprite(Seiji.GetSprite("Displeased"), 1);

            yield return Seiji.TransitionSprite(Seiji.GetSprite("Displeased"), 1);

            yield return new WaitForSeconds(1f);

            Seiji.MoveToPosition(Vector2.one);
            Myo.Show();

            yield return null;
        }
    }
}

