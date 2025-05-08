using System;
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

        int min = 0;
        int sec = 0;
        int millisec = 0;
        long time = (long)player.Score;
        
        GameManager.SubdivideMilliseconds(time, out min, out sec, out millisec);
        

        timetxt.text = string.Format("{0:00}:{1:00}:{2:00}", min, sec, millisec);
        Debug.Log("get score " + time);
    }
}