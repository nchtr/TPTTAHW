using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    public GameObject bulletPrefab;
    //public Transform bulletSpawnPoint;
    public float fireRate = 0.2f;

    private float fireCooldown;

    void Update()
    {
        transform.position = new Vector2(transform.position.x + (Input.GetAxis("Horizontal") * moveSpeed), transform.position.y + (Input.GetAxis("Vertical") * moveSpeed));
        HandleMovement();
        HandleShooting();
    }

    void HandleMovement()
    {

        // Keep the player within screen bounds
        Vector3 clampedPosition = Camera.main.WorldToViewportPoint(transform.position);
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, 0.07f, 0.40f);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, 0.09f, 0.9f);
        transform.position = Camera.main.ViewportToWorldPoint(clampedPosition);
    }

    void HandleShooting()
    {
        fireCooldown -= Time.deltaTime + 0.0008f;

        if (Input.GetKey(KeyCode.Space) && fireCooldown <= 0f)
        {
            FireBullet();
            fireCooldown = fireRate;
        }
    }

    void FireBullet()
    {
        Instantiate(bulletPrefab, gameObject.transform.position, Quaternion.identity);
    }

}
