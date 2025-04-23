using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatSprite
{
    public GameObject root;
    public Animator animator;

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

            yield return null;
        }

        animator.SetBool("Forward", false);
        animator.SetBool("Back", false);

        root.transform.position = target;
    }
}
