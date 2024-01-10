using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spikedBallMoving : MonoBehaviour
{
    Rigidbody2D rb;

    float coolTime;
    public float coolTimeMax;

    //움직임 좌우 구분
    float randomDir;
    float jumpPower = 1.2f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, 10f);
    }

    void Update()
    {
        coolTime += Time.deltaTime;

        if (coolTime >= coolTimeMax)
        {
            randomDir = Random.Range(-2f, 2f);
            rb.AddForce(new Vector2(randomDir, jumpPower), ForceMode2D.Impulse);

            coolTime = 0;
        }
    }
}
