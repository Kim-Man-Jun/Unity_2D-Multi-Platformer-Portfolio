using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(AudioSource))]
public class EffectSoundManager : MonoBehaviourPunCallbacks
{
    public static EffectSoundManager instance;

    AudioSource Audio;

    [Header("local")]
    public AudioClip button;

    [Header("Multi")]
    public AudioClip jump;
    public AudioClip jumpPad;
    public AudioClip spikedBall;
    public AudioClip dead;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Audio = GetComponent<AudioSource>();
    }

    public void ButtonSound()
    {
        Audio.PlayOneShot(button);
    }

    public void JumpSound()
    {
        Audio.PlayOneShot(jump);
    }

    public void JumpPadSound()
    {
        Audio.PlayOneShot(jumpPad);
    }

    public void SpikedBallSound()
    {
        Audio.PlayOneShot(spikedBall);
    }

    public void DeadSound()
    {
        Audio.PlayOneShot(dead);
    }
}
