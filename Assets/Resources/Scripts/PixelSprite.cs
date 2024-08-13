using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PixelSprite
{
    public SceneManager sceneManager => SceneManager.Instance;
    public SpriteManager spriteManager => SpriteManager.Instance;

    public Transform root = null;

    public SpriteRenderer rootSpriteRenderer;
    public Animator rootAnimator;
    public CanvasGroup rootCanvasGroup;
    public string sceneToDisappear;

    public CurrentSpriteDirection currentDirection = CurrentSpriteDirection.Right;

    protected Coroutine showingSpriteCoroutine, hidingSpriteCoroutine;
    public Coroutine movingSpriteCoroutine;

    public bool isSpriteShowing => showingSpriteCoroutine != null;
    public bool isSpriteHiding => hidingSpriteCoroutine != null;
    public bool isSpriteMoving => movingSpriteCoroutine != null;

    private float fadeSpeed = 3f;

    public PixelSprite(GameObject prefab, Vector2 spritePosition, Vector2 spriteScale, BackgroundConfigData.PlayerDirection spriteDirection, Transform backgroundSpriteIsOn, string sceneToDisappear = "")
    {
        if (prefab != null)
        {
            GameObject ob = Object.Instantiate(prefab, backgroundSpriteIsOn);

            ob.SetActive(true);

            root = ob.GetComponent<Transform>();

            rootCanvasGroup = root.GetComponent<CanvasGroup>();
            rootSpriteRenderer = root.GetComponentInChildren<SpriteRenderer>();
            rootAnimator = rootSpriteRenderer.GetComponent<Animator>();
            this.sceneToDisappear = sceneToDisappear;

            Color spriteColor = rootSpriteRenderer.color;
            spriteColor.a = 0f;
            rootSpriteRenderer.color = spriteColor;

            SetPositionDirectionScale(spritePosition, spriteScale, spriteDirection);
        }
    }

    public void SetPositionDirectionScale(Vector2 position, Vector2 scale, BackgroundConfigData.PlayerDirection direction)
    {
        if (root == null) return;

        root.transform.position = position;
        root.localScale = scale;

        if (direction == BackgroundConfigData.PlayerDirection.left)
        {
            root.Find("Sprite").transform.eulerAngles = new Vector3(0, 180, 0);

            if(root.Find("Sprite").Find("Icon") != null)
            {
                root.Find("Sprite").Find("Icon").transform.eulerAngles = new Vector3(0, 0, 0);
            }
        }
        else if (direction == BackgroundConfigData.PlayerDirection.right)
        {
            root.Find("Sprite").transform.eulerAngles = new Vector3(0, 0, 0);
        }
    }

    public Coroutine MoveSprite(Vector3 currentPosition, Vector3 positionToGo, float speed, bool interacting = false, bool isPlayer = false)
    {
        if (isSpriteMoving) return movingSpriteCoroutine;

        positionToGo.y = currentPosition.y;

        if (interacting)
        {
            if (currentDirection == CurrentSpriteDirection.Left)
            {
                positionToGo.x = positionToGo.x + 1.8f; //if facing left, go right
            }
            else
            {
                positionToGo.x = positionToGo.x - 2.3f; //if facing right, go left
            }
        }

        if (Vector2.Distance(currentPosition, positionToGo) <= 0.01f)
        {
            movingSpriteCoroutine = null;
        }
        else
        {
            movingSpriteCoroutine = spriteManager.StartCoroutine(MovingSprite(currentPosition, positionToGo, speed, interacting, isPlayer));
        }

        return movingSpriteCoroutine;
    }

    private IEnumerator MovingSprite(Vector3 currentPosition, Vector3 positionToGo, float speed, bool interacting = false, bool isPlayer = false)
    {
        if (isPlayer) Player.playerBeingMoved = true;
        
        if (interacting)
        {
            if (currentDirection == CurrentSpriteDirection.Left)
            {
                if(isPlayer) Player.move.x = -1;
                FlipSprite(CurrentSpriteDirection.Right);
            }
            else
            {
                if (isPlayer) Player.move.x = 1;
                FlipSprite(CurrentSpriteDirection.Left);
            }
        }
        else
        {
            if (positionToGo.x < currentPosition.x)
            {
                if (isPlayer) Player.move.x = -1;
                FlipSprite(CurrentSpriteDirection.Left);
            }
            else if (positionToGo.x > currentPosition.x)
            {
                if (isPlayer) Player.move.x = 1;
                FlipSprite(CurrentSpriteDirection.Right);
            }
        }

        while (currentPosition.x != positionToGo.x)
        {
            root.transform.position = currentPosition;

            currentPosition = Vector3.MoveTowards(currentPosition, positionToGo, speed * Time.deltaTime);

            if (Vector2.Distance(currentPosition, positionToGo) <= 0.0001f)
            {
                if (isPlayer) Player.move = Vector3.zero;

                if (interacting)
                {
                    if (currentDirection == CurrentSpriteDirection.Left)
                    {
                        FlipSprite(CurrentSpriteDirection.Right);
                    }
                    else
                    {
                        FlipSprite(CurrentSpriteDirection.Left);
                    }
                }

                root.transform.position = positionToGo;

                if (isPlayer) Player.playerBeingMoved = false;

                break;
            }

            yield return null;
        }

        movingSpriteCoroutine = null;
    }

    public void FlipSprite(CurrentSpriteDirection direction)
    {
        if (direction == CurrentSpriteDirection.Left)
        {
            root.Find("Sprite").transform.eulerAngles = new Vector3(0, 180, 0);
            currentDirection = CurrentSpriteDirection.Left;
        }
        else
        {
            root.Find("Sprite").transform.eulerAngles = new Vector3(0, 0, 0);
            currentDirection = CurrentSpriteDirection.Right;
        }
    }

    public Coroutine Show(bool immediate = false)
    {
        if (isSpriteShowing) return showingSpriteCoroutine;

        if (isSpriteHiding)
        {
            spriteManager.StopCoroutine(hidingSpriteCoroutine);
        }

        showingSpriteCoroutine = spriteManager.StartCoroutine(ShowingOrHiding(true, immediate));

        return showingSpriteCoroutine;
    }

    public Coroutine Hide(bool immediate = false)
    {
        if (isSpriteHiding) return hidingSpriteCoroutine;

        if (isSpriteShowing)
        {
            spriteManager.StopCoroutine(showingSpriteCoroutine);
        }

        hidingSpriteCoroutine = spriteManager.StartCoroutine(ShowingOrHiding(false, immediate));

        return hidingSpriteCoroutine;
    }

    public IEnumerator ShowingOrHiding(bool show, bool immediate)
    {
        if (rootSpriteRenderer == null) yield break;

        float targetAlpha = show ? 1f : 0f;

        Color spriteColor = rootSpriteRenderer.color;

        if (immediate)
        {
            spriteColor.a = targetAlpha;
            rootCanvasGroup.alpha = targetAlpha;
        }
        else
        {
            while (spriteColor.a != targetAlpha)
            {
                spriteColor.a = Mathf.MoveTowards(spriteColor.a, targetAlpha, fadeSpeed * Time.deltaTime);
                rootSpriteRenderer.color = spriteColor;

                rootCanvasGroup.alpha = Mathf.MoveTowards(rootCanvasGroup.alpha, targetAlpha, fadeSpeed * Time.deltaTime);
                rootCanvasGroup.alpha = targetAlpha;

                if (spriteColor.a == 0f)
                {
                    Object.Destroy(rootSpriteRenderer.transform.parent.gameObject);
                    break;
                }

                yield return null;
            }
        }

        showingSpriteCoroutine = null;
        hidingSpriteCoroutine = null;
    }

    public enum CurrentSpriteDirection
    {
        Left,
        Right
    }
}
