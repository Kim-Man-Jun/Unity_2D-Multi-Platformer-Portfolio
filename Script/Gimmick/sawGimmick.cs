using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sawGimmick : MonoBehaviour
{
    public bool moveVertically = false;
    public bool moveHorizontally = false;

    public float moveSpeed = 5f;
    public float moveDistance = 2f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        float delta = Mathf.PingPong(Time.time * moveSpeed, moveDistance * 2) - moveDistance;

        Vector3 targetPos = startPosition;

        if (moveVertically)
        {
            targetPos += Vector3.up * delta;
        }

        if (moveHorizontally)
        {
            targetPos += Vector3.right * delta;
        }

        transform.position = targetPos;
    }
}
