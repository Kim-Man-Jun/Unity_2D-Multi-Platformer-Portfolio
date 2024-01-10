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
            Debug.Log("�� �̸� : " + PhotonNetwork.CurrentRoom.Name);
            Debug.Log("�� ���� �ο� : " + PhotonNetwork.CurrentRoom.PlayerCount);
            Debug.Log("�� �ִ� �ο� : " + PhotonNetwork.CurrentRoom.MaxPlayers);
        }
        else
        {
            Debug.Log("������ �ο� : " + PhotonNetwork.CountOfPlayers);
            Debug.Log("�� ���� : " + PhotonNetwork.CountOfRooms);
            Debug.Log("�κ� �����ϴ��� : " + PhotonNetwork.InLobby);
            Debug.Log("����� : " + PhotonNetwork.IsConnected);
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
