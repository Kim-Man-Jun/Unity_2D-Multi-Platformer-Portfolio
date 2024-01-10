using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public GameObject victoryPanel;

    PhotonView pv;

    private void Start()
    {
        pv = GetComponent<PhotonView>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SceneSoundManager.instance.WinDisplay();

            string playerName = collision.transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text;

            // 모든 플레이어에게 Goal에 닿은 플레이어의 ActorNumber를 전송
            pv.RPC("playerGoal", RpcTarget.All, playerName);
        }
    }

    [PunRPC]
    private void playerGoal(string playername)
    {
        //카메라 조정
        var cam = GameObject.Find("playerFollowCamera").GetComponent<CinemachineVirtualCamera>();

        cam.Follow = this.gameObject.transform;
        cam.LookAt = this.gameObject.transform;

        //변수 조정
        stageManager.stageClear = true;

        victoryPanel.SetActive(true);

        victoryPanel.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = playername;
    }
}
