using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Player : PixelSprite
{
    public static Vector3 move = Vector3.zero;
    private float speed = 3f;
    private float steps = 2f;

    public Player(GameObject prefab, Vector2 spritePosition, BackgroundConfigData.PlayerDirection spriteDirection, Transform backgroundSpriteIsOn) : base(prefab, spritePosition, spriteDirection, backgroundSpriteIsOn)
    {

    }

    public void MoveToInteract(Vector3 interactablePosition)
    {
        Debug.Log(interactablePosition);

        //move.x = 1;

        Vector2 currentPosition = root.transform.position;
        Debug.Log("CURRENT POSITION: " + currentPosition);

        Vector2 positionToGo = interactablePosition + new Vector3(4f,currentPosition.y);//new Vector3(2.79f, 0f);
        Debug.Log("POSITION TO GO: " + positionToGo);

        spriteManager.StartCoroutine(MovePlayerToInteract(currentPosition, positionToGo));
    }

    private IEnumerator MovePlayerToInteract(Vector3 currentPosition, Vector3 positionToGo)
    {
        while (root.transform.position.x != positionToGo.x)
        {
            root.transform.position = Vector3.MoveTowards(root.transform.position, new Vector3(positionToGo.x, root.transform.position.y), speed * Time.deltaTime);

            if (Vector2.Distance(root.transform.position, positionToGo) <= 0.0001f)
            {
                root.transform.position = positionToGo;

                break;
            }

            yield return null;
        }
    }

    public void AnimatePlayer()
    {
        if (move == Vector3.zero)
        {
            rootAnimator.SetBool("Walk", false);
        }
        else
        {
            rootAnimator.SetBool("Walk", true);
        }
    }

    public void MoveSprite()
    {
        return;
        move.y = 0;
        move.z = 0;

        if (move.x == -1) //left
        {
            root.Find("Sprite").transform.eulerAngles = new Vector3(0, 180, 0);

            move.x -= steps;
        }
        else if (move.x == 1) //right
        {
            root.Find("Sprite").transform.eulerAngles = new Vector3(0, 0, 0);

            move.x += steps;
        }

        root.transform.position += move * speed * Time.deltaTime;
    }
}
