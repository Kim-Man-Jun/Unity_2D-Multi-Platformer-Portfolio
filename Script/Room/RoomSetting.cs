using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RoomSetting : MonoBehaviourPunCallbacks, IPunObservable, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public TMP_Text roomName;
    public TMP_Text roomPlayerCount;

    LobbyManager lobbyManager;

    Vector3 curPos;

    public event Action<RoomSetting> OnPlayRoomClick;

    private void Awake()
    {
        gameObject.GetComponent<Outline>().enabled = false;

        lobbyManager = GameObject.Find("Canvas").GetComponent<LobbyManager>();
    }

    //�������� ��
    public void OnPointerEnter(PointerEventData eventData)
    {
        gameObject.GetComponent<Outline>().enabled = true;
    }

    //Ŭ������ ��
    public void OnPointerExit(PointerEventData eventData)
    {
        gameObject.GetComponent<Outline>().enabled = false;
    }

    //����� ��
    public void OnPointerClick(PointerEventData eventData)
    {
        OnPlayRoomClick?.Invoke(this);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //���� ������ ����
            stream.SendNext(transform.position);
        }
        else
        {
            //���޹��� ���� ����
            curPos = (Vector3)stream.ReceiveNext();
        }
    }
}
