using UnityEngine;

public class SatelliteController : MonoBehaviour
{
    private Transform player;
    private float angle;
    private const float OrbitRadius = 0.9f;
    private const float OrbitSpeed = 120f;
    private const float FireRate = 0.18f;
    private float fireCooldown;

    public static SatelliteController Create(Transform playerTransform, float startAngle)
    {
        var go = new GameObject("Satellite");
        go.transform.localScale = Vector3.one * 0.35f;

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = PlaceholderGraphics.CreateCircle(new Color(1f, 0.85f, 0f), 20);
        sr.sortingOrder = 4;

        var sat = go.AddComponent<SatelliteController>();
        sat.player = playerTransform;
        sat.angle = startAngle;
        return sat;
    }

    void Update()
    {
        if (player == null) { Destroy(gameObject); return; }

        angle += OrbitSpeed * Time.deltaTime;
        float rad = angle * Mathf.Deg2Rad;
        transform.position = player.position + new Vector3(Mathf.Cos(rad), Mathf.Sin(rad)) * OrbitRadius;

        var pc = PlayerController.Instance;
        if (pc == null || pc.IsDead) return;

        fireCooldown -= Time.deltaTime;
        if ((Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.Space)) && fireCooldown <= 0f)
        {
            float power = GameManager.Instance != null ? GameManager.Instance.Power : 1f;
            PlayerBullet.Spawn(transform.position, Vector2.up, 11f, 6f * (power / 2f), 0.3f);
            fireCooldown = FireRate;
        }
    }
}
