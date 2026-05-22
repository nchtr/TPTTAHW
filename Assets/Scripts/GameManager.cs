using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public float Score { get; private set; }
    public int Lives { get; private set; } = 3;
    public int Bombs { get; private set; } = 3;
    public float Power { get; private set; } = 1.0f;
    public int Graze { get; private set; }
    public int HighScore { get; private set; }

    public const float MaxPower = 4.0f;
    public const int MaxBombs = 8;
    public const float GrazeScore = 10f;
    public const float PointItemScore = 100f;
    public const float SmallPowerAmount = 0.05f;
    public const float LargePowerAmount = 1.0f;

    public bool IsGameOver { get; private set; }
    public bool IsBombing { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddScore(float amount)
    {
        Score += amount;
        if ((int)Score > HighScore) HighScore = (int)Score;
    }

    public void AddGraze()
    {
        Graze++;
        AddScore(GrazeScore);
    }

    public void AddPower(float amount)
    {
        Power = Mathf.Clamp(Power + amount, 1.0f, MaxPower);
    }

    public bool TryUseBomb()
    {
        if (Bombs <= 0 || IsBombing) return false;
        Bombs--;
        return true;
    }

    public void SetBombing(bool value) { IsBombing = value; }

    public void LoseLife()
    {
        Lives--;
        Power = Mathf.Max(1.0f, Power - 1.0f);
        if (Bombs < 3) Bombs = 3;
        if (Lives <= 0)
        {
            IsGameOver = true;
            Time.timeScale = 0f;
        }
    }

    public void AddBomb() { if (Bombs < MaxBombs) Bombs++; }

    public void RestartGame()
    {
        Score = 0; Lives = 3; Bombs = 3; Power = 1.0f; Graze = 0;
        IsGameOver = false; IsBombing = false;
        Time.timeScale = 1f;
    }
}
