using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelSprite
{
    private bool spriteCurrentlyMoving = false;

    public GameObject root = null;
    protected Animator animator = null;

    //uses local position
    public void SetPosition(GameObject npc, Vector2 targetPosition)
    {
        Transform npcTransform = npc.transform;
        Vector3 currentPos = npcTransform.localPosition;
        npcTransform.localPosition = new Vector3(targetPosition.x, targetPosition.y, currentPos.z);
    }

    //uses world position
    public IEnumerator MoveToPosition(PixelSprite npc, Vector2 targetPosition, float speed = 4f, bool smooth = false)
    {
        Transform npcTransform = npc.root.transform;
        Vector3 startPosition = npcTransform.position;
        Vector3 target = new Vector3(targetPosition.x, targetPosition.y, startPosition.z);
        
        // Get the parent's scale to adjust speed
        float scaleAdjustment = npcTransform.parent ? npcTransform.parent.localScale.x : 1f;
        float adjustedSpeed = speed * scaleAdjustment;

        while (Vector3.Distance(npcTransform.position, target) > 0.01f)
        {
            Vector2 direction = (target - npcTransform.position).normalized;

            if (HasParameter(animator, "isWalking"))
            {
                animator.SetBool("isWalking", true);
            }
            
            if (HasParameter(animator, "Flipped"))
            {
                animator.SetBool("Flipped", direction.x < 0); //true means going left
            }

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

        if (HasParameter(animator, "isWalking"))
        {
            animator.SetBool("isWalking", false);
        }

        npcTransform.position = target;
    }

    public IEnumerator FollowPlayer(GameObject npc, float followDistance = 3f, float speed = 6f)
    {
        Transform npcTransform = npc.transform;
        Transform playerTransform = SceneManager.Instance.player.root.transform;

        while (SceneManager.Instance.followPlayer)
        {
            if (spriteCurrentlyMoving)
            {
                yield return null;
                continue;
            }

            Vector3 npcPosition = npcTransform.position;
            Vector3 playerPosition = new Vector2(playerTransform.position.x, npcPosition.y);

            float distanceToPlayer = Vector3.Distance(npcPosition, playerPosition);
            Vector3 directionToPlayer = (playerPosition - npcPosition).normalized;

            animator.SetBool("Flipped", directionToPlayer.x < 0);

            if (distanceToPlayer > followDistance && !SceneManager.Instance.player.movingToInteract)
            {
                animator.SetBool("isWalking", true);

                npcTransform.position = Vector3.MoveTowards(
                    npcPosition,
                    playerPosition,
                    speed * Time.deltaTime
                );
            }
            else
            {
                animator.SetBool("isWalking", false);
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

        bool direction = (targetPosition.x - npcTransform.position.x) < 0;
        animator.SetBool("Flipped", direction);

        float distanceToTarget = Vector3.Distance(npcTransform.position, targetPosition);
        if (distanceToTarget > 0.01f)
        {
            animator.SetBool("isWalking", true);
            
            while (Vector3.Distance(npcTransform.position, targetPosition) > 0.01f)
            {
                npcTransform.position = Vector3.MoveTowards(
                    npcTransform.position,
                    targetPosition,
                    speed * Time.deltaTime
                );
                yield return null;
            }
        }

        animator.SetBool("isWalking", false);
        animator.SetBool("Flipped", !direction);
        
        spriteCurrentlyMoving = false;
    }

    public bool HasParameter(Animator animator, string paramName)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName)
            {
                return true;
            }
        }
        return false;
    }

    public void Flip(int direction)
    {
        bool flipDirection = direction < 0; //-1 is true => left

        animator.SetBool("Flipped", flipDirection);
    }
}
