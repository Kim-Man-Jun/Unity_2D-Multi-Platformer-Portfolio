using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boxGimmick : MonoBehaviour
{
    public float bouncePower = 30f;

    private Animator boxAnim;

    public PhotonView pv;

    private void Start()
    {
        boxAnim = GetComponent<Animator>();

        boxAnim.SetBool("Idle", true);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();

            if (playerRb != null)
            {
                playerRb.velocity = new Vector2(playerRb.velocity.x, bouncePower);
            }

            if (boxAnim != null)
            {
                boxAnim.SetBool("Idle", false);
                boxAnim.SetBool("Jump", true);
            }

            pv.RPC("jumpPadSound", RpcTarget.All);
        }
    }

    [PunRPC]
    private void jumpPadSound()
    {
        EffectSoundManager.instance.JumpPadSound();
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            if (boxAnim != null)
            {
                boxAnim.SetBool("Jump", false);
                boxAnim.SetBool("Idle", true);
            }
        }
    }
}
