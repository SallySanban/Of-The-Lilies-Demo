using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelSprite
{
    protected void SetPosition(GameObject npc, Vector2 targetPosition)
    {
        Transform npcTransform = npc.transform;
        Vector3 currentPos = npcTransform.localPosition;
        npcTransform.localPosition = new Vector3(targetPosition.x, targetPosition.y, currentPos.z);
    }

    private IEnumerator MoveToPosition(GameObject npc, Vector2 targetPosition, float speed = 4f, bool smooth = true)
    {
        Transform npcTransform = npc.transform;
        Vector3 startPosition = npcTransform.localPosition;
        Vector3 target = new Vector3(targetPosition.x, targetPosition.y, startPosition.z);

        while (Vector3.Distance(npcTransform.localPosition, target) > 0.01f)
        {
            if (smooth)
            {
                npcTransform.localPosition = Vector3.Lerp(
                    npcTransform.localPosition,
                    target,
                    speed * Time.deltaTime
                );
            }
            else
            {
                npcTransform.localPosition = Vector3.MoveTowards(
                    npcTransform.localPosition,
                    target,
                    speed * Time.deltaTime
                );
            }
            yield return null;
        }

        npcTransform.localPosition = target;
    }
}
