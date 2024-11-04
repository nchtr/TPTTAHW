using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject Enemy;
    
    void Start()
    {
        StartCoroutine(HawkTuah());
    }

    IEnumerator HawkTuah()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            Instantiate(Enemy, new Vector2(transform.position.x + Random.RandomRange(-2f, 2f), transform.position.y + Random.RandomRange(-2f, 2f)), Quaternion.identity);
        }
    }

}
