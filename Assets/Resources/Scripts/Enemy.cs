using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : PixelSprite
{
    private float speed = 3f;
    public float attackRange = 0.94f;

    public Enemy(GameObject prefab, Vector2 spritePosition, Vector2 spriteScale, BackgroundConfigData.PlayerDirection spriteDirection, Transform backgroundSpriteIsOn) : base(prefab, spritePosition, spriteScale, spriteDirection, backgroundSpriteIsOn)
    {

    }

    public void FollowPlayer(Transform self)
    {
        Transform player = spriteManager.currentPlayer.root.transform;

        LookAtPlayer(player, self);

        Vector2 target = new Vector2(player.position.x, self.position.y);
        Vector2 position = Vector2.MoveTowards(self.position, target, speed * Time.fixedDeltaTime);

        self.GetComponent<Rigidbody2D>().MovePosition(position);

        if (Vector2.Distance(player.position, self.position) <= attackRange)
        {
            //animator.SetTrigger("Attack");
        }
    }

    private void LookAtPlayer(Transform player, Transform self)
    {
        if (self.position.x > player.position.x)
        {
            self.Find("Sprite").transform.eulerAngles = new Vector3(0, 180, 0);
        }
        else if (self.position.x < player.position.x)
        {
            self.Find("Sprite").transform.eulerAngles = new Vector3(0, 0, 0);
        }
    }
}
