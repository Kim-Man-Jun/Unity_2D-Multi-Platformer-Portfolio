using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(AudioSource))]
public class SceneSoundManager : MonoBehaviourPunCallbacks
{
    public static SceneSoundManager instance;

    AudioSource Audio;

    public AudioClip login;
    public AudioClip lobby;
    public AudioClip room;
    public AudioClip stage_Run;
    public AudioClip Win;

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
        LoginScene();
    }

    public void LoginScene()
    {
        Audio.clip = login;
        Audio.loop = true;

        Audio.Play();
    }

    public void LobbyScene()
    {
        Audio.clip = lobby;
        Audio.loop = true;

        Audio.Play();
    }

    public void RoomDisplay()
    {
        Audio.clip = room;
        Audio.loop = true;

        Audio.Play();
    }

    public void StageRunScene()
    {
        Audio.clip = stage_Run;
        Audio.loop = true;

        Audio.Play();
    }

    public void WinDisplay()
    {
        Audio.clip = Win;
        Audio.loop = true;

        Audio.Play();
    }

    public void StopMusic()
    {
        Audio.Stop();
    }
}
