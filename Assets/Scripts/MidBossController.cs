using System.Collections;
using UnityEngine;

public class MidBossController : MonoBehaviour
{
    public float maxHP = 600f;
    public float currentHP;
    public int scoreValue = 5000;

    private SpriteRenderer sr;
    private bool isDying;
    private int attackPhase;
    private static readonly Color BaseColor = new Color(0.7f, 0.3f, 0.85f);

    public static MidBossController Create(Vector3 position)
    {
        var go = new GameObject("MidBoss");
        go.transform.position = position;
        go.transform.localScale = Vector3.one * 1.1f;
        go.tag = "MidBoss";

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = PlaceholderGraphics.CreateSquare(BaseColor, 48);
        sr.sortingOrder = 3;

        go.AddComponent<BoxCollider2D>().size = Vector2.one;

        var mb = go.AddComponent<MidBossController>();
        mb.sr = sr;
        mb.currentHP = mb.maxHP;
        return mb;
    }

    void Start() => StartCoroutine(EnterThenAttack());

    IEnumerator EnterThenAttack()
    {
        var target = new Vector3(0f, 2.5f, 0f);
        while (Vector3.Distance(transform.position, target) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, 4f * Time.deltaTime);
            yield return null;
        }
        transform.position = target;

        while (!isDying)
        {
            yield return StartCoroutine(attackPhase % 2 == 0 ? AttackCircle() : AttackAimed());
            attackPhase++;
            yield return new WaitForSeconds(0.8f);
        }
    }

    IEnumerator AttackCircle()
    {
        for (int wave = 0; wave < 5 && !isDying; wave++)
        {
            int count = 18;
            float offset = wave * (360f / count * 0.5f);
            for (int i = 0; i < count; i++)
            {
                float angle = offset + (360f / count) * i;
                var dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
                EnemyBullet.Spawn(transform.position, dir, 2.2f, new Color(0.9f, 0.2f, 0.9f), 0.32f);
            }
            yield return new WaitForSeconds(0.45f);
        }
    }

    IEnumerator AttackAimed()
    {
        for (int burst = 0; burst < 6 && !isDying; burst++)
        {
            if (PlayerController.Instance != null)
            {
                Vector2 toPlayer = (PlayerController.Instance.transform.position - transform.position).normalized;
                for (int i = -2; i <= 2; i++)
                    EnemyBullet.Spawn(transform.position, Rotate(toPlayer, i * 14f), 3.2f, new Color(0.9f, 0.4f, 1f), 0.32f);
            }
            yield return new WaitForSeconds(0.42f);
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
        for (int i = 0; i < 5; i++)
            ItemController.SpawnBigPowerItem(transform.position + (Vector3)(Random.insideUnitCircle * 1.2f));
        for (int i = 0; i < 3; i++)
            ItemController.SpawnPointItem(transform.position + (Vector3)(Random.insideUnitCircle * 1.2f));
        LevelManager.Instance?.OnMidBossDefeated();
        Destroy(gameObject);
    }
}
