using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    [Header("Start Game Info:")]
    public bool gameStarted;
    public bool roundOver;
    public int startTimer;
    public int playersAlive;
    public Text startTimerText;


    [Header("Player Spawning")]
    public GameObject playerPrefab;
    public Transform[] playerSpawnPositions;

    [Header("Scoring Info:")]
    public GameObject scoreBoard;
    public int playerOneScore;
    public Image[] playerOneScoreImages;
    [Space]
    public int playerTwoScore;
    public Image[] playerTwoScoreImages;
    [Space]
    public int playerThreeScore;
    public Image[] playerThreeScoreImages;
    [Space]
    public int playerFourScore;
    public Image[] playerFourScoreImages;

    private PlayerController playerController;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        SpawnPlayers();
        StartCoroutine(StartTimer());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator StartTimer()
    {
        int time = startTimer;

        while(time > 0)
        {
            startTimerText.text = time.ToString();
            yield return new WaitForSeconds(1);
            time--;

            if(time == 0)
            {
                startTimerText.text = "Start!";
                yield return new WaitForSeconds(1);
                startTimerText.text = "";
            }
            scoreBoard.SetActive(true);
            gameStarted = true;
            roundOver = false;
        }
    }

    private void SpawnPlayers()
    {
        string name = PhotonNetwork.LocalPlayer.NickName;

        switch (name)
        {
            case "Player 1":
                player = PhotonNetwork.Instantiate(playerPrefab.name, playerSpawnPositions[0].position, Quaternion.identity);
                playerController = player.GetComponent<PlayerController>();
                playerController.name = name;
                break;
            case "Player 2":
                player = PhotonNetwork.Instantiate(playerPrefab.name, playerSpawnPositions[1].position, Quaternion.identity);
                playerController = player.GetComponent<PlayerController>();
                playerController.name = name;

                break;
            case "Player 3":
                player = PhotonNetwork.Instantiate(playerPrefab.name, playerSpawnPositions[3].position, Quaternion.identity);
                playerController = player.GetComponent<PlayerController>();
                playerController.name = name;
                break;
            case "Player 4":
                player = PhotonNetwork.Instantiate(playerPrefab.name, playerSpawnPositions[4].position, Quaternion.identity);
                playerController = player.GetComponent<PlayerController>();
                playerController.name = name;
                break;

        }
    }

    private void Scoring()
    {
        if(playersAlive == 1 && !roundOver)
        {
            roundOver = true;
            PlayerController remainingPlayer = FindObjectOfType<PlayerController>();
            switch (remainingPlayer.name)
            {
                case "Player 1":
                    playerOneScore++;
                    playerOneScoreImages[playerOneScore - 1].color = new Color(255,255,255,255);
                    break;
                case "Player 2":
                    playerTwoScore++;
                    playerTwoScoreImages[playerTwoScore - 1].color = new Color(255, 255, 255, 255);
                    break;
                case "Player 3":
                    playerThreeScore++;
                    playerThreeScoreImages[playerThreeScore - 1].color = new Color(255, 255, 255, 255);
                    break;
                case "Player 4":
                    playerFourScore++;
                    playerFourScoreImages[playerFourScore - 1].color = new Color(255, 255, 255, 255);
                    break;
            }
        }
    }
}
