using System;
using Cinemachine;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI")]
    public TMP_Text[] ScoreText;
    public TMP_Text TimerText;
    
    public GameObject PauseMenu;

    [Header("Game Settings")]
    public float gameDuration = 30f; // Total time in seconds

    [Space] [Range(0f, 1f)] public int TimeScale;
    
    public float timer;
    private bool gameEnded = false;
    private int[] _totalScore = new int[4];

    [Header("Finish Line")]
    public GameObject CharacterCam;
    public bool CanEnd;
    public GameObject finishLinePrefab;
    public GameObject WinScreen;
    public float timeToSpawnFinish = 20f; // Spawn finish line when 10 seconds are left
    public bool FinishLineSpawned = false;
    private GameObject finishLineInstance;

    public GameObject mainMenu;
    bool gameStarted;

    public float totalTime;

    public Button leaderboardButton;



    private void Awake()
    {
        Instance = this;

        Time.timeScale = 0;
    }

    public void StartGame()

    {
        leaderboardButton.gameObject.SetActive(false);
        LeaderBoardMenu.instance.userNameInputField.gameObject.SetActive(false);
        LeaderBoardMenu.instance.SetUserName();
        mainMenu.SetActive(false);
        Time.timeScale = 1;
        gameStarted = true;
        ObjectPooler.Instance.SpawnTurnedEnvironmentPatch();
    }
    private void Start()
    {
        leaderboardButton.gameObject.SetActive(true);
        LeaderBoardMenu.instance.userNameInputField.gameObject.SetActive(true);
    }

    public void UpdateInGameTimer()
    {
       
    
    

        if (timer >= gameDuration)
        {
            Debug.Log("time over");
            GameOver(false);
   
        }
        else

        {
            timer += Time.deltaTime;
        }

        TimerText.text = FormatTime((int)timer);
    }
    private void Update()
    {
        if(!gameStarted) { return; }

        if (gameEnded) return;

        // Countdown Timer

        UpdateInGameTimer();

    
        // ⏳ Spawn Finish Line at a Specific Time
        if (!FinishLineSpawned && timer <= (gameDuration - timeToSpawnFinish) && CanEnd)
        {
            FinishLineSpawned = true; 
            SpawnFinishLine();
        }
    
        // ❌ Lose condition: Time runs out
       
    }
    public static string FormatTime(int totalSeconds)
    {
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        return $"{minutes:D2}:{seconds:D2}";
    }
    public void Collected(int amount,int id)
    {
        _totalScore[id] += amount;
        ScoreText[id].text = _totalScore[id].ToString(); // Update UI Score

        // Play Score Pop Animation
        ScoreText[id].transform.parent.transform.DOKill();
        ScoreText[id].transform.parent.transform.localScale = Vector3.one;
        ScoreText[id].transform.parent.transform.DOScale(1.3f, 0.2f).SetLoops(2, LoopType.Yoyo);
    }

    private void SpawnFinishLine()
    {
        finishLineInstance = Instantiate(finishLinePrefab);
        finishLineInstance.transform.SetParent(ObjectPooler.Instance.ActivedTuredPatch.transform);
        if (ObjectPooler.Instance.ActivedTuredPatch ==
            ObjectPooler.Instance.TurnedPatchParent.GetComponent<TurnedPatchesManager>().LeftTurn)
        {
            finishLineInstance.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        finishLineInstance.transform.position=ObjectPooler.Instance.ActivedTuredPatch.GetComponent<TurnedPatchEnv>().SchoolPoint.position;
        Time.timeScale = TimeScale;
        Debug.Log("🚩 Finish line spawned!");
    }

    [ContextMenu("Play")]
    void Play()
    {
        Time.timeScale = 1;
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
    public void WinGame()
    {
        totalTime = timer;
        LeaderBoardMenu.instance.SubmitScore((long)totalTime);
    

        WinScreen.SetActive(true);
        Time.timeScale = 0f;
    }
    // private void OnGUI()
    // {
    //     if (GUI.Button(new Rect(10, 10, 150, 30), "Play"))
    //     {
    //         Play();
    //     }
    //     
    //     if (GUI.Button(new Rect(10, 50, 150, 30), "Restart"))
    //     {
    //         RestartDebug();
    //     }
    // }
    //public IEnumerator getLeaderboardData()
    //{
    //    yield return new WaitForSeconds(1);
    //    LeaderBoardMenu.instance.GetLeaderboardTop();
    //}
    public void RestartDebug()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }
}
