using UnityEngine;

public class ScrollBackground : MonoBehaviour
{
    [SerializeField] private float scrollSpeed;

    private SpriteRenderer spriteRenderer;
    private float originalWidth;
    private float width;
    private const float SCROLL_LIMIT = 202.08f;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        originalWidth = spriteRenderer.size.x;
        width = originalWidth;
    }

    private void LateUpdate()
    {
        if (spriteRenderer.size.x >= SCROLL_LIMIT) width = originalWidth;

        width += scrollSpeed * Time.deltaTime;

        spriteRenderer.size = new Vector2(width, spriteRenderer.size.y);
    }
}
