using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using UnityEngine.SceneManagement;

public class WaitingRoomManager : MonoBehaviourPunCallbacks
{
    [Header("Player Spawn Info:")]
    public GameObject playerPrefab;
    public Vector2 maxSpawnValues, minSpawnValues;

    [Header("Player Count Info:")]
    public Text playerCountText;
    public int currentNumerOfPlayers;
    public int maxNumberOfPlayers;
    public bool isCountingDown;
    public int countDownTime;

    [Header("Team Info")]
    public int blueTeamCount;
    public int redTeamCount;
    [ColorUsage(true)]
    public Color redTeamColor;
    [ColorUsage(true)]
    public Color blueTeamColor;

    private PhotonTeamsManager photonTeamManager;
    private PhotonTeam[] teams;
    private PlayerController playerController;
    private TeamManager teamManager;

    // Start is called before the first frame update
    void Start()
    {
        teamManager = FindObjectOfType<TeamManager>();
        //photonTeamManager = GetComponent<PhotonTeamsManager>();

        //spawn player
        Vector2 spawnPosition = new Vector2(Random.Range(minSpawnValues.x, maxSpawnValues.x), Random.Range(minSpawnValues.y, maxSpawnValues.y));
        GameObject character = PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, Quaternion.identity);
        GameObject player = character.GetComponentInChildren<PlayerController>().body;

        //get the current number of players in the room
        PhotonNetwork.CurrentRoom.MaxPlayers = (byte)maxNumberOfPlayers;
        currentNumerOfPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
        
        //add the players nickname
        PhotonNetwork.LocalPlayer.NickName = PlayerPrefs.GetString("PlayerName");
        playerController = player.GetComponent<PlayerController>();
        playerController.name = PhotonNetwork.LocalPlayer.NickName;
        
        //assign player to an available team
        //teams = photonTeamManager.GetAvailableTeams();
        //UpdatePlayerList();
        //AssignTeam();
    }

    // Update is called once per frame
    void Update()
    {
        TeamCount();
        //UpdatePlayerList();
    }

    //private void UpdatePlayerList()
    //{
    //    blueTeamCount = 0;
    //    redTeamCount = 0;

    //    //for each player get there team and add them to the teams count
    //    foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
    //    {
    //        PhotonTeam playersTeam = player.Value.GetPhotonTeam();

    //        if (playersTeam == teams[0])
    //        {
    //            teamManager.blueTeamCount++;
    //        }
    //        else if (playersTeam == teams[1])
    //        {
    //            redTeamCount++;
    //        }
    //    }
    //}

    //private void AssignTeam()
    //{
    //    //assign local player to team with few players
    //    if (blueTeamCount <= redTeamCount || blueTeamCount == redTeamCount)
    //    {
    //        PhotonNetwork.LocalPlayer.JoinTeam("Blue");
    //        blueTeamCount++;
    //    }
    //    else
    //    {
    //        PhotonNetwork.LocalPlayer.JoinTeam("Red");
    //        redTeamCount++;
    //    }
    //}

    private void TeamCount()
    {
        currentNumerOfPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
        
        //if both teams equal the same number of players, start the count down
        if(teamManager.blueTeamCount == teamManager.redTeamCount && !isCountingDown && currentNumerOfPlayers != 1)
        {
            StartCoroutine(CountDown());
        }
        
        //output how many players are needed
        if(!isCountingDown && teamManager.blueTeamCount != teamManager.redTeamCount)
        {
            playerCountText.text = "Players Needed:\n" + (Mathf.Abs(teamManager.blueTeamCount - teamManager.redTeamCount));
        }
    }

    private IEnumerator CountDown()
    {
        int i = countDownTime;
        isCountingDown = true;

        //count down i until it is no longer greater than zero
        while (i > 0) 
        {
            if(blueTeamCount != redTeamCount)
            {
                isCountingDown = false;
                break;
            }

            playerCountText.text = "Starting in:\n" + i.ToString();
            yield return new WaitForSeconds(1);
            i--;

            //once i equals zero load the game scene
            if (i == 0)
            {
                //PhotonNetwork.LoadLevel("Game");
                Debug.Log("start game");
            }
        }
    }

    public void StateOfPause()
    {
        playerController.isPaused = !playerController.isPaused;
    }

    public void ChangeScene(string scene)
    {
        //change to the scene given in the parameter
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }
}
