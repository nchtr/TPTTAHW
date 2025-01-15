using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnRate = 1f;

    private float spawnCooldown;

    void Update()
    {
        spawnCooldown -= Time.deltaTime;

        if (spawnCooldown <= 0f)
        {
            SpawnEnemy();
            spawnCooldown = spawnRate;
        }
    }

    void SpawnEnemy()
    {
        float spawnX = Random.Range(-8f, 8f);
        Vector2 spawnPosition = new Vector2(spawnX, 6f);
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }
}