using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    [Header("Start Game Info:")]
    public bool startGame;
    public int startTimer;
    public int playersAlive;
    public Text startTimerText;

    [Header("Player Spawning")]
    public GameObject playerPrefab;
    public Transform[] playerSpawnPositions;

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
            startGame = true;
        }
    }

    private void SpawnPlayers()
    {
        string name = PhotonNetwork.LocalPlayer.NickName;

        switch (name)
        {
            case "Player 1":
                PhotonNetwork.Instantiate(playerPrefab.name, playerSpawnPositions[0].position, Quaternion.identity);
                break;
            case "Player 2":
                PhotonNetwork.Instantiate(playerPrefab.name, playerSpawnPositions[1].position, Quaternion.identity);
                break;
            case "Player 3":
                PhotonNetwork.Instantiate(playerPrefab.name, playerSpawnPositions[3].position, Quaternion.identity);
                break;
            case "Player 4":
                PhotonNetwork.Instantiate(playerPrefab.name, playerSpawnPositions[4].position, Quaternion.identity);
                break;
        }
    }
}
