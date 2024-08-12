using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    void Update()
    {
        Vector3 position = GameObject.FindWithTag("MainCamera").transform.position;
        position.z = 1f;

        transform.position = position;
        GetComponent<RectTransform>().localScale = new Vector3(GetComponent<RectTransform>().localScale.x, GetComponent<RectTransform>().localScale.y, 0f);
    }
}
