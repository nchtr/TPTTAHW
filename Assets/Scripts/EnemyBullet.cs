using System.Collections;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public Vector2 velocity;

    private bool hasGrazed;
    private SpriteRenderer sr;
    private Color originalColor;

    // Hitbox: distance from bullet center to player center that counts as a hit
    public const float HitRadius = 0.12f;
    // Graze zone: within this range but outside HitRadius
    public const float GrazeRadius = 0.55f;

    public static EnemyBullet Spawn(Vector3 position, Vector2 direction, float speed, Color color, float scale = 0.3f)
    {
        var go = new GameObject("EnemyBullet");
        go.transform.position = position;
        go.transform.localScale = Vector3.one * scale;
        go.tag = "EnemyBullet";

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = PlaceholderGraphics.CreateCircle(color, 16);
        sr.sortingOrder = 3;

        go.AddComponent<CircleCollider2D>().radius = 0.4f;

        var eb = go.AddComponent<EnemyBullet>();
        eb.velocity = direction.normalized * speed;
        eb.sr = sr;
        eb.originalColor = color;
        return eb;
    }

    void Update()
    {
        transform.position += (Vector3)velocity * Time.deltaTime;

        if (Mathf.Abs(transform.position.x) > 7f || transform.position.y > 9f || transform.position.y < -9f)
        {
            Destroy(gameObject);
            return;
        }

        var player = PlayerController.Instance;
        if (player == null || player.IsDead || GameManager.Instance.IsBombing) return;

        float dist = Vector2.Distance(transform.position, player.transform.position);

        if (!hasGrazed && dist < GrazeRadius && dist > HitRadius)
        {
            hasGrazed = true;
            GameManager.Instance.AddGraze();
            StartCoroutine(GrazeFlash());
        }

        if (dist < HitRadius)
        {
            player.OnHit();
            Destroy(gameObject);
        }
    }

    IEnumerator GrazeFlash()
    {
        if (sr) sr.color = Color.white;
        yield return new WaitForSeconds(0.07f);
        if (sr) sr.color = originalColor;
    }
}
