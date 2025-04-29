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
 

    private void Awake()
    {
        if (instance==null)
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
            playerId=System.Guid.NewGuid().ToString();
            playerId=playerId.Replace('-',' ').Trim();
            if(playerId.Length>5)
            {
                for(int i=6;i<playerId.Length;i++)
                {
                    playerId=playerId.Remove(i);
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
        GetLeaderboardTop();
    }
    public async void SetUserName()
    {
        await AuthenticationService.Instance.UpdatePlayerNameAsync(userName);
        Debug.Log("New Player ID: " + AuthenticationService.Instance.PlayerId);
        Debug.Log("New Player name: " + AuthenticationService.Instance.PlayerName);
    }
   

    private void OnDestroy()
    {
     
        if (instance==this)
        {
            instance=null;
        }
    } 
    //public async void InitServices()
    //{

    //    await UnityServices.InitializeAsync();
    //    var options = new InitializationOptions();
    //    await AuthenticationService.Instance.SignInAnonymouslyAsync(); 

    //}



    public async void SubmitScore(long score)
    {
        var response = await LeaderboardsService.Instance.AddPlayerScoreAsync(id, score);
        Debug.Log($"player name {response.PlayerName} player score{response.Score}");
    }

    public async void GetLeaderboardTop()
    {
        var scoresResponse = await LeaderboardsService.Instance.GetScoresAsync(id);
      //  LeaderBoardItem[] items = container.GetComponentsInChildren<LeaderBoardItem>(true);
        Debug.Log("result count "+scoresResponse.Results.Count);
        for(int i=0;i<scoresResponse.Results.Count;i++)
        {
           GameObject obj= SpawnLeaderBoarditem();
           obj.GetComponent<LeaderBoardItem>().Initialize(scoresResponse.Results[i], i);
        }
    }
     public  string userName;
    public void GetInput()
    {
      userName = userNameInputField.text;
    }
    public GameObject SpawnLeaderBoarditem()
    {

        GameObject obj = Instantiate(leaderBoardItemPrefab, container.transform);
        return obj;

    }
  }

