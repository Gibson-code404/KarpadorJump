using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { Playing, Paused, GameOver }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public const string HighScoreKey = "HighScore";

    [Header("Tempo und Schwierigkeit")]
    public float startSpeed = 4f;
    public float maxSpeed = 9f;
    public float secondsToMaxDifficulty = 120f;

    [Header("Welt")]
    public float waterLevel = -0.8f;   // unter diesem y-Wert ist der Fisch im Wasser

    public GameState State { get; private set; } = GameState.Playing;
    public float Difficulty { get; private set; }     // 0 = leicht, 1 = maximal schwer
    public float CurrentSpeed { get; private set; }
    public int Coins { get; private set; }
    public int Score => Mathf.FloorToInt(distance) + Coins * 10;
    public int HighScore { get; private set; }
    public bool NewHighScore { get; private set; }

    public event Action CoinCollected;
    public event Action GameEnded;

    float distance;
    float timePlayed;

    void Awake()
    {
        // Singleton: es darf nur einen GameManager geben
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        Time.timeScale = 1f;   // falls wir aus der Pause neu geladen haben
        HighScore = PlayerPrefs.GetInt(HighScoreKey, 0);
        CurrentSpeed = startSpeed;
    }

    void Update()
    {
        if (GameInput.PausePressed())
        {
            if (State == GameState.Playing) Pause();
            else if (State == GameState.Paused) Resume();
        }

        if (State != GameState.Playing) return;

        timePlayed += Time.deltaTime;
        Difficulty = Mathf.Clamp01(timePlayed / secondsToMaxDifficulty);
        CurrentSpeed = Mathf.Lerp(startSpeed, maxSpeed, Difficulty);
        distance += CurrentSpeed * Time.deltaTime;   // deltaTime: framerate-unabhaengig
    }

    public void Pause()
    {
        if (State != GameState.Playing) return;
        State = GameState.Paused;
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        if (State != GameState.Paused) return;
        State = GameState.Playing;
        Time.timeScale = 1f;
    }

    public void AddCoin(int amount = 1)
    {
        if (State != GameState.Playing) return;
        Coins += amount;
        CoinCollected?.Invoke();
    }

    public void GameOver()
    {
        if (State == GameState.GameOver) return;
        State = GameState.GameOver;

        int finalScore = Score;
        if (finalScore > HighScore)
        {
            HighScore = finalScore;
            NewHighScore = true;
            PlayerPrefs.SetInt(HighScoreKey, HighScore);
            PlayerPrefs.Save();
        }
        GameEnded?.Invoke();
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
