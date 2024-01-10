using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayRoomSetting : MonoBehaviourPunCallbacks
{
    public GameObject player1Slot;
    public GameObject player2Slot;
    public GameObject player3Slot;
    public GameObject player4Slot;

    private void Awake()
    {
        player1Slot.SetActive(false);
        player2Slot.SetActive(false);
        player3Slot.SetActive(false);
        player4Slot.SetActive(false);
    }

    private void Start()
    {

    }
}
