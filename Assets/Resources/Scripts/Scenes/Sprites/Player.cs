using System.Collections;
using UnityEngine;
using FMODUnity;

public class Player : PixelSprite
{
    public static Player Instance { get; private set; }

    public Vector3 move = Vector3.zero;
    private const float SPEED = 3f;
    private const float STEPS = 2f;

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
        
        while (Mathf.Abs(currentPos.x - targetPos.x) > 0.1f)
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

        // Wait a small frame to ensure the player has completely stopped
        yield return new WaitForSeconds(0.1f);

        if (SceneManager.Instance.followPlayer)
        {
            yield return SceneManager.Instance.follower.GoBehindPlayer(movedLeft);
        }

        movingToInteract = false;
    }
}
