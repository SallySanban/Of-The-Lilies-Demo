using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class Player : PixelSprite
{
    public static Player Instance { get; private set; }

    public GameObject root = null;
    
    private Animator animator = null;

    public Vector3 move = Vector3.zero;
    private const float SPEED = 3f;
    private const float STEPS = 2f;

    public Player(GameObject prefab, Transform parent, Vector2 position, int direction)
    {
        Instance = this;
        
        root = Object.Instantiate(prefab, Vector2.zero, Quaternion.identity);
        root.transform.SetParent(parent, false);
        SetPosition(root, position);

        animator = root.GetComponentInChildren<Animator>();

        animator.SetBool("Flipped", direction < 0);

        //FOR DEBUGGING PURPOSES
        //root.AddComponent<PositionDebugger>();
    }

    public void Move()
    {
        if (move.x != 0)
        {
            //animator.SetBool("IsWalking", true);
            animator.SetBool("Flipped", move.x < 0);
            
            Vector3 movement = new Vector3(move.x * STEPS, 0, 0) * SPEED * Time.deltaTime;
            root.transform.position += movement;
        }
        else
        {
            //animator.SetBool("IsWalking", false);
        }
    }
}
