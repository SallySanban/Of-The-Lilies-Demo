using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : PixelSprite
{
    public static Vector3 move = Vector3.zero;
    public static CurrentPlayerDirection currentDirection = CurrentPlayerDirection.Right;
    private float speed = 3f;
    private float steps = 2f;

    public static bool playerBeingMoved = false;

    public Coroutine movingPlayerCoroutine;
    public bool isPlayerMoving => movingPlayerCoroutine != null;

    public Player(GameObject prefab, Vector2 spritePosition, BackgroundConfigData.PlayerDirection spriteDirection, Transform backgroundSpriteIsOn) : base(prefab, spritePosition, spriteDirection, backgroundSpriteIsOn)
    {

    }

    public Coroutine MoveToInteract(Vector3 interactablePosition)
    {
        if (isPlayerMoving) return movingPlayerCoroutine;

        Vector2 currentPosition = root.transform.position;
        Vector2 positionToGo;

        if(currentDirection == CurrentPlayerDirection.Left)
        {
            positionToGo = new Vector3(interactablePosition.x + 3.5f, currentPosition.y); //if facing left, go right
        }
        else
        {
            positionToGo = new Vector3(interactablePosition.x - 0.5f, currentPosition.y); //if facing right, go left
        }
        
        if (Vector2.Distance(root.transform.position, positionToGo) <= 0.01f)
        {
            movingPlayerCoroutine = null;
        }
        else
        {
            movingPlayerCoroutine = spriteManager.StartCoroutine(MovePlayerToInteract(positionToGo));
        }
        
        return movingPlayerCoroutine;
    }

    private IEnumerator MovePlayerToInteract(Vector3 positionToGo)
    {
        playerBeingMoved = true;

        if (currentDirection == CurrentPlayerDirection.Left)
        {
            move.x = -1;
            FlipPlayer(CurrentPlayerDirection.Right);
        }
        else
        {
            move.x = 1;
            FlipPlayer(CurrentPlayerDirection.Left);
        }

        while (root.transform.position.x != positionToGo.x)
        {
            root.transform.position = Vector3.MoveTowards(root.transform.position, new Vector3(positionToGo.x, root.transform.position.y), speed * Time.deltaTime);

            if (Vector2.Distance(root.transform.position, positionToGo) <= 0.0001f)
            {
                move = Vector3.zero;

                if (currentDirection == CurrentPlayerDirection.Left)
                {
                    FlipPlayer(CurrentPlayerDirection.Right);
                }
                else
                {
                    FlipPlayer(CurrentPlayerDirection.Left);
                }

                root.transform.position = positionToGo;

                playerBeingMoved = false;

                break;
            }

            yield return null;
        }

        movingPlayerCoroutine = null;
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
        move.y = 0;
        move.z = 0;

        if (move.x == -1) //left
        {
            FlipPlayer(CurrentPlayerDirection.Left);

            move.x -= steps;
        }
        else if (move.x == 1) //right
        {
            FlipPlayer(CurrentPlayerDirection.Right);

            move.x += steps;
        }

        root.transform.position += move * speed * Time.deltaTime;
    }

    private void FlipPlayer(CurrentPlayerDirection direction)
    {
        if(direction == CurrentPlayerDirection.Left)
        {
            root.Find("Sprite").transform.eulerAngles = new Vector3(0, 180, 0);
            currentDirection = CurrentPlayerDirection.Left;
        }
        else
        {
            root.Find("Sprite").transform.eulerAngles = new Vector3(0, 0, 0);
            currentDirection = CurrentPlayerDirection.Right;
        }
    }

    public enum CurrentPlayerDirection
    {
        Left,
        Right
    }
}
