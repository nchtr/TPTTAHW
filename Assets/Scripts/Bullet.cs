using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    int LifeTime = 10;
    void Awake()
    {
        StartCoroutine(Life());
    }


    void Update()
    {
        transform.position = new Vector2(transform.position.x, transform.position.y + 0.05f);
    }

    IEnumerator Life()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            LifeTime--;
            if (LifeTime == 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
