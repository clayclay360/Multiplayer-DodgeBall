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

    private PhotonTeamsManager teamManager;
    private PhotonTeam[] teams;
    private PlayerController playerController;


    // Start is called before the first frame update
    void Start()
    {
        teamManager = GetComponent<PhotonTeamsManager>();

        Vector2 spawnPosition = new Vector2(Random.Range(minSpawnValues.x, maxSpawnValues.x), Random.Range(minSpawnValues.y, maxSpawnValues.y));
        GameObject character = PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, Quaternion.identity);
        GameObject player = character.GetComponentInChildren<PlayerController>().body;

        PhotonNetwork.CurrentRoom.MaxPlayers = (byte)maxNumberOfPlayers;
        currentNumerOfPlayers = PhotonNetwork.CurrentRoom.PlayerCount;

        PhotonNetwork.LocalPlayer.NickName = PlayerPrefs.GetString("PlayerName");
        playerController = player.GetComponent<PlayerController>();

        playerController.name = PhotonNetwork.LocalPlayer.NickName;

        teams = teamManager.GetAvailableTeams();

        AssignTeam();
    }

    // Update is called once per frame
    void Update()
    {
        TeamCount();
        updatePlayerList();
    }

    private void updatePlayerList()
    {
        blueTeamCount = 0;
        redTeamCount = 0;

        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            PhotonTeam playersTeam = player.Value.GetPhotonTeam();

            if (playersTeam == teams[0])
            {
                blueTeamCount++;
            }
            else if (playersTeam == teams[1])
            {
                redTeamCount++;
            }

            //Debug.Log("Name: " + player.Value.NickName + "Team: " + playersTeam);
        }
    }

    private void AssignTeam()
    {
        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            PhotonTeam playersTeam = player.Value.GetPhotonTeam();

            Debug.Log("Name: " + player.Value.NickName + "Team: " + playersTeam);

            if (playersTeam == teams[0])
            {
                blueTeamCount++;
            }
            else if (playersTeam == teams[1])
            {
                redTeamCount++;
            }
        }

        if (blueTeamCount <= redTeamCount || blueTeamCount == redTeamCount)
        {
            PhotonNetwork.LocalPlayer.JoinTeam("Blue");
            blueTeamCount++;
        }
        else
        {
            PhotonNetwork.LocalPlayer.JoinTeam("Red");
            redTeamCount++;
        }
    }

    private void TeamCount()
    {
        currentNumerOfPlayers = PhotonNetwork.CurrentRoom.PlayerCount;

        if(blueTeamCount == redTeamCount && !isCountingDown && currentNumerOfPlayers != 1)
        {
            StartCoroutine(CountDown());
        }
        
        if(!isCountingDown && blueTeamCount != redTeamCount)
        {
            playerCountText.text = "Players Needed:\n" + (Mathf.Abs(blueTeamCount - redTeamCount));
        }
    }

    private IEnumerator CountDown()
    {
        int i = countDownTime;
        isCountingDown = true;

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

            if(i == 0)
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
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }
}
