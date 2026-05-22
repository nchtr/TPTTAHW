using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    public float normalSpeed = 5f;
    public float focusSpeed = 2.2f;
    public float fireRate = 0.1f;

    // Play field bounds (world space)
    public float minX = -3.5f, maxX = 3.5f, minY = -4.8f, maxY = 4.8f;

    public bool IsDead { get; private set; }
    public bool IsFocused { get; private set; }

    private float fireCooldown;
    private SpriteRenderer bodyRenderer;
    private SpriteRenderer hitboxRenderer;
    private bool isBombing;
    private float bombTimer;
    private const float BombDuration = 3f;
    private const float BombDPS = 60f;

    private readonly List<SatelliteController> satellites = new List<SatelliteController>();
    private float lastPowerTier = -1f;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        BuildVisuals();
        UpdateSatellites();
    }

    void BuildVisuals()
    {
        bodyRenderer = gameObject.AddComponent<SpriteRenderer>();
        bodyRenderer.sprite = PlaceholderGraphics.CreateSquare(Color.cyan, 32);
        bodyRenderer.sortingOrder = 5;
        transform.localScale = Vector3.one * 0.7f;

        // Hitbox child — only the red dot, no separate collider needed
        var hbGO = new GameObject("HitboxDot");
        hbGO.transform.SetParent(transform);
        hbGO.transform.localPosition = Vector3.zero;
        hbGO.transform.localScale = Vector3.one * 0.22f;

        hitboxRenderer = hbGO.AddComponent<SpriteRenderer>();
        hitboxRenderer.sprite = PlaceholderGraphics.CreateCircle(Color.red, 16);
        hitboxRenderer.sortingOrder = 6;
        hitboxRenderer.enabled = false;
    }

    void Update()
    {
        if (IsDead || GameManager.Instance.IsGameOver) return;

        HandleMovement();
        HandleShooting();
        HandleBomb();

        int tier = Mathf.FloorToInt(GameManager.Instance.Power);
        if (Mathf.FloorToInt(lastPowerTier) != tier)
        {
            lastPowerTier = tier;
            UpdateSatellites();
        }
    }

    void HandleMovement()
    {
        IsFocused = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        hitboxRenderer.enabled = IsFocused;

        float speed = IsFocused ? focusSpeed : normalSpeed;
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        if (h != 0 && v != 0) { h *= 0.7071f; v *= 0.7071f; }

        Vector3 pos = transform.position + new Vector3(h, v, 0f) * speed * Time.deltaTime;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        transform.position = pos;
    }

    void HandleShooting()
    {
        fireCooldown -= Time.deltaTime;
        if ((Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.Space)) && fireCooldown <= 0f)
        {
            FireShots();
            fireCooldown = fireRate;
        }
    }

    void FireShots()
    {
        float power = GameManager.Instance.Power;
        float dmg = 10f * (power / 2f);
        Vector3 origin = transform.position + Vector3.up * 0.5f;

        if (power < 2.0f)
        {
            PlayerBullet.Spawn(origin, Vector2.up, 12f, dmg);
        }
        else if (power < 3.0f)
        {
            PlayerBullet.Spawn(origin + Vector3.left * 0.22f,  Vector2.up, 12f, dmg * 0.6f);
            PlayerBullet.Spawn(origin + Vector3.right * 0.22f, Vector2.up, 12f, dmg * 0.6f);
        }
        else if (power < 4.0f)
        {
            PlayerBullet.Spawn(origin + Vector3.left * 0.38f,  Vector2.up, 12f, dmg * 0.5f);
            PlayerBullet.Spawn(origin,                         Vector2.up, 12f, dmg * 0.7f);
            PlayerBullet.Spawn(origin + Vector3.right * 0.38f, Vector2.up, 12f, dmg * 0.5f);
        }
        else
        {
            PlayerBullet.Spawn(origin + Vector3.left  * 0.42f, new Vector2(-0.15f, 1f).normalized, 12f, dmg * 0.5f);
            PlayerBullet.Spawn(origin + Vector3.left  * 0.16f, Vector2.up, 12f, dmg * 0.6f);
            PlayerBullet.Spawn(origin + Vector3.right * 0.16f, Vector2.up, 12f, dmg * 0.6f);
            PlayerBullet.Spawn(origin + Vector3.right * 0.42f, new Vector2( 0.15f, 1f).normalized, 12f, dmg * 0.5f);
        }
    }

    void HandleBomb()
    {
        if (Input.GetKeyDown(KeyCode.X) && !isBombing)
            TryActivateBomb();

        if (!isBombing) return;

        bombTimer -= Time.deltaTime;

        // Continuously clear enemy bullets
        foreach (var eb in FindObjectsOfType<EnemyBullet>())
            Destroy(eb.gameObject);

        // Damage enemies and bosses
        float dmg = BombDPS * Time.deltaTime;
        foreach (var e in FindObjectsOfType<EnemyController>()) e.TakeDamage(dmg);
        FindFirstObjectByType<BossController>()?.TakeDamage(dmg);
        FindFirstObjectByType<MidBossController>()?.TakeDamage(dmg);

        if (bombTimer <= 0f) EndBomb();
    }

    void TryActivateBomb()
    {
        if (!GameManager.Instance.TryUseBomb()) return;
        isBombing = true;
        bombTimer = BombDuration;
        GameManager.Instance.SetBombing(true);

        foreach (var eb in FindObjectsOfType<EnemyBullet>())
            Destroy(eb.gameObject);

        StartCoroutine(BombFlash());
    }

    IEnumerator BombFlash()
    {
        float t = 0f;
        while (t < BombDuration && bodyRenderer != null)
        {
            bodyRenderer.color = Color.Lerp(Color.cyan, Color.white, Mathf.PingPong(t * 5f, 1f));
            t += Time.deltaTime;
            yield return null;
        }
        if (bodyRenderer != null) bodyRenderer.color = Color.cyan;
    }

    void EndBomb()
    {
        isBombing = false;
        GameManager.Instance.SetBombing(false);
    }

    void UpdateSatellites()
    {
        foreach (var s in satellites) if (s != null) Destroy(s.gameObject);
        satellites.Clear();

        float power = GameManager.Instance != null ? GameManager.Instance.Power : 1f;
        int count = power >= 2.0f ? (power >= 3.0f ? 4 : 2) : 0;

        for (int i = 0; i < count; i++)
            satellites.Add(SatelliteController.Create(transform, (360f / count) * i));
    }

    public void OnHit()
    {
        if (IsDead || isBombing || GameManager.Instance.IsBombing) return;
        IsDead = true;
        GameManager.Instance.LoseLife();
        if (GameManager.Instance.Lives > 0)
            StartCoroutine(Respawn());
    }

    IEnumerator Respawn()
    {
        if (bodyRenderer) bodyRenderer.enabled = false;
        yield return new WaitForSeconds(2f);
        transform.position = new Vector3(0f, -3f, 0f);
        if (bodyRenderer) bodyRenderer.enabled = true;
        IsDead = false;
        UpdateSatellites();
    }
}
