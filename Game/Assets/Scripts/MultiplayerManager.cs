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
            PhotonNetwork.ConnectUsingSettings(); //connect to photon
        }
    }

    public void CheckConnection()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            errorText.text = "Connection Error: Check Internet Connection";

            if (playerName.text != "")
            {
                //take the player into the lobby
                startMenu.SetActive(false);
                StartCoroutine(LoadingLobby());
            }
            else
            {
                //display error if there's no name 
                errorText.text = "Insert A Name";
                StartCoroutine(DisplayError());
                return;
            }
            
            PlayerPrefs.SetString("PlayerName", playerName.text); //save the player name
        }
        else
        {

            StartCoroutine(DisplayError()); // display error not connected to photon
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
        PhotonNetwork.JoinLobby(); //when connected to master server, join lobby
    }

    public IEnumerator LoadingLobby()
    {
        while (true)
        {
            yield return null;

            if (PhotonNetwork.InLobby)
            {
                //if in the lobby display the create and join menu
                loadingTextGameObject.SetActive(false);
                createAndJoinMenu.SetActive(true);
                break;
            }
            else
            {
                //if not in lobby, display loading screen
                loadingTextGameObject.SetActive(true);
            }
        }
    }

    public void CreateRoom()
    {
        if (createRoom.text != "" )
        {
            //create room with the name 
            PhotonNetwork.CreateRoom(createRoom.text);
        }
        else
        {
            //diesplay error if no name
            errorText.text = "Insert A Name";
            StartCoroutine(DisplayError());
        }
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(joinRoom.text); //join room by the name
    }

    public void FindRoom()
    {
        PhotonNetwork.JoinRandomOrCreateRoom(); //join or create a random room
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Waiting Room"); //on joined room, load the waiting room
    }
}
