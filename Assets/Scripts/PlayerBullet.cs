using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public float damage = 10f;
    public Vector2 direction = Vector2.up;
    public float speed = 12f;

    public static PlayerBullet Spawn(Vector3 position, Vector2 dir, float spd, float dmg, float scale = 0.38f)
    {
        var go = new GameObject("PlayerBullet");
        go.transform.position = position;
        go.transform.localScale = Vector3.one * scale;
        go.tag = "PlayerBullet";

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = PlaceholderGraphics.CreateCircle(new Color(0.4f, 1f, 1f), 12);
        sr.sortingOrder = 4;

        go.AddComponent<CircleCollider2D>().radius = 0.5f;

        var pb = go.AddComponent<PlayerBullet>();
        pb.damage = dmg;
        pb.direction = dir.normalized;
        pb.speed = spd;
        return pb;
    }

    void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);

        if (transform.position.y > 8f || transform.position.y < -8f || Mathf.Abs(transform.position.x) > 7f)
        {
            Destroy(gameObject);
            return;
        }

        var hits = Physics2D.OverlapCircleAll(transform.position, 0.12f);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                hit.GetComponentInParent<EnemyController>()?.TakeDamage(damage);
                Destroy(gameObject); return;
            }
            if (hit.CompareTag("Boss"))
            {
                hit.GetComponentInParent<BossController>()?.TakeDamage(damage);
                Destroy(gameObject); return;
            }
            if (hit.CompareTag("MidBoss"))
            {
                hit.GetComponentInParent<MidBossController>()?.TakeDamage(damage);
                Destroy(gameObject); return;
            }
        }
    }
}
