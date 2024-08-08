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

    public Coroutine MoveSprite(Vector3 currentPosition, Vector3 positionToGo, float speed, bool interacting = false)
    {
        if (isPlayerMoving) return movingPlayerCoroutine;

        positionToGo.y = currentPosition.y;

        if (interacting)
        {
            if (currentDirection == CurrentPlayerDirection.Left)
            {
                positionToGo.x = positionToGo.x + 3.5f; //if facing left, go right
            }
            else
            {
                positionToGo.x = positionToGo.x - 0.5f; //if facing right, go left
            }
        }
        
        if (Vector2.Distance(currentPosition, positionToGo) <= 0.01f)
        {
            movingPlayerCoroutine = null;
        }
        else
        {
            movingPlayerCoroutine = spriteManager.StartCoroutine(MovingSprite(currentPosition, positionToGo, speed, interacting));
        }
        
        return movingPlayerCoroutine;
    }

    private IEnumerator MovingSprite(Vector3 currentPosition, Vector3 positionToGo, float speed, bool interacting = false)
    {
        playerBeingMoved = true;

        if (interacting)
        {
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
        }
        else
        {
            if(positionToGo.x < currentPosition.x)
            {
                move.x = -1;
                FlipPlayer(CurrentPlayerDirection.Left);
            }
            else if(positionToGo.x > currentPosition.x)
            {
                move.x = 1;
                FlipPlayer(CurrentPlayerDirection.Right);
            }
        }

        while (currentPosition.x != positionToGo.x)
        {
            root.transform.position = currentPosition;

            currentPosition = Vector3.MoveTowards(currentPosition, positionToGo, speed * Time.deltaTime);

            if (Vector2.Distance(currentPosition, positionToGo) <= 0.0001f)
            {
                move = Vector3.zero;

                if (interacting)
                {
                    if (currentDirection == CurrentPlayerDirection.Left)
                    {
                        FlipPlayer(CurrentPlayerDirection.Right);
                    }
                    else
                    {
                        FlipPlayer(CurrentPlayerDirection.Left);
                    }
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

    public void MovePlayer()
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
