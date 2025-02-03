using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEditor.Animations;
using UnityEngine;
using FMODUnity;

public class Player : PixelSprite
{
    public static Player Instance { get; private set; }

    public Vector3 move = Vector3.zero;
    private const float SPEED = 3f;
    private const float STEPS = 2f;

    //FMOD Footstep Event
    private float footstepTimer = 0f;
    private float footstepInterval = 0.4f; //Set to match player movespeed

    // Surface detection
    private string currentSurface = "Grass"; // Default surface type

    // Raycast visualization
    [SerializeField] private Color raycastColor = Color.green; // Color of the Raycast line
    [SerializeField] private float raycastDistance = 10f; // Distance of the Raycast

    public bool movingToInteract = false;


    public Player(GameObject prefab, Transform parent, Vector2 position, int direction, string animationState)
    {
        Instance = this;
        
        root = Object.Instantiate(prefab, Vector2.zero, Quaternion.identity);
        root.transform.SetParent(parent, false);
        SetPosition(root, position);

        animator = root.GetComponentInChildren<Animator>();

        animator.Play(animationState);

        animator.SetBool("Flipped", direction < 0);

        //FOR DEBUGGING PURPOSES
        //root.AddComponent<PositionDebugger>();
    }

    //uses world position
    public void Move()
    {
        if (move.x != 0)
        {
            animator.SetBool("isWalking", true);
            animator.SetBool("Flipped", move.x < 0);
            
            Vector3 movement = new Vector3(move.x * STEPS, 0, 0) * SPEED * Time.deltaTime;
            root.transform.position += movement;

            // Plays footstep sound
            PlayFootstepSound();
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }

    public void StopMoving()
    {
        animator.SetBool("isWalking", false);
        move = Vector2.zero;
    }

    //uses world position
    public IEnumerator MoveToInteract(Vector2 position)
    {
        movingToInteract = true;

        Vector2 targetPos = position;
        Vector2 currentPos = root.transform.position;
        bool movedLeft = false;
        
        while (Vector2.Distance(currentPos, targetPos) > 0.1f)
        {
            currentPos = root.transform.position;
            Vector2 direction = (targetPos - currentPos).normalized;
            
            movedLeft = direction.x < 0;
            
            animator.SetBool("isWalking", true);
            animator.SetBool("Flipped", movedLeft); //true means going left
            
            root.transform.position += new Vector3(direction.x, 0, 0) * SPEED * Time.deltaTime;

            // Plays footstep sound
            PlayFootstepSound();

            yield return null;
        }
        
        animator.SetBool("isWalking", false);
        animator.SetBool("Flipped", !movedLeft);

        if(SceneManager.Instance.followPlayer) yield return SceneManager.Instance.follower.GoBehindPlayer(movedLeft);

        movingToInteract = false;
    }

    public void Flip(int direction)
    {
        bool flipDirection = direction < 0; //-1 is true => left

        animator.SetBool("Flipped", flipDirection);
    }
    private void PlayFootstepSound()
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

    private void DetectSurface()
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
            Debug.Log("Hit Object: " + hit.collider.gameObject.name);
            Debug.Log("Hit Tag: " + hit.collider.tag);

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
            else
            {
                currentSurface = "Grass"; // Default surface
            }
        }
    }

    private string GetSurfaceName(string surface)
    {
        // Map surface names to FMOD parameter values
        switch (surface)
        {
            case "Grass": return "Grass";
            case "Wood": return "Wood";
            case "Stone": return "Stone";
            default: return "Grass"; // Default to Grass
        }
    }
}
