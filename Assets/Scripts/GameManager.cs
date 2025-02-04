using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI")]
    public TMP_Text ScoreText;
    public TMP_Text TimerText;
    
    public GameObject PauseMenu;

    [Header("Game Settings")]
    public float gameDuration = 30f; // Total time in seconds
    private float timer;
    private bool gameEnded = false;
    private int _totalScore = 0;

    [Header("Finish Line")]
    public GameObject finishLinePrefab;
    public float timeToSpawnFinish = 20f; // Spawn finish line when 10 seconds are left
    private bool finishSpawned = false;
    private GameObject finishLineInstance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        timer = gameDuration;
    }

    private void Update()
    {
        if (gameEnded) return;
    
        // Countdown Timer
        timer -= Time.deltaTime;
        TimerText.text = FormatTime( (int)timer); // Display as integer
    
        // ⏳ Spawn Finish Line at a Specific Time
        if (!finishSpawned && timer <= (gameDuration - timeToSpawnFinish))
        {
            finishSpawned = true; 
            SpawnFinishLine();
        }
    
        // ❌ Lose condition: Time runs out
        if (timer <= 0)
        {
            GameOver(false);
        }
    }
    public static string FormatTime(int totalSeconds)
    {
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        return $"{minutes:D2}:{seconds:D2}";
    }
    public void Collected(int amount)
    {
        _totalScore += amount;
        ScoreText.text = _totalScore.ToString(); // Update UI Score

        // Play Score Pop Animation
        ScoreText.transform.DOKill();
        ScoreText.transform.localScale = Vector3.one;
        ScoreText.transform.DOScale(1.3f, 0.2f).SetLoops(2, LoopType.Yoyo);
    }

    private void SpawnFinishLine()
    {
        // Spawn finish line at a fixed distance in front of the player
        Vector3 spawnPosition = new Vector3(0, 0, 50f); // Adjust distance as needed
        finishLineInstance = Instantiate(finishLinePrefab, spawnPosition, Quaternion.identity);
        Debug.Log("🚩 Finish line spawned!");
    }

    public void PlayerReachedFinish()
    {
        if (!gameEnded) GameOver(true);
    }

    private void GameOver(bool won)
    {
        gameEnded = true;

        if (won)
        {
            Debug.Log("🎉 YOU WIN!");
        }
        else
        {
            Time.timeScale = 0f;
            PauseMenu.SetActive(true);
            Debug.Log("💀 YOU LOSE! Time's up!");
        }
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
