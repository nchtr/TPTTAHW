using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float maxHP = 30f;
    public float currentHP;
    public int scoreValue = 100;
    public float moveSpeed = 2f;
    public int bulletCount = 1;
    public float bulletSpeed = 2.8f;
    public float shootInterval = 1.5f;
    public int powerDrops = 2;
    public int pointDrops = 1;

    private SpriteRenderer sr;
    private bool isDying;
    private float shootTimer;
    private static readonly Color BaseColor = new Color(0.85f, 0.2f, 0.2f);

    public static EnemyController Create(Vector3 position, float hp = 30f)
    {
        var go = new GameObject("Enemy");
        go.transform.position = position;
        go.transform.localScale = Vector3.one * 0.6f;
        go.tag = "Enemy";

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = PlaceholderGraphics.CreateSquare(BaseColor, 32);
        sr.sortingOrder = 3;

        go.AddComponent<BoxCollider2D>().size = Vector2.one;

        var e = go.AddComponent<EnemyController>();
        e.maxHP = hp;
        e.currentHP = hp;
        e.sr = sr;
        e.shootTimer = Random.Range(0.5f, 1.5f);
        return e;
    }

    void Update()
    {
        if (isDying) return;

        transform.position += Vector3.down * moveSpeed * Time.deltaTime;
        if (transform.position.y < -7.5f) { Destroy(gameObject); return; }

        shootTimer -= Time.deltaTime;
        if (shootTimer <= 0f) { Shoot(); shootTimer = shootInterval; }
    }

    void Shoot()
    {
        if (PlayerController.Instance == null) return;
        Vector2 toPlayer = (PlayerController.Instance.transform.position - transform.position).normalized;

        if (bulletCount <= 1)
        {
            EnemyBullet.Spawn(transform.position, toPlayer, bulletSpeed, new Color(1f, 0.3f, 0.3f));
            return;
        }

        float spread = 20f * (bulletCount - 1) / 4f;
        for (int i = 0; i < bulletCount; i++)
        {
            float angle = bulletCount > 1 ? -spread + (spread * 2f / (bulletCount - 1)) * i : 0f;
            EnemyBullet.Spawn(transform.position, Rotate(toPlayer, angle), bulletSpeed, new Color(1f, 0.3f, 0.3f));
        }
    }

    static Vector2 Rotate(Vector2 v, float deg)
    {
        float r = deg * Mathf.Deg2Rad;
        return new Vector2(v.x * Mathf.Cos(r) - v.y * Mathf.Sin(r), v.x * Mathf.Sin(r) + v.y * Mathf.Cos(r));
    }

    public void TakeDamage(float dmg)
    {
        if (isDying) return;
        currentHP -= dmg;
        StartCoroutine(HitFlash());
        if (currentHP <= 0f) Die();
    }

    IEnumerator HitFlash()
    {
        if (sr) { sr.color = Color.white; yield return new WaitForSeconds(0.06f); if (sr) sr.color = BaseColor; }
    }

    void Die()
    {
        isDying = true;
        GameManager.Instance.AddScore(scoreValue);
        for (int i = 0; i < powerDrops; i++)
            ItemController.SpawnPowerItem(transform.position + (Vector3)(Random.insideUnitCircle * 0.4f));
        for (int i = 0; i < pointDrops; i++)
            ItemController.SpawnPointItem(transform.position + (Vector3)(Random.insideUnitCircle * 0.4f));
        Destroy(gameObject);
    }
}
