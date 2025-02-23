using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class PixelSprite
{
    private bool spriteCurrentlyMoving = false;

    public GameObject root = null;
    protected Animator animator = null;

    //FMOD Footstep Event
    private float footstepTimer = 0f;
    private float footstepInterval = 0.4f; //Set to match player movespeed

    // Surface detection
    private string currentSurface = "Grass"; // Default surface type

    // Raycast visualization
    [SerializeField] private Color raycastColor = Color.green; // Color of the Raycast line
    [SerializeField] private float raycastDistance = 10f; // Distance of the Raycast

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

                PlayFootstepSound();
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

                PlayFootstepSound();
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

    public void PlayFootstepSound()
    {
        footstepTimer += Time.deltaTime;

        if (footstepTimer >= footstepInterval)
        {
            // Detect the current surface
            DetectSurface();

            // Play footstep sound with the current surface type
            FMOD.Studio.EventInstance footstepInstance = RuntimeManager.CreateInstance("event:/SFX/SFX_Footsteps");
            footstepInstance.setParameterByNameWithLabel("SurfaceType", currentSurface);
            footstepInstance.start();
            footstepInstance.release(); // Release the instance after playing

            footstepTimer = 0f; // Reset the timer
        }
    }

    public void DetectSurface()
    {
        // Raycast to detect the surface below the player
        Vector2 raycastOrigin = root.transform.position;
        Vector2 raycastDirection = Vector2.down;
        int ignoreLayers = ~LayerMask.GetMask("UI");

        RaycastHit2D hit = Physics2D.Raycast(raycastOrigin, raycastDirection, raycastDistance, 1 << 11 & ignoreLayers);

        // Draw the Raycast in the Scene view
        Debug.DrawRay(raycastOrigin, raycastDirection * raycastDistance, Color.red, 0.1f, false);

        if (hit.collider != null)
        {
            //Debug.Log("Hit Object: " + hit.collider.gameObject.name);
            //Debug.Log("Hit Tag: " + hit.collider.tag);

            // Check the surface tag or layer
            if (hit.collider.CompareTag("Grass"))
            {
                currentSurface = "Grass";
            }
            else if (hit.collider.CompareTag("Wood"))
            {
                currentSurface = "Wood";
            }
            else if (hit.collider.CompareTag("Stone"))
            {
                currentSurface = "Stone";
            }
            else if (hit.collider.CompareTag("Wood2"))
            {
                currentSurface = "Wood2";
            }
            else
            {
                currentSurface = "Grass"; // Default surface
            }
        }
    }

    public string GetSurfaceName(string surface)
    {
        // Map surface names to FMOD parameter values
        switch (surface)
        {
            case "Grass": return "Grass";
            case "Wood": return "Wood";
            case "Stone": return "Stone";
            case "Wood2": return "Wood2";
            default: return "Grass"; // Default to Grass
        }
    }
}
