using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class MultiplayerManager : MonoBehaviourPunCallbacks
{
    public InputField createRoom, joinRoom;
    public GameObject gameModes, createAndJoinMenu, connectionErrorText;
    public float connectionErrorTime;

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public void CheckConnection()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            gameModes.SetActive(false);
            createAndJoinMenu.SetActive(true);
        }
        else
        {
            StartCoroutine(DisplayConnectionError());
        }
    }

    IEnumerator DisplayConnectionError()
    {
        connectionErrorText.SetActive(true);
        yield return new WaitForSeconds(connectionErrorTime);
        connectionErrorText.SetActive(true);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(createRoom.text);
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(joinRoom.text);
    }

    public void FindRoom()
    {
        PhotonNetwork.JoinRandomOrCreateRoom();
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Waiting Room");
    }
}
