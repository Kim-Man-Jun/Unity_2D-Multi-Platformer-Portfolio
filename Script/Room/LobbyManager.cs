using BackEnd;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.Demo.Cockpit;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public static LobbyManager Instance;

    //우측 상단 플레이어 네임
    public TMP_Text playerName;

    public PhotonView pv;

    [Header("Room Make Objcet")]
    //패널들
    public GameObject roomMakePanel;
    //다른 위치 터치 예방
    public GameObject touchPreventPanel;

    public TMP_InputField roomName;
    public TMP_Dropdown roomPlayerCount;
    public List<Image> gameStageImage;

    public GameObject roomPrefab;
    public RectTransform watingRoomContent;

    private List<RoomInfo> cachedRoomList = new List<RoomInfo>();

    [Header("Room Join Objcet")]
    public GameObject roomJoinPanel;
    private string roomNameTemp;

    public GameObject roomJoinFailWarning;
    private float disappearTime = 1.5f;

    public GameObject playRoomObj;
    public TMP_Text playRoomName;
    public GameObject player1Slot;
    public GameObject player2Slot;
    public GameObject player3Slot;
    public GameObject player4Slot;

    public bool isReady = false;

    //막 들어온 플레이어가 부여받는 고유값
    public int playerNum;
    //방 최대 인원수와 ready한 인원수가 맞을 경우 실행
    public int[] readyCount;

    [Header("Game Start Objcet")]
    public GameObject playTouchPreventPanel;
    public GameObject HereWeGo;

    [Header("Game Quit Objcet")]
    public GameObject gameQuitPanel;

    private void Awake()
    {
        PhotonNetwork.NickName = Backend.UserNickName;

        playerName.text = PhotonNetwork.NickName;

        //Photon에 연결
        PhotonNetwork.ConnectUsingSettings();

        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
    }

    private void Start()
    {
        SceneSoundManager.instance.LobbyScene();
    }

    //포톤 서버 로비에 접속
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    //방 만들때 시퀀스
    public void OnCreateRoomFirst()
    {
        if (photonView.IsMine == true)
        {
            OnCreateRoomMaster();
        }
    }

    public void OnCreateRoomMaster()
    {
        PhotonNetwork.CreateRoom(roomName.text, new RoomOptions { MaxPlayers = roomPlayerCount.value + 1 }, TypedLobby.Default);

        RoomMakeCancelButton();

        //초기화
        roomName.text = string.Empty;
        roomPlayerCount.value = 0;
    }

    public override void OnCreatedRoom()
    {
        GameObject room = PhotonNetwork.Instantiate("PlayRoom", new Vector3(0, 0, 0), Quaternion.identity);

        Transform pos = GameObject.FindGameObjectWithTag("RoomUI").transform;
        room.transform.SetParent(pos);
        room.transform.localScale = new Vector3(1, 1, 1);

        //이미지 셋팅
        //room.transform.GetChild(0).gameObject.SetActive(true);

        //방제목 셋팅
        room.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = PhotonNetwork.CurrentRoom.Name;

        //현재 인원수 셋팅
        room.transform.GetChild(2).gameObject.GetComponent<TMP_Text>().text = $"{PhotonNetwork.CurrentRoom.PlayerCount} / {PhotonNetwork.CurrentRoom.MaxPlayers}";

        //방장 기준으로 코드 작성
        //플레이 룸 이름
        playRoomName.text = PhotonNetwork.CurrentRoom.Name;

        SceneSoundManager.instance.RoomDisplay();
    }

    //새로운 룸이 만들어지거나 룸이 삭제되는 경우에 실행
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (cachedRoomList.Count <= 0)
        {
            cachedRoomList = roomList;
        }
        else
        {
            foreach (var room in roomList)
            {
                for (int i = 0; i < cachedRoomList.Count; i++)
                {
                    if (cachedRoomList[i].Name == room.Name)
                    {
                        List<RoomInfo> newlist = cachedRoomList;

                        if (room.RemovedFromList)
                        {
                            newlist.Remove(newlist[i]);
                        }
                        else
                        {
                            newlist[i] = room;
                        }

                        cachedRoomList = newlist;
                    }
                }
            }
        }

        UpdateUI();
    }

    //다른 유저들에게 보여주고 전달하기 위한 메서드
    public void UpdateUI()
    {
        foreach (Transform roomItem in watingRoomContent)
        {
            Destroy(roomItem.gameObject);
        }

        foreach (var room in cachedRoomList)
        {
            GameObject roomItem = Instantiate(roomPrefab, watingRoomContent);

            Transform pos = GameObject.FindGameObjectWithTag("RoomUI").transform;
            roomItem.transform.SetParent(pos);
            roomItem.transform.localScale = new Vector3(1, 1, 1);

            //이미지 셋팅
            //roomItem.transform.GetChild(0).gameObject.SetActive(true);

            //방제목 셋팅
            roomItem.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = room.Name;

            //현재 인원수 셋팅
            roomItem.transform.GetChild(2).gameObject.GetComponent<TMP_Text>().text = $"{room.PlayerCount} / {room.MaxPlayers}";

            RoomSetting roomSetting = roomItem.GetComponent<RoomSetting>();

            roomSetting.OnPlayRoomClick += RoomClickCheck;

            //만들어진 방 이름 임시 저장
            roomNameTemp = room.Name;
        }
    }


    //방에 참가하려는 유저 기준
    //roomsetting에서 이벤트 받아옴
    public void RoomClickCheck(RoomSetting roomSetting)
    {
        RoomJoinButton();
        roomJoinPanel.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = $"\" {roomSetting.transform.GetChild(1).GetComponent<TMP_Text>().text} \"";
    }

    //게임 방에 들어갔을 때 버튼 액션
    public void RoomJoinOn()
    {
        PhotonNetwork.JoinRoom(roomNameTemp);
    }

    //실패했을때
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("방 입장 실패");
        RoomJoinCancelButton();

        roomJoinFailWarning.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        roomJoinFailWarning.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().color = new Color(0, 0, 0, 1);
        roomJoinFailWarning.gameObject.SetActive(true);

        StartCoroutine(RoomJoinFail());
    }

    IEnumerator RoomJoinFail()
    {
        Color panelColor = roomJoinFailWarning.GetComponent<Image>().color;
        Color letterColor = roomJoinFailWarning.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().color;
        float nowTime = 0;

        while (nowTime < disappearTime)
        {
            float alpha = Mathf.Lerp(1f, 0f, nowTime / disappearTime);

            roomJoinFailWarning.GetComponent<Image>().color = new Color(1, 1, 1, alpha);
            roomJoinFailWarning.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().color = new Color(0, 0, 0, alpha);

            nowTime += Time.deltaTime;

            yield return null;
        }
    }

    //성공했을때
    public override void OnJoinedRoom()
    {
        RoomJoinCancelButton();
        playRoomObj.SetActive(true);

        playRoomName.text = PhotonNetwork.CurrentRoom.Name;

        UpdatePlayerSlots();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerSlots();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerSlots();
    }

    private void UpdatePlayerSlots()
    {
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;

        for (int i = 0; i < playerCount; i++)
        {
            GameObject playerSlot = null;
            TMP_Text playerNameText = null;

            switch (i)
            {
                case 0:
                    playerSlot = player1Slot;
                    playerNameText = player1Slot.transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<TMP_Text>();
                    playerNum = PhotonNetwork.LocalPlayer.ActorNumber;
                    break;
                case 1:
                    playerSlot = player2Slot;
                    playerNameText = player2Slot.transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<TMP_Text>();
                    playerNum = PhotonNetwork.LocalPlayer.ActorNumber;
                    break;
                case 2:
                    playerSlot = player3Slot;
                    playerNameText = player3Slot.transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<TMP_Text>();
                    playerNum = PhotonNetwork.LocalPlayer.ActorNumber;
                    break;
                case 3:
                    playerSlot = player4Slot;
                    playerNameText = player4Slot.transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<TMP_Text>();
                    playerNum = PhotonNetwork.LocalPlayer.ActorNumber;
                    break;
            }

            if (playerSlot != null)
            {
                playerSlot.SetActive(true);

                playerNameText.text = PhotonNetwork.PlayerList[i].NickName;
            }
        }
    }

    private void Update()
    {

    }

    //방에서 나갈 때 버튼 액션
    public void RoomJoinOut()
    {
        RoomLeaveButton();
        PhotonNetwork.LeaveRoom();
    }

    //방을 떠났을때
    public override void OnLeftRoom()
    {
        //초기화
        for (int i = 0; i < 4; i++)
        {
            GameObject playerSlot = null;

            switch (i)
            {
                case 0:
                    playerSlot = player1Slot;
                    break;
                case 1:
                    playerSlot = player2Slot;
                    break;
                case 2:
                    playerSlot = player3Slot;
                    break;
                case 3:
                    playerSlot = player4Slot;
                    break;
            }

            if (playerSlot != null)
            {
                playerSlot.SetActive(false);

                TMP_Text playerNameText = playerSlot.transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<TMP_Text>();
                playerNameText.text = string.Empty;
            }
        }

        roomJoinPanel.SetActive(false);
    }

    public void PlayRoomGameStartButtonOnOff()
    {
        pv.RPC("PlayRoomGameStartButtonOnOffRPC", RpcTarget.AllBuffered, playerNum, !isReady);
        pv.RPC("PlayerReady", RpcTarget.AllBuffered, playerNum, isReady);
    }

    [PunRPC]
    public void PlayRoomGameStartButtonOnOffRPC(int playernum, bool readyStatus)
    {
        for (int i = 1; i <= PhotonNetwork.CurrentRoom.MaxPlayers; i++)
        {
            TMP_Text playReadyText = null;
            GameObject playReadyImage = null;

            switch (playernum)
            {
                case 1:
                    playReadyText = player1Slot.transform.GetChild(3).transform.GetChild(0).GetComponent<TMP_Text>();
                    playReadyImage = player1Slot.transform.GetChild(4).gameObject;
                    break;

                case 2:
                    playReadyText = player2Slot.transform.GetChild(3).transform.GetChild(0).GetComponent<TMP_Text>();
                    playReadyImage = player2Slot.transform.GetChild(4).gameObject;
                    break;

                case 3:
                    playReadyText = player3Slot.transform.GetChild(3).transform.GetChild(0).GetComponent<TMP_Text>();
                    playReadyImage = player3Slot.transform.GetChild(4).gameObject;
                    break;

                case 4:
                    playReadyText = player4Slot.transform.GetChild(3).transform.GetChild(0).GetComponent<TMP_Text>();
                    playReadyImage = player4Slot.transform.GetChild(4).gameObject;
                    break;
            }

            if (playReadyText != null && playReadyImage != null)
            {
                // 해당 플레이어의 UI만 업데이트
                isReady = readyStatus;
                playReadyText.text = readyStatus ? "준비 완료!" : "준비중...";

                if (isReady)
                {
                    playReadyImage.SetActive(true);
                }
                else
                {
                    playReadyImage.SetActive(false);
                }
            }
        }
    }

    [PunRPC]
    public void PlayerReady(int playerNum, bool isReady)
    {
        // 1은 레디, 0은 레디 해제
        readyCount[playerNum - 1] = isReady ? 1 : 0;

        // 모든 플레이어가 레디 상태인지 확인하는 함수 호출
        CheckAllPlayersReady();
    }

    private void CheckAllPlayersReady()
    {
        bool allPlayersReady = true;

        for (int i = 0; i < PhotonNetwork.CurrentRoom.MaxPlayers; i++)
        {
            if (readyCount[i] == 0)
            {
                allPlayersReady = false;
                break;
            }
        }

        if (allPlayersReady)
        {
            SceneSoundManager.instance.StopMusic();

            StartCoroutine(GameStartSceneMoving());
        }
    }

    IEnumerator GameStartSceneMoving()
    {
        playTouchPreventPanel.SetActive(true);
        HereWeGo.SetActive(true);

        RectTransform rectTransform = HereWeGo.GetComponent<RectTransform>();
        Vector3 originalPosition = rectTransform.anchoredPosition;

        // 흔들리는 강도, 시간 변수
        float shakeIntensity = 3f;
        float shakeTime = 2.5f;
        float elapsedTime = 0f;

        while (elapsedTime < shakeTime)
        {
            float x = originalPosition.x + UnityEngine.Random.Range(-shakeIntensity, shakeIntensity);
            float y = originalPosition.y + UnityEngine.Random.Range(-shakeIntensity, shakeIntensity);

            rectTransform.anchoredPosition = new Vector3(x, y, originalPosition.z);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        rectTransform.anchoredPosition = originalPosition;

        yield return new WaitForSeconds(1f);

        SceneSoundManager.instance.StageRunScene();

        SceneManager.LoadScene("Stage_Run");
    }


    //로비 게임종료 버튼
    public void gameQuit()
    {
        EffectSoundManager.instance.ButtonSound();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    #region Lobby, Room Button Action
    //게임 만들기 버튼
    public void RoomMakeButton()
    {
        touchPreventPanel.SetActive(true);
        roomMakePanel.SetActive(true);

        EffectSoundManager.instance.ButtonSound();
    }

    //취소
    public void RoomMakeCancelButton()
    {
        touchPreventPanel.SetActive(false);
        roomMakePanel.SetActive(false);

        //초기화
        roomName.text = string.Empty;
        roomPlayerCount.value = 0;

        EffectSoundManager.instance.ButtonSound();
    }

    //방 들어가기 버튼
    public void RoomJoinButton()
    {
        touchPreventPanel.SetActive(true);
        roomJoinPanel.SetActive(true);

        EffectSoundManager.instance.ButtonSound();
    }

    //취소
    public void RoomJoinCancelButton()
    {
        touchPreventPanel.SetActive(false);
        roomJoinPanel.SetActive(false);

        EffectSoundManager.instance.ButtonSound();
    }

    //방에서 나가기 버튼
    public void RoomLeaveButton()
    {
        touchPreventPanel.SetActive(false);
        playRoomObj.SetActive(false);

        EffectSoundManager.instance.ButtonSound();
        SceneSoundManager.instance.LobbyScene();
    }

    //게임 종료 버튼
    public void GameQuitButton()
    {
        touchPreventPanel.SetActive(true);
        gameQuitPanel.SetActive(true);

        EffectSoundManager.instance.ButtonSound();
    }

    //취소
    public void GameQuitCancelButton()
    {
        touchPreventPanel.SetActive(false);
        gameQuitPanel.SetActive(false);

        EffectSoundManager.instance.ButtonSound();
    }
    #endregion

    public void Disconnect() => PhotonNetwork.Disconnect();

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("연결종료");
    }

    //테스트용 버튼 액션
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

    public void TestSceneMove()
    {
        SceneManager.LoadScene("Test");
    }
}
