using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : PixelSprite
{
    public static Vector3 move = Vector3.zero;
    private float speed = 3f;
    private float steps = 2f;

    public static bool playerBeingMoved = false;

    public Player(GameObject prefab, Vector2 spritePosition, Vector2 spriteScale, BackgroundConfigData.PlayerDirection spriteDirection, Transform backgroundSpriteIsOn) : base(prefab, spritePosition, spriteScale, spriteDirection, backgroundSpriteIsOn)
    {

    }

    public void AnimatePlayerWalk()
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
            FlipSprite(CurrentSpriteDirection.Left);

            move.x -= steps;
        }
        else if (move.x == 1) //right
        {
            FlipSprite(CurrentSpriteDirection.Right);

            move.x += steps;
        }

        root.transform.position += move * speed * Time.deltaTime;
    }
}
