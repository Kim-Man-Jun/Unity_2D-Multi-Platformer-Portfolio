using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using BackEnd;
using System;
using Cinemachine;
using System.Runtime.CompilerServices;

public class playerController : MonoBehaviourPunCallbacks, IPunObservable
{
    [Header("Player Component")]
    public Rigidbody2D rbody;
    public Animator anim;
    public SpriteRenderer sr;
    public PhotonView pv;

    [Header("Player Name")]
    public TMP_Text playerName;

    [Header("Player Move")]
    public float moveSpeed;

    [Header("Player Jump")]
    public float jumpForce;
    public Transform groundCheck;
    public float groundDistance;
    public LayerMask groundLayer;

    //애니메이션 관련
    string nowAnima;
    string oldAnima;

    Vector3 curPos;

    public float disappearTime = 2f;
    public bool playerDIe = false;
    public bool playerRevive = false;

    void Awake()
    {
        playerName.text = pv.IsMine ? Backend.UserNickName : pv.Owner.NickName;

        if (pv.IsMine)
        {
            var Cam = GameObject.Find("playerFollowCamera").GetComponent<CinemachineVirtualCamera>();

            Cam.Follow = this.gameObject.transform;
            Cam.LookAt = this.gameObject.transform;
        }
    }

    void Start()
    {
        nowAnima = "Idle";
        oldAnima = "Idle";
    }

    void Update()
    {
        if (pv.IsMine && stageManager.stageClear == false)
        {
            float axis = Input.GetAxisRaw("Horizontal");

            playerMoving(axis);
            pv.RPC("MovingRPC", RpcTarget.Others, axis);

            //Player Animation Controller
            if (axis == 0 && isGroundDetected() == true && playerDIe == false)
            {
                nowAnima = "Idle";
            }
            else if (axis != 0 && isGroundDetected() == true && playerDIe == false)
            {
                nowAnima = "Run";

                //좌우 플립
                playerFlip(axis);
                pv.RPC("FlipXRPC", RpcTarget.Others, axis);
            }
            else if (isGroundDetected() == false && playerDIe == false)
            {
                nowAnima = "Jump";

                anim.SetFloat("yVelocity", rbody.velocity.y);

                playerFlip(axis);
                pv.RPC("FlipXRPC", RpcTarget.Others, axis);
            }
            else if (playerDIe == true)
            {
                nowAnima = "Disappear";

                rbody.velocity = Vector3.zero;
            }
            else if (playerRevive == true)
            {
                nowAnima = "Appear";

                StartCoroutine(playerReviveOn());
            }

            if (nowAnima != oldAnima)
            {
                anim.SetBool(oldAnima, false);
                oldAnima = nowAnima;
                anim.SetBool(nowAnima, true);
            }

            //Player Jump
            if (Input.GetKeyDown(KeyCode.Space) && isGroundDetected() == true)
            {
                playerJump();
                pv.RPC("JumpRPC", RpcTarget.Others);
            }
        }

        //positon sync
        else if ((transform.position - curPos).sqrMagnitude >= 100) transform.position = curPos;
        else transform.position = Vector3.Lerp(transform.position, curPos, Time.deltaTime * 10);
    }

    IEnumerator playerReviveOn()
    {
        yield return new WaitForSeconds(1);

        playerRevive = false;
        nowAnima = "Idle";
    }

    #region playerMoving
    void playerMoving(float axis)
    {
        if (playerDIe == false)
        {
            rbody.velocity = new Vector2(moveSpeed * axis, rbody.velocity.y);
        }
    }

    [PunRPC]
    void MovingRPC(float axis)
    {
        playerFlip(axis);
    }
    #endregion

    #region playerFlip
    void playerFlip(float axis)
    {
        if (axis < 0)
        {
            sr.flipX = true;
        }
        else if (axis > 0)
        {
            sr.flipX = false;
        }
    }

    [PunRPC]
    void FlipXRPC(float axis)
    {
        playerFlip(axis);
    }
    #endregion

    #region playerJump
    void playerJump()
    {
        rbody.velocity = Vector2.zero;
        rbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        EffectSoundManager.instance.JumpSound();
    }

    //플레이어 점프
    [PunRPC]
    void JumpRPC()
    {
        playerJump();
    }
    #endregion

    public bool isGroundDetected() => Physics2D.Raycast(groundCheck.position,
    Vector2.down, groundDistance, groundLayer);

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x,
            groundCheck.position.y - groundDistance));

        Gizmos.color = Color.red;
    }

    //변수 동기화 진행
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Dead"))
        {
            StartCoroutine(DisappearAndRespawn());

            pv.RPC("DeadSound", RpcTarget.All);
        }
    }

    IEnumerator DisappearAndRespawn()
    {
        playerDIe = true;

        yield return new WaitForSeconds(disappearTime);

        // 처음 태어난 장소로 보내는 코드
        Respawn();
    }

    private void Respawn()
    {
        // 처음 태어난 장소로 플레이어를 이동시키는 코드
        if (PhotonNetwork.LocalPlayer.ActorNumber == 1)
        {
            Transform playerPos1 = GameObject.Find("player1Pos").transform;

            transform.position = playerPos1.position;

            playerDIe = false;
            playerRevive = true;
        }
        else if (PhotonNetwork.LocalPlayer.ActorNumber == 2)
        {
            Transform playerPos2 = GameObject.Find("player2Pos").transform;

            transform.position = playerPos2.position;

            playerDIe = false;
            playerRevive = true;
        }
        else if (PhotonNetwork.LocalPlayer.ActorNumber == 3)
        {
            Transform playerPos3 = GameObject.Find("player3Pos").transform;

            transform.position = playerPos3.position;

            playerDIe = false;
            playerRevive = true;
        }
        else if (PhotonNetwork.LocalPlayer.ActorNumber == 4)
        {
            Transform playerPos4 = GameObject.Find("player4Pos").transform;

            transform.position = playerPos4.position;

            playerDIe = false;
            playerRevive = true;
        }
    }

    [PunRPC]
    private void DeadSound()
    {
        EffectSoundManager.instance.DeadSound();
    }
}
