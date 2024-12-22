using UnityEngine;

public class PositionDebugger : MonoBehaviour
{
    private Vector3 lastPosition;

    void Update()
    {
        if (transform.position != lastPosition)
        {
            PrintPosition();
            lastPosition = transform.position;
        }
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying && transform.position != lastPosition)
        {
            PrintPosition();
            lastPosition = transform.position;
        }
    }

    private void PrintPosition()
    {
        Debug.Log($"GameObject '{gameObject.name}' world position: {transform.position}");
        Debug.Log($"GameObject '{gameObject.name}' local position: {transform.localPosition}");
    }
}