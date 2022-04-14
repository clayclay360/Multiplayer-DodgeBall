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

    [Space]
    public GameObject ball;

    private PlayerController currentPlayerController;
    private TeamManager teamManager;

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        teamManager = FindObjectOfType<TeamManager>();

        //PhotonNetwork.Instantiate(ball.name, Vector3.zero, Quaternion.identity);

        //spawn player
        Vector2 spawnPosition = new Vector2(Random.Range(minSpawnValues.x, maxSpawnValues.x), Random.Range(minSpawnValues.y, maxSpawnValues.y));
        GameObject character = PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, Quaternion.identity);
        GameObject player = character.GetComponentInChildren<PlayerController>().body;

        //get the current number of players in the room
        PhotonNetwork.CurrentRoom.MaxPlayers = (byte)maxNumberOfPlayers;
        currentNumerOfPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
        
        //add the players nickname
        PhotonNetwork.LocalPlayer.NickName = PlayerPrefs.GetString("PlayerName");
        currentPlayerController = player.GetComponent<PlayerController>();
        currentPlayerController.playerName = PhotonNetwork.LocalPlayer.NickName;

        teamManager.playerController = currentPlayerController;
    }

    // Update is called once per frame
    void Update()
    {
        TeamCount();
    }

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
            if (teamManager.blueTeamCount != teamManager.redTeamCount)
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
                if (PhotonNetwork.IsMasterClient)
                {
                    DodeballScript[] dodgeballs = FindObjectsOfType<DodeballScript>();
                    foreach (DodeballScript db in dodgeballs)
                    {
                        Destroy(db.gameObject);
                    }

                    PhotonNetwork.LoadLevel("Game");
                }
            }
        }
    }

    public void StateOfPause()
    {
        currentPlayerController.isPaused = !currentPlayerController.isPaused;
    }

    public void ChangeScene(string scene)
    {
        //change to the scene given in the parameter
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }
}
