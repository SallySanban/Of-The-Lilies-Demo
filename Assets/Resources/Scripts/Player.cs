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

    public void MoveSprite()
    {
        move.y = 0;
        move.z = 0;

        if (move.x == -1) //left
        {
            root.transform.eulerAngles = new Vector3(0, 180, 0);

            move.x -= steps;
        }
        else if (move.x == 1) //right
        {
            root.transform.eulerAngles = new Vector3(0, 0, 0);

            move.x += steps;
        }

        root.transform.position += move * speed * Time.deltaTime;
    }
}
