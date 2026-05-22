using System.Collections;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public int currentLevel = 1;
    public int totalLevels = 3;

    private bool midBossDefeated;
    private bool bossDefeated;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start() => StartCoroutine(RunLevel(currentLevel));

    IEnumerator RunLevel(int level)
    {
        midBossDefeated = false;
        bossDefeated = false;

        UIManager.Instance?.ShowMessage($"Stage {level}", 2.5f);
        yield return new WaitForSeconds(2.5f);

        var spawner = FindFirstObjectByType<EnemySpawner>();

        // Phase 1 waves
        float p1 = Mathf.Lerp(35f, 22f, (level - 1f) / Mathf.Max(1, totalLevels - 1));
        spawner?.StartWaves(level, 1);
        yield return new WaitForSeconds(p1);
        spawner?.StopWaves();
        yield return new WaitForSeconds(1.5f);

        // Mid-boss
        UIManager.Instance?.ShowMessage("Mid-Boss!", 2f);
        MidBossController.Create(new Vector3(0f, 8f, 0f));
        yield return new WaitUntil(() => midBossDefeated);
        yield return new WaitForSeconds(2.5f);

        // Phase 2 waves
        float p2 = Mathf.Lerp(22f, 14f, (level - 1f) / Mathf.Max(1, totalLevels - 1));
        spawner?.StartWaves(level, 2);
        yield return new WaitForSeconds(p2);
        spawner?.StopWaves();
        yield return new WaitForSeconds(1.5f);

        // Boss
        UIManager.Instance?.ShowMessage($"Stage {level} Boss!", 2.5f);
        BossController.Create(new Vector3(0f, 8f, 0f));
        yield return new WaitUntil(() => bossDefeated);
        yield return new WaitForSeconds(3f);

        // Stage clear
        UIManager.Instance?.ShowMessage($"Stage {level} Clear!", 3f);
        GameManager.Instance.AddScore(level * 12000);
        yield return new WaitForSeconds(3f);

        currentLevel++;
        if (currentLevel <= totalLevels)
            StartCoroutine(RunLevel(currentLevel));
        else
        {
            UIManager.Instance?.ShowMessage("All Stages Clear!\nCongratulations!", 8f);
            GameManager.Instance.AddScore(100000);
        }
    }

    public void OnMidBossDefeated() => midBossDefeated = true;
    public void OnBossDefeated()    => bossDefeated    = true;
}
