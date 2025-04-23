
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Leaderboards;
using Unity.Services.Core;
public class LeaderBoardMenu : MonoBehaviour
{
    public static LeaderBoardMenu instance;
    public string id;
    public GameObject container;
 


    private void Awake()
    {
        if (instance==null)
        {
            instance = this;
        }
    }
    private void Start()
    {
        InitServices();
    }
    private void OnDestroy()
    {
     
        if (instance==this)
        {
            instance=null;
        }
    } 
    public async void InitServices()
    {
  
        await UnityServices.InitializeAsync();
        await  AuthenticationService.Instance.SignInAnonymouslyAsync();
        await AuthenticationService.Instance.UpdatePlayerNameAsync("william");

    }



    public async void SubmitScore(long score)
    {
        var response = await LeaderboardsService.Instance.AddPlayerScoreAsync(id, score);
        Debug.Log($"player name {response.PlayerName} player score{response.Score}");
    }

    public async void GetLeaderboardTop()
    {
        var scoresResponse = await LeaderboardsService.Instance.GetScoresAsync(id);
        LeaderBoardItem[] items = container.GetComponentsInChildren<LeaderBoardItem>(true);
        Debug.Log("result count "+scoresResponse.Results.Count);
        for(int i=0;i<items.Length;i++)
        {
            items[i].Initialize(scoresResponse.Results[0], i);
        }
    }
  }

