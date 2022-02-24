using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class WaitingRoomManager : MonoBehaviour
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

    // Start is called before the first frame update
    void Start()
    {
        Vector2 spawnPosition = new Vector2(Random.Range(minSpawnValues.x, maxSpawnValues.x), Random.Range(minSpawnValues.y, maxSpawnValues.y));
        GameObject character = PhotonNetwork.Instantiate(playerPrefab.name, Vector2.zero, Quaternion.identity);
        GameObject player = character.GetComponentInChildren<PlayerController>().body;
        player.transform.position = spawnPosition;

        PhotonNetwork.CurrentRoom.MaxPlayers = (byte)maxNumberOfPlayers;
        currentNumerOfPlayers = PhotonNetwork.CurrentRoom.PlayerCount;

        PhotonNetwork.LocalPlayer.NickName = "Player " + currentNumerOfPlayers;
        PlayerController controller = player.GetComponent<PlayerController>();
        controller.name = PhotonNetwork.LocalPlayer.NickName;
        Debug.Log("Waiting Room");
    }

    // Update is called once per frame
    void Update()
    {
        PlayerCount();
    }

    private void PlayerCount()
    {
        currentNumerOfPlayers = PhotonNetwork.CurrentRoom.PlayerCount;

        if(currentNumerOfPlayers == maxNumberOfPlayers && !isCountingDown)
        {
            StartCoroutine(CountDown());
        }
        
        if(!isCountingDown && currentNumerOfPlayers != maxNumberOfPlayers)
        {
            playerCountText.text = "Players Needed:\n" + (maxNumberOfPlayers - currentNumerOfPlayers);
        }
    }

    private IEnumerator CountDown()
    {
        int i = countDownTime;
        isCountingDown = true;

        while (i > 0) 
        {
            if(currentNumerOfPlayers != maxNumberOfPlayers)
            {
                isCountingDown = false;
                break;
            }

            playerCountText.text = "Starting in:\n" + i.ToString();
            yield return new WaitForSeconds(1);
            i--;

            if(i == 0)
            {
                PhotonNetwork.LoadLevel("Game");
            }
        }
    }
}
