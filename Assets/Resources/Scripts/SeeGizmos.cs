using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeeGizmos : MonoBehaviour
{
    private CapsuleCollider2D sliderCollider;

    void Start()
    {
        sliderCollider = GetComponent<CapsuleCollider2D>();
    }

    void Update()
    {
        if (sliderCollider == null) return;

        Vector2 center = sliderCollider.bounds.center;
        Vector2 size = sliderCollider.bounds.size;

        // Calculate the corners of the box
        Vector2 topLeft = new Vector2(center.x - size.x / 2, center.y + size.y / 2);
        Vector2 topRight = new Vector2(center.x + size.x / 2, center.y + size.y / 2);
        Vector2 bottomLeft = new Vector2(center.x - size.x / 2, center.y - size.y / 2);
        Vector2 bottomRight = new Vector2(center.x + size.x / 2, center.y - size.y / 2);

        // Draw the box using lines in runtime
        Debug.DrawLine(topLeft, topRight, Color.red);
        Debug.DrawLine(topRight, bottomRight, Color.red);
        Debug.DrawLine(bottomRight, bottomLeft, Color.red);
        Debug.DrawLine(bottomLeft, topLeft, Color.red);
    }

    void OnDrawGizmos()
    {
        // Ensure the sliderCollider is assigned when not in play mode
        if (sliderCollider == null)
        {
            sliderCollider = GetComponent<CapsuleCollider2D>();
        }

        if (sliderCollider == null) return;

        // Set the color for the Gizmo box
        Gizmos.color = Color.red;

        // Draw the box in the Scene view
        Gizmos.DrawWireCube(sliderCollider.bounds.center, sliderCollider.bounds.size);
    }
}
