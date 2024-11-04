using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BotMovement : MonoBehaviour
{
    Movement movement;
    private void Start()
    {
        StartCoroutine(Movement());
    }
    private void Awake()
    {
        movement = GameObject.FindWithTag("Player").GetComponent<Movement>();
    }


    IEnumerator Movement()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            transform.position = new Vector2(transform.position.x + Random.RandomRange(-1f, 1f), transform.position.y);

        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            movement.Score++;
            Destroy(gameObject);
        }
    }
}
