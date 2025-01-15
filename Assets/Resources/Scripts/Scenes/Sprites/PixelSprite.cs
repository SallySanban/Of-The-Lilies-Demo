using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelSprite
{
    //uses local position
    protected void SetPosition(GameObject npc, Vector2 targetPosition)
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
}
