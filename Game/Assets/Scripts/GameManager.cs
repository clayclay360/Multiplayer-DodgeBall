using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

public class GameManager : MonoBehaviour
{
    [Header("Game Info:")]
    public bool gameStarted;
    public bool roundOver;
    public int redTeamScore;
    public int blueTeamScore;

    [Header("Game Variables:")]
    public int redPlayersAlive;
    public int bluePlayersAlive;
    public int startTimer;
    
    public GameObject playerPrefab;
    public Collider2D startLine;
    public Text startTimerText;
    public Text winnerText;

    [Header("Player Spawn Variables:")]
    public Vector2 maxSpawnValues;
    public Vector2 minSpawnValues;

    [Header("Ball Spawn Variables")]
    public GameObject dodgeBallPrefab;
    public float[] pos_y;

    private string winningTeam;

    private Vector2 spawnPosition;
    private PlayerController currentPlayerController;
    private TeamManager teamManager;
    private PhotonTeamsManager photonTeamManager;
    private PhotonTeam[] teams;

    // Start is called before the first frame update
    void Start()
    {
        teamManager = FindObjectOfType<TeamManager>();
        photonTeamManager = FindObjectOfType<PhotonTeamsManager>();
        teams = photonTeamManager.GetAvailableTeams();
        startLine.GetComponent<Collider2D>();

        //spawn player
        #region
        if (PhotonNetwork.LocalPlayer.GetPhotonTeam().Name.Equals("Blue"))
        {
            spawnPosition = new Vector2(-7.75f, Random.Range(minSpawnValues.y, maxSpawnValues.y));
        }
        else
        {
            spawnPosition = new Vector2(7.75f, Random.Range(minSpawnValues.y, maxSpawnValues.y));
        }

        #endregion
        GameObject character = PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, Quaternion.identity);
        GameObject player = character.GetComponentInChildren<PlayerController>().body;
        character.GetComponentInChildren<PlayerController>().hasBall = false;

        //add the players nickname
        PhotonNetwork.LocalPlayer.NickName = PlayerPrefs.GetString("PlayerName");
        currentPlayerController = player.GetComponent<PlayerController>();
        currentPlayerController.playerName = PhotonNetwork.LocalPlayer.NickName;

        teamManager.playerController = currentPlayerController;

        PositionBalls();
        StartCoroutine(StartTimer());
    }

    // Update is called once per frame
    void Update()
    {
        CheckPlayersEliminated();
    }

    private IEnumerator StartTimer()
    {
        int time = startTimer;

        while (time > 0)
        {
            startTimerText.text = time.ToString();
            yield return new WaitForSeconds(1);
            time--;

            if (time == 0)
            {
                startTimerText.text = "Go!";
                startLine.enabled = false;
                yield return new WaitForSeconds(1);
                startTimerText.text = "";
            }
            gameStarted = true;
            roundOver = false;
        }
    }

    private void CheckPlayersEliminated()
    {
        if (!roundOver && gameStarted)
        {
            redPlayersAlive = 0;
            bluePlayersAlive = 0;

            foreach (KeyValuePair<int, Player> currentPlayer in PhotonNetwork.CurrentRoom.Players)
            {
                GameObject player = (GameObject)currentPlayer.Value.TagObject;
                if (!player.GetComponentInChildren<PlayerController>().isOut)
                {
                    if (currentPlayer.Value.GetPhotonTeam().Equals(teams[0]))
                    {
                        bluePlayersAlive++;
                    }
                    else
                    {
                        redPlayersAlive++;
                    }
                }
            }

            if (redPlayersAlive <= 0)
            {
                winningTeam = "Blue";
                roundOver = true;
                blueTeamScore++;
                CheckScore();
            }
            else if (bluePlayersAlive <= 0)
            {
                winningTeam = "Red";
                roundOver = true;
                redTeamScore++;
                CheckScore();
            }
        }
    }

    private void CheckScore()
    {
        if (blueTeamScore - 2 >= redTeamScore ||
                redTeamScore - 2 >= blueTeamScore)
        {
            StartCoroutine(DisplayGameWinner());
        }
        else if (roundOver)
        {
            StartCoroutine(DisplayRoundWinner());
        }
    }

    private IEnumerator DisplayGameWinner()
    {
        yield return new WaitForSeconds(.5f);
        winnerText.text = winningTeam + " Wins!";
        yield return new WaitForSeconds(5f);
        winnerText.text = winningTeam + "";
        LoadScene(1);

    }

    private IEnumerator DisplayRoundWinner()
    {
        yield return new WaitForSeconds(.5f);
        winnerText.text = winningTeam + " Wins The Round";
        yield return new WaitForSeconds(3f);
        winnerText.text = winningTeam + "";
        Restart();
    }

    private void Restart()
    {
        startLine.enabled = true;

        PlayerController[] allPlayers = FindObjectsOfType<PlayerController>();
        foreach(PlayerController player in allPlayers)
        {
            player.lives = 3;
            player.isOut = false;

            if (player.teamName.Equals("Blue"))
            {
                spawnPosition = new Vector2(-7.75f, Random.Range(minSpawnValues.y, maxSpawnValues.y));
                player.transform.position = spawnPosition;
            }
            else
            {
                spawnPosition = new Vector2(7.75f, Random.Range(minSpawnValues.y, maxSpawnValues.y));
                player.transform.position = spawnPosition;
            }
        }

        PositionBalls();
        StartCoroutine(StartTimer());
    }

    private void LoadScene(int index)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(index);
        }
    }

    private void PositionBalls()
    {
        for (int i = 0; i < 4; i++)
        {
            Vector2 spawnPosition = new Vector2(0, pos_y[i]);
            if (!gameStarted)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    GameObject dodgeBall = PhotonNetwork.Instantiate(dodgeBallPrefab.name, spawnPosition, Quaternion.identity);
                    dodgeBall.GetComponent<DodeballScript>().ballName = "Ball " + i;

                }
            }
            else
            {
                DodeballScript[] dodgeBalls = FindObjectsOfType<DodeballScript>();
                if (dodgeBalls[i].view.AmOwner)
                {
                    dodgeBalls[i].transform.position = spawnPosition;
                }
            }
        }
    }
}
