using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class MultiplayerManager : MonoBehaviourPunCallbacks
{
    public InputField createRoom, joinRoom, playerName;
    public GameObject startMenu, gameModes, createAndJoinMenu, errorTextGameObject, loadingTextGameObject;
    public Text errorText;
    public float connectionErrorTime;

    // Start is called before the first frame update
    void Start()
    {
        errorText.GetComponent<Text>();
        playerName.text = PlayerPrefs.GetString("PlayerName");

        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public void CheckConnection()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            errorText.text = "Connection Error: Check Internet Connection";

            if (playerName.text != "")
            {
                startMenu.SetActive(false);
                StartCoroutine(LoadingLobby());
            }
            else
            {
                errorText.text = "Insert A Name";
                StartCoroutine(DisplayError());
                return;
            }
            
            PlayerPrefs.SetString("PlayerName", playerName.text);
        }
        else
        {

            StartCoroutine(DisplayError());
        }
    }

    IEnumerator DisplayError()
    {
        errorTextGameObject.SetActive(true);
        yield return new WaitForSeconds(connectionErrorTime);
        errorTextGameObject.SetActive(false);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public IEnumerator LoadingLobby()
    {
        while (true)
        {
            yield return null;

            if (PhotonNetwork.InLobby)
            {
                loadingTextGameObject.SetActive(false);
                createAndJoinMenu.SetActive(true);
                break;
            }
            else
            {
                loadingTextGameObject.SetActive(true);
            }
        }
    }

    public void CreateRoom()
    {
        if (createRoom.text != "" )
        {
            PhotonNetwork.CreateRoom(createRoom.text);
        }
        else
        {
            errorText.text = "Insert A Name";
            StartCoroutine(DisplayError());
        }
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
