using System;
using Cinemachine;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI")] public TMP_Text[] ScoreText;
    public TMP_Text TimerText;

    public GameObject PauseMenu;

    [Header("Game Settings")] public float gameDuration = 30f; // Total time in seconds

    [Space] [Range(0f, 1f)] public int TimeScale;

    public float timer;
    public bool gameEnded = false;
    int[] _totalScore = new int[4];

    [Header("Finish Line")] public GameObject CharacterCam;
    public bool CanEnd;
    public GameObject finishLinePrefab;
    public GameObject WinScreen;
    public float timeToSpawnFinish = 20f; // Spawn finish line when 10 seconds are left
    public bool FinishLineSpawned = false;
    private GameObject finishLineInstance;

    [Header("Progress-Bar")] public Image progressBar; // Your UI Slider
    public TextMeshProUGUI progressText; // Your UI Slider
    public float patchLength = 50f; // Length of ONE patch
    public int totalPatches = 10; // Total number of patches
    private float totalDistance;
    private float travelledDistance;
    private float elapsedTime;

    public TextMeshProUGUI Finished_TimeText;

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
        Audiomanager.instance.audi_bg.Pause();
        leaderboardButton.gameObject.SetActive(false);
        LeaderBoardMenu.instance.userNameInputField.gameObject.SetActive(false);
        LeaderBoardMenu.instance.SetUserName();
        mainMenu.SetActive(false);
        Time.timeScale = 1;
        gameStarted = true;
        ObjectPooler.Instance.SpawnTurnedEnvironmentPatch();

        PlayerController.instance.StartCountdown();
    }
    private void Start()
    {
        totalDistance = patchLength * totalPatches;
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

    public float displayedProgress = 0f; // Smooth displayed progress

    private void FixedUpdate()
    {
        if (gameEnded)
            return;

        UpdateInGameTimer();

        if (!FinishLineSpawned && timer >= (gameDuration - timeToSpawnFinish) && CanEnd && !gameEnded)
        {
            FinishLineSpawned = true;
            SpawnFinishLine();
        }

        if (PlayerController.instance.isHurt || gameEnded)
            return;

        elapsedTime += Time.fixedDeltaTime;
        travelledDistance = 25f * elapsedTime;

        float progress = Mathf.Clamp01(travelledDistance / totalDistance);
        progressBar.fillAmount = progress;

        int progressPercent = Mathf.RoundToInt(progress * 100f);
        progressText.text = progressPercent + "%";
    }

    public static string FormatTime(int totalSeconds)
    {
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        return $"{minutes:D2}:{seconds:D2}";
    }

    public void Collected(int amount, int id)
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

        finishLineInstance.transform.position = ObjectPooler.Instance.ActivedTuredPatch.GetComponent<TurnedPatchEnv>()
            .SchoolPoint.position;
        Time.timeScale = TimeScale;
        Debug.Log("🚩 Finish line spawned on time : " + timer);
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

    public void LevelFinished()
    {
        gameEnded = true;
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
        Finished_TimeText.text = FormatTime((int)timer);
        WinScreen.SetActive(true);
        Audiomanager.instance.audi_bg.mute = true;
        Audiomanager.instance.Player_Source.mute = true;
        
        Audiomanager.instance.Play_Win();
        Time.timeScale = 0f;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}