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

            // ��� �÷��̾�� Goal�� ���� �÷��̾��� ActorNumber�� ����
            pv.RPC("playerGoal", RpcTarget.All, playerName);
        }
    }

    [PunRPC]
    private void playerGoal(string playername)
    {
        //ī�޶� ����
        var cam = GameObject.Find("playerFollowCamera").GetComponent<CinemachineVirtualCamera>();

        cam.Follow = this.gameObject.transform;
        cam.LookAt = this.gameObject.transform;

        //���� ����
        stageManager.stageClear = true;

        victoryPanel.SetActive(true);

        victoryPanel.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = playername;
    }
}
