using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelSprite
{
    private bool spriteCurrentlyMoving = false;

    //uses local position
    public void SetPosition(GameObject npc, Vector2 targetPosition)
    {
        Transform npcTransform = npc.transform;
        Vector3 currentPos = npcTransform.localPosition;
        npcTransform.localPosition = new Vector3(targetPosition.x, targetPosition.y, currentPos.z);
    }

    //uses world position
    public IEnumerator MoveToPosition(GameObject npc, Vector2 targetPosition, float speed = 4f, bool smooth = false)
    {
        Transform npcTransform = npc.transform;
        Vector3 startPosition = npcTransform.position;
        Vector3 target = new Vector3(targetPosition.x, targetPosition.y, startPosition.z);
        
        // Get the parent's scale to adjust speed
        float scaleAdjustment = npcTransform.parent ? npcTransform.parent.localScale.x : 1f;
        float adjustedSpeed = speed * scaleAdjustment;

        while (Vector3.Distance(npcTransform.position, target) > 0.01f)
        {
            if (smooth)
            {
                npcTransform.position = Vector3.Lerp(
                    npcTransform.position,
                    target,
                    adjustedSpeed * Time.deltaTime
                );
            }
            else
            {
                npcTransform.position = Vector3.MoveTowards(
                    npcTransform.position,
                    target,
                    adjustedSpeed * Time.deltaTime
                );
            }
            yield return null;
        }

        npcTransform.position = target;
    }

    public IEnumerator FollowPlayer(GameObject npc, float followDistance = 2f, float speed = 6f)
    {
        Transform npcTransform = npc.transform;
        Transform playerTransform = SceneManager.Instance.player.root.transform;

        while (SceneManager.Instance.followPlayer)
        {
            Vector3 npcPosition = npcTransform.position;
            Vector3 playerPosition = new Vector2(playerTransform.position.x, npcPosition.y);

            float distanceToPlayer = Vector3.Distance(npcPosition, playerPosition);


            if (!spriteCurrentlyMoving)
            {
                Vector3 directionToPlayer = (playerPosition - npcPosition).normalized;

                if (directionToPlayer.x > 0)
                {
                    npcTransform.localScale = new Vector3(Mathf.Abs(npcTransform.localScale.x), npcTransform.localScale.y, npcTransform.localScale.z);
                }
                else if (directionToPlayer.x < 0)
                {
                    npcTransform.localScale = new Vector3(-Mathf.Abs(npcTransform.localScale.x), npcTransform.localScale.y, npcTransform.localScale.z);
                }

                if (distanceToPlayer > followDistance)
                {
                    npcTransform.position = Vector3.MoveTowards(
                        npcPosition,
                        playerPosition,
                        speed * Time.deltaTime
                    );
                }
            }

            yield return null;
        }
    }

    public IEnumerator GoBehindPlayer(bool movedLeft, float followDistance = 2f, float speed = 5f)
    {
        spriteCurrentlyMoving = true;

        Transform npcTransform = SceneManager.Instance.follower.root.transform;
        Transform playerTransform = SceneManager.Instance.player.root.transform;

        float offsetX = movedLeft ? -followDistance : followDistance;

        Vector3 targetPosition = new Vector3(
            playerTransform.position.x + offsetX,
            npcTransform.position.y,
            npcTransform.position.z
        );

        while (Vector3.Distance(npcTransform.position, targetPosition) > 0.01f)
        {
            Vector3 directionOfMotion = targetPosition - npcTransform.position;

            if (directionOfMotion.x > 0)
            {
                npcTransform.localScale = new Vector3(
                    Mathf.Abs(npcTransform.localScale.x),
                    npcTransform.localScale.y,
                    npcTransform.localScale.z
                );
            }
            else if (directionOfMotion.x < 0)
            {
                npcTransform.localScale = new Vector3(
                    -Mathf.Abs(npcTransform.localScale.x),
                    npcTransform.localScale.y,
                    npcTransform.localScale.z
                );
            }

            npcTransform.position = Vector3.MoveTowards(
                npcTransform.position,
                targetPosition,
                speed * Time.deltaTime
            );

            yield return null;
        }

        Vector3 directionToPlayer = playerTransform.position - npcTransform.position;

        if (directionToPlayer.x > 0)
        {
            npcTransform.localScale = new Vector3(Mathf.Abs(npcTransform.localScale.x), npcTransform.localScale.y, npcTransform.localScale.z);
        }
        else if (directionToPlayer.x < 0)
        {
            npcTransform.localScale = new Vector3(-Mathf.Abs(npcTransform.localScale.x), npcTransform.localScale.y, npcTransform.localScale.z);
        }

        yield return null;

        spriteCurrentlyMoving = false;
    }
}
