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

    [Header("Game Variables:")]
    public int playersAlive;
    public int startTimer;
    
    public GameObject playerPrefab;
    public Collider2D startLine;
    public Text startTimerText;

    [Header("Spawn Variables:")]
    public Vector2 maxSpawnValues;
    public Vector2 minSpawnValues;

    private Vector2 spawnPosition;
    private PlayerController currentPlayerController;
    private TeamManager teamManager;

    // Start is called before the first frame update
    void Start()
    {
        teamManager = FindObjectOfType<TeamManager>();
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

        //add the players nickname
        PhotonNetwork.LocalPlayer.NickName = PlayerPrefs.GetString("PlayerName");
        currentPlayerController = player.GetComponent<PlayerController>();
        currentPlayerController.playerName = PhotonNetwork.LocalPlayer.NickName;

        teamManager.playerController = currentPlayerController;
        StartCoroutine(StartTimer());
    }

    // Update is called once per frame
    void Update()
    {
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
                startTimerText.text = "Start!";
                startLine.enabled = false;
                yield return new WaitForSeconds(1);
                startTimerText.text = "";
            }
            //scoreBoard.SetActive(true);
            gameStarted = true;
            roundOver = false;
        }
    }


}
