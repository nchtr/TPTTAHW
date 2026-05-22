using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private Coroutine waveRoutine;

    public void StartWaves(int level, int phase)
    {
        StopWaves();
        waveRoutine = StartCoroutine(WaveLoop(level, phase));
    }

    public void StopWaves()
    {
        if (waveRoutine != null) { StopCoroutine(waveRoutine); waveRoutine = null; }
    }

    IEnumerator WaveLoop(int level, int phase)
    {
        while (true)
        {
            yield return StartCoroutine(SpawnWave(level, phase));
            yield return new WaitForSeconds(Mathf.Max(0.5f, 3.8f - level * 0.4f));
        }
    }

    IEnumerator SpawnWave(int level, int phase)
    {
        float hp    = 20f + (level - 1) * 12f + (phase - 1) * 6f;
        float spd   = 1.5f + (level - 1) * 0.25f;
        int   shots = Mathf.Clamp(1 + (level - 1) + (phase - 1), 1, 5);

        switch (Random.Range(0, 4))
        {
            case 0: yield return StartCoroutine(SpawnLine(5,   hp, spd, shots)); break;
            case 1: yield return StartCoroutine(SpawnV(5,      hp, spd, shots)); break;
            case 2: yield return StartCoroutine(SpawnSide(4,   hp, spd, shots)); break;
            case 3: yield return StartCoroutine(SpawnColumn(5, hp, spd, shots)); break;
        }
    }

    IEnumerator SpawnLine(int count, float hp, float spd, int shots)
    {
        for (int i = 0; i < count; i++)
        {
            float x = Mathf.Lerp(-2.8f, 2.8f, (float)i / Mathf.Max(1, count - 1));
            Spawn(new Vector3(x, 7f), hp, spd, shots);
            yield return new WaitForSeconds(0.18f);
        }
    }

    IEnumerator SpawnV(int count, float hp, float spd, int shots)
    {
        for (int i = 0; i < count; i++)
        {
            float x = (i - count / 2) * 1.1f;
            float y = 7f + Mathf.Abs(i - count / 2) * 0.6f;
            Spawn(new Vector3(x, y), hp, spd, shots);
            yield return new WaitForSeconds(0.12f);
        }
    }

    IEnumerator SpawnSide(int count, float hp, float spd, int shots)
    {
        bool left = Random.value > 0.5f;
        float x = left ? -5.5f : 5.5f;
        for (int i = 0; i < count; i++)
        {
            Spawn(new Vector3(x, 1f + i * 1.3f), hp, spd * 0.6f, shots);
            yield return new WaitForSeconds(0.28f);
        }
    }

    IEnumerator SpawnColumn(int count, float hp, float spd, int shots)
    {
        float x = Random.Range(-3f, 3f);
        for (int i = 0; i < count; i++)
        {
            Spawn(new Vector3(x, 7f + i * 0.9f), hp, spd, shots);
            yield return new WaitForSeconds(0.35f);
        }
    }

    EnemyController Spawn(Vector3 pos, float hp, float spd, int shots)
    {
        var e = EnemyController.Create(pos, hp);
        e.moveSpeed   = spd;
        e.bulletCount = shots;
        return e;
    }
}
