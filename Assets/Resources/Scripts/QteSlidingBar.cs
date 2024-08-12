using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QteSlidingBar
{
    [SerializeField] Image slidingBar;
    [SerializeField] Image slider;
    [SerializeField] Image arrow;
    [SerializeField] GameObject leftPoint;
    [SerializeField] GameObject rightPoint;

    private float speed = 3f;
    private Vector3 targetPosition;

    public GameObject root = null;

    public QteSlidingBar()
    {

    }

    //void Start()
    //{
    //    targetPosition = rightPoint.transform.position;
    //}

    
    //void Update()
    //{
    //    arrow.transform.position = Vector3.MoveTowards(arrow.transform.position, targetPosition, speed * Time.deltaTime);

    //    if (Vector3.Distance(arrow.transform.position, rightPoint.transform.position) < 0.1f)
    //    {
    //        targetPosition = leftPoint.transform.position;
    //    }
    //    else if (Vector3.Distance(arrow.transform.position, leftPoint.transform.position) < 0.1f)
    //    {
    //        targetPosition = rightPoint.transform.position;
    //    }
    //}

    //if hit, then attack soldier. if miss, soldier attack player. start from the beginning.
}
