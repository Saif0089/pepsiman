using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Leaderboards.Models;
using UnityEngine;

public class LeaderBoardItem : MonoBehaviour
{
    public TextMeshProUGUI srtxt;
    public TextMeshProUGUI nametxt;
    public TextMeshProUGUI timetxt;


    public void Initialize(LeaderboardEntry player, int sr)
    {
        srtxt.text = sr.ToString();
        nametxt.text = player.PlayerName;

        int time = (int)player.Score;
        int min = Mathf.FloorToInt((float)time / 60f);
        int sec = Mathf.FloorToInt((float)time % 60f);

        timetxt.text = string.Format("{0:00}:{1:00}", min, sec);
        Debug.Log("get score " + time);
    }


}
