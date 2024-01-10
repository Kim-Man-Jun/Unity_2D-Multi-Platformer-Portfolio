using BackEnd;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Cinemachine.DocumentationSortingAttribute;

public class stageManager : MonoBehaviourPunCallbacks
{
    public Transform playerPos1 = null;
    public Transform playerPos2 = null;
    public Transform playerPos3 = null;
    public Transform playerPos4 = null;

    public static bool stageClear = false;

    private void Awake()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == 1)
        {
            PhotonNetwork.Instantiate("Player1", playerPos1.position, Quaternion.identity);
        }
        else if (PhotonNetwork.LocalPlayer.ActorNumber == 2)
        {
            PhotonNetwork.Instantiate("Player2", playerPos2.position, Quaternion.identity);
        }
        else if (PhotonNetwork.LocalPlayer.ActorNumber == 3)
        {
            PhotonNetwork.Instantiate("Player3", playerPos3.position, Quaternion.identity);
        }
        else if (PhotonNetwork.LocalPlayer.ActorNumber == 4)
        {
            PhotonNetwork.Instantiate("Player4", playerPos4.position, Quaternion.identity);
        }
    }

    private void Update()
    {

    }

    public void RoomInfo()
    {
        if (PhotonNetwork.InRoom)
        {
            Debug.Log("방 이름 : " + PhotonNetwork.CurrentRoom.Name);
            Debug.Log("방 현재 인원 : " + PhotonNetwork.CurrentRoom.PlayerCount);
            Debug.Log("방 최대 인원 : " + PhotonNetwork.CurrentRoom.MaxPlayers);
        }
        else
        {
            Debug.Log("접속한 인원 : " + PhotonNetwork.CountOfPlayers);
            Debug.Log("방 갯수 : " + PhotonNetwork.CountOfRooms);
            Debug.Log("로비에 존재하는지 : " + PhotonNetwork.InLobby);
            Debug.Log("연결됨 : " + PhotonNetwork.IsConnected);
        }
    }


    public void GoalNextButton()
    {
        StartCoroutine(LeaveRoom());
    }

    IEnumerator LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();

        yield return new WaitForSeconds(1f);

        PhotonNetwork.JoinLobby();
        SceneManager.LoadScene("Lobby");
    }
}
