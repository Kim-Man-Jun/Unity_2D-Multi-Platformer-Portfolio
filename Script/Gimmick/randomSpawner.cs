using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class randomSpawner : MonoBehaviour
{
    BoxCollider2D bc;

    public GameObject spikedBall;

    private void Awake()
    {
        bc = GetComponent<BoxCollider2D>();

        StartCoroutine(respawn());
    }

    IEnumerator respawn()
    {
        yield return new WaitForSeconds(0.8f);

        spawn();

        StartCoroutine(respawn());
    }

    public void spawn()
    {
        float width = bc.bounds.size.x;
        float height = bc.bounds.size.y;

        float x = UnityEngine.Random.Range(-width / 2, width / 2);
        float y = UnityEngine.Random.Range(-height / 2, height / 2);

        Vector2 spawnVec2 = new Vector2(x, y);

        Instantiate(spikedBall, new Vector2(transform.position.x + x, transform.position.y + y), Quaternion.identity);
    }
}
