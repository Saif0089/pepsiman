using System;
using System.Collections;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Leaderboards;
using Unity.Services.Core;
using TMPro;

public class LeaderBoardMenu : MonoBehaviour
{
    public static LeaderBoardMenu instance;
    public string id;
    public GameObject container;
    public TMP_InputField userNameInputField;
    public GameObject leaderBoardItemPrefab;
    public string userName;
    Coroutine refreshCoroutine;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    private async void Start()
    {
        var options = new InitializationOptions();

        string playerId = string.Empty;

        if (!PlayerPrefs.HasKey(nameof(playerId)))
        {
            playerId = System.Guid.NewGuid().ToString();
            playerId = playerId.Replace('-', ' ').Trim();
            if (playerId.Length > 5)
            {
                for (int i = 6; i < playerId.Length; i++)
                {
                    playerId = playerId.Remove(i);
                }
            }

            PlayerPrefs.SetString(nameof(playerId), playerId);
        }
        else
        {
            playerId = PlayerPrefs.GetString(nameof(playerId));
            userNameInputField.gameObject.SetActive(false);
        }

        options.SetProfile(playerId);
        await UnityServices.InitializeAsync(options);
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        StartCoroutine(RefreshLeaderboardLoop());
        
        GetLeaderboardTop();
    }
    
    IEnumerator RefreshLeaderboardLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            GetLeaderboardTop();
        }
    }

    public async void SetUserName()
    {
        await AuthenticationService.Instance.UpdatePlayerNameAsync(userName);
        Debug.Log("New Player ID: " + AuthenticationService.Instance.PlayerId);
        Debug.Log("New Player name: " + AuthenticationService.Instance.PlayerName);
    }


    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
    public async void SubmitScore(float score)
    {
        var response = await LeaderboardsService.Instance.AddPlayerScoreAsync(id, GameManager.ConvertSecondsToMilliseconds(score));
        Debug.Log($"player name {response.PlayerName} player score{response.Score}");
    }
    
    public bool isFetching = false;
    public async void GetLeaderboardTop()
    {
        if (isFetching) return;
        isFetching = true;

        foreach (Transform child in container.transform)
        {
            Destroy(child.gameObject);
        }

        try
        {
            var scoresResponse = await LeaderboardsService.Instance.GetScoresAsync(id);
            Debug.Log("result count " + scoresResponse.Results.Count);

            for (int i = 0; i < scoresResponse.Results.Count; i++)
            {
                GameObject obj = SpawnLeaderBoarditem();
                obj.GetComponent<LeaderBoardItem>().Initialize(scoresResponse.Results[i], i);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Leaderboard fetch failed: " + e.Message);
        }

        isFetching = false;
    }

    public void GetInput()
    {
        if (userNameInputField.text == string.Empty)
        {
            userName="Local: " + UnityEngine.Random.Range(0,100);
        }
        else
        {
        userName = userNameInputField.text;
        }
    }
    public GameObject SpawnLeaderBoarditem()
    {
        GameObject obj = Instantiate(leaderBoardItemPrefab, container.transform);
        return obj;
    }
}