﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
public class TeamManager : MonoBehaviour
{

    [Header("Team Info")]
    public int blueTeamCount;
    public int redTeamCount;
    [ColorUsage(true)]
    public Color redTeamColor;
    [ColorUsage(true)]
    public Color blueTeamColor;
    [Space]
    public bool isAssigningTeams;
    public PlayerController playerController { get; set; }

    private PhotonTeamsManager teamManager;
    private PhotonTeam[] teams;

    // Start is called before the first frame update
    void Start()
    {
        teamManager = GetComponent<PhotonTeamsManager>();
        teams = teamManager.GetAvailableTeams();
        UpdatePlayerList();
        AssignTeam();
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlayerList();
    }

    private void UpdatePlayerList()
    {
        blueTeamCount = 0;
        redTeamCount = 0;

        //for each player get there team and add them to the teams count
        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            PhotonTeam playersTeam = player.Value.GetPhotonTeam();

            if (playersTeam == teams[0])
            {
                playerController.teamName = "Blue";
                blueTeamCount++;
            }
            else if (playersTeam == teams[1])
            {
                playerController.teamName = "Red";
                redTeamCount++;
            }
        }

        PlayerController[] allPlayerControllers = FindObjectsOfType<PlayerController>();
        foreach(PlayerController player in allPlayerControllers)
        {
            switch (player.teamName)
            {
                case "Red":
                    player.teamColor = redTeamColor;
                    break;
                case "Blue":
                    player.teamColor = blueTeamColor;
                    break;
            }
        }
    }

    private void AssignTeam()
    {
        //assign local player to team with few players
        if (isAssigningTeams)
        {
            if (blueTeamCount <= redTeamCount || blueTeamCount == redTeamCount)
            {
                PhotonNetwork.LocalPlayer.JoinTeam("Blue");
            }
            else
            {
                PhotonNetwork.LocalPlayer.JoinTeam("Red");
            }
        }
    }
}
