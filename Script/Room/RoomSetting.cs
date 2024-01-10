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

    //입장했을 때
    public void OnPointerEnter(PointerEventData eventData)
    {
        gameObject.GetComponent<Outline>().enabled = true;
    }

    //클릭했을 때
    public void OnPointerExit(PointerEventData eventData)
    {
        gameObject.GetComponent<Outline>().enabled = false;
    }

    //벗어났을 때
    public void OnPointerClick(PointerEventData eventData)
    {
        OnPlayRoomClick?.Invoke(this);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //현재 포지션 전달
            stream.SendNext(transform.position);
        }
        else
        {
            //전달받은 정보 받음
            curPos = (Vector3)stream.ReceiveNext();
        }
    }
}
