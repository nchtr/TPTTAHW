using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Movement : MonoBehaviour
{
    public Text ScoreText;
    [SerializeField] private float Speed;
    public GameObject Bullet;
    public int Score;
    void Start()
    {
        StartCoroutine(Shooting());
    }

   
    void Update()
    {
        ScoreText.text = "Score: " + Score.ToString();
        transform.position = new Vector2(transform.position.x + Input.GetAxis("Horizontal") * Speed, transform.position.y);
       
    }

    IEnumerator Shooting()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            Instantiate(Bullet, gameObject.transform.position, Quaternion.identity);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (CompareTag("Enemy"))
        {
            SceneManager.LoadScene(0);
        }
    }
}
