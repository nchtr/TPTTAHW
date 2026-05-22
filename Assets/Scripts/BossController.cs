using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public int scoreValue = 25000;

    private readonly float[] phaseMaxHPs = { 900f, 700f, 1100f };
    private float currentHP;
    private int currentPhase;
    private float phaseTimer;
    private SpriteRenderer sr;
    private bool isDying;
    private Coroutine attackRoutine;
    private static readonly Color BaseColor = new Color(0.5f, 0.1f, 0.65f);

    public static BossController Create(Vector3 position)
    {
        var go = new GameObject("Boss");
        go.transform.position = position;
        go.transform.localScale = Vector3.one * 1.6f;
        go.tag = "Boss";

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = PlaceholderGraphics.CreateSquare(BaseColor, 64);
        sr.sortingOrder = 3;

        go.AddComponent<BoxCollider2D>().size = Vector2.one;

        var boss = go.AddComponent<BossController>();
        boss.sr = sr;
        return boss;
    }

    void Start()
    {
        UIManager.Instance?.ShowBossHP(true);
        StartCoroutine(Enter());
    }

    void Update()
    {
        if (isDying) return;
        phaseTimer -= Time.deltaTime;
        UIManager.Instance?.UpdateBossHP(currentHP / phaseMaxHPs[currentPhase]);

        if (phaseTimer <= 0f)
        {
            foreach (var eb in FindObjectsOfType<EnemyBullet>()) Destroy(eb.gameObject);
            NextPhase();
        }
    }

    IEnumerator Enter()
    {
        var target = new Vector3(0f, 3f, 0f);
        while (Vector3.Distance(transform.position, target) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, 3.5f * Time.deltaTime);
            yield return null;
        }
        transform.position = target;
        BeginPhase(0);
    }

    void BeginPhase(int phase)
    {
        currentPhase = Mathf.Clamp(phase, 0, phaseMaxHPs.Length - 1);
        currentHP = phaseMaxHPs[currentPhase];
        phaseTimer = 55f;

        if (attackRoutine != null) StopCoroutine(attackRoutine);
        attackRoutine = StartCoroutine(AttackLoop(currentPhase));
    }

    void NextPhase()
    {
        if (currentPhase + 1 >= phaseMaxHPs.Length) { Die(); return; }
        BeginPhase(currentPhase + 1);
    }

    IEnumerator AttackLoop(int phase)
    {
        while (!isDying)
        {
            switch (phase)
            {
                case 0:
                    yield return StartCoroutine(PatternAimedFan(5, 25f, 3.0f));
                    yield return new WaitForSeconds(1f);
                    yield return StartCoroutine(PatternRings(3, 16));
                    yield return new WaitForSeconds(1f);
                    break;
                case 1:
                    yield return StartCoroutine(PatternSpiral(80, 0.05f, 2.8f));
                    yield return new WaitForSeconds(1.2f);
                    yield return StartCoroutine(PatternAimedFan(7, 28f, 3.5f));
                    yield return new WaitForSeconds(0.8f);
                    break;
                case 2:
                    yield return StartCoroutine(PatternSpiral(60, 0.04f, 3.2f));
                    yield return StartCoroutine(PatternRings(4, 20));
                    yield return StartCoroutine(PatternAimedFan(9, 32f, 4.0f));
                    break;
            }
        }
    }

    IEnumerator PatternAimedFan(int count, float spread, float spd)
    {
        for (int burst = 0; burst < 10 && !isDying; burst++)
        {
            if (PlayerController.Instance != null)
            {
                Vector2 toPlayer = (PlayerController.Instance.transform.position - transform.position).normalized;
                for (int i = 0; i < count; i++)
                {
                    float angle = count > 1 ? -spread + (spread * 2f / (count - 1)) * i : 0f;
                    EnemyBullet.Spawn(transform.position, Rotate(toPlayer, angle), spd, new Color(0.6f, 0f, 1f), 0.3f);
                }
            }
            yield return new WaitForSeconds(0.38f);
        }
    }

    IEnumerator PatternRings(int rings, int countPerRing)
    {
        for (int r = 0; r < rings && !isDying; r++)
        {
            float offset = r * (360f / countPerRing * 0.5f);
            for (int i = 0; i < countPerRing; i++)
            {
                float angle = offset + (360f / countPerRing) * i;
                var dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
                EnemyBullet.Spawn(transform.position, dir, 2.3f + currentPhase * 0.2f, new Color(0.8f, 0f, 0.5f), 0.3f);
            }
            yield return new WaitForSeconds(0.48f);
        }
    }

    IEnumerator PatternSpiral(int shots, float interval, float spd)
    {
        float angle = 0f;
        for (int i = 0; i < shots && !isDying; i++)
        {
            angle += 13f;
            var d1 = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            EnemyBullet.Spawn(transform.position, d1,  spd, new Color(1f, 0.2f, 0.7f), 0.28f);
            EnemyBullet.Spawn(transform.position, -d1, spd, new Color(1f, 0.2f, 0.7f), 0.28f);
            yield return new WaitForSeconds(interval);
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
        if (currentHP <= 0f) NextPhase();
    }

    IEnumerator HitFlash()
    {
        if (sr) { sr.color = Color.white; yield return new WaitForSeconds(0.05f); if (sr) sr.color = BaseColor; }
    }

    void Die()
    {
        isDying = true;
        if (attackRoutine != null) StopCoroutine(attackRoutine);
        GameManager.Instance.AddScore(scoreValue);
        for (int i = 0; i < 8; i++)
            ItemController.SpawnBigPowerItem(transform.position + (Vector3)(Random.insideUnitCircle * 2f));
        for (int i = 0; i < 5; i++)
            ItemController.SpawnPointItem(transform.position + (Vector3)(Random.insideUnitCircle * 2f));
        UIManager.Instance?.ShowBossHP(false);
        LevelManager.Instance?.OnBossDefeated();
        Destroy(gameObject);
    }
}
