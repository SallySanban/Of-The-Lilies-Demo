using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class CombatSprite
{
    public GameObject root;
    public Animator animator;

    //FMOD Footstep Event
    private float footstepTimer = 0f;
    private float footstepInterval = 0.4f; //Set to match player movespeed

    // Surface detection
    private string currentSurface = "Grass"; // Default surface type

    // Raycast visualization
    [SerializeField] private Color raycastColor = Color.green; // Color of the Raycast line
    [SerializeField] private float raycastDistance = 10f; // Distance of the Raycast

    private float moveSpeed = 4f;

    public CombatSprite(GameObject prefab, Transform parent, Vector2 position)
    {
        root = Object.Instantiate(prefab, position, Quaternion.identity, parent);

        animator = root.GetComponentInChildren<Animator>();

        SceneManager.Instance.playerCamera.Follow = root.transform;
    }

    public IEnumerator ChangePositionA(float targetX)
    {
        Vector3 startPosition = root.transform.position;
        Vector3 target = new Vector3(targetX, startPosition.y, startPosition.z);

        //float scaleAdjustment = npcTransform.parent ? npcTransform.parent.localScale.x : 1f;
        //float adjustedSpeed = moveSpeed * scaleAdjustment;

        while (Vector3.Distance(root.transform.position, target) > 0.01f)
        {
            Vector2 direction = (target - root.transform.position).normalized;

            if(direction.x < 0) //going left
            {
                animator.SetBool("Back", true);
            }
            else
            {
                animator.SetBool("Forward", true);
            }

            root.transform.position = Vector3.MoveTowards(
                    root.transform.position,
                    target,
                    moveSpeed * Time.deltaTime
                );

            PlayFootstepSound();
            yield return null;
        }

        animator.SetBool("Forward", false);
        animator.SetBool("Back", false);

        root.transform.position = target;
    }

    //FMOD Footstep audio relevant codes below
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
            else if (hit.collider.CompareTag("CarpetedStone"))
            {
                currentSurface = "CarpetedStone";
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
            case "CarpetedStone": return "CarpetedStone";
            default: return "Grass"; // Default to Grass
        }
    }
}
