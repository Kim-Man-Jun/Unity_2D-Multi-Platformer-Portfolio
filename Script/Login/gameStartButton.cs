using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class gameStartButton : MonoBehaviourPunCallbacks
{
    private static gameStartButton _instance = null;

    public static gameStartButton Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new gameStartButton();
            }
            return _instance;

        }
    }

    [Header("Pop Up Panel")]
    public GameObject loginPanel;
    public GameObject signUpPanel;

    [Header("Log In Text")]
    public TMP_InputField logInID;
    public TMP_InputField logInPW;

    [Header("Sign Up Text")]
    public TMP_InputField signUpID;
    public TMP_InputField signUpPW;

    //패널 off
    private void Awake()
    {
        loginPanel.SetActive(false);
        signUpPanel.SetActive(false);
    }

    private void Start()
    {
        
    }

    //로그인 버튼 on
    public void loginPanelOn()
    {
        loginPanel.SetActive(true);
        EffectSoundManager.instance.ButtonSound();
    }

    //로그인 버튼 off
    public void loginPanelOff()
    {
        loginPanel.SetActive(false);

        logInID.text = string.Empty;
        logInPW.text = string.Empty;
        signUpID.text = string.Empty;
        signUpPW.text = string.Empty;

        EffectSoundManager.instance.ButtonSound();
    }

    //가입하기 버튼 on
    public void signUpPanelOn()
    {
        loginPanel.SetActive(false);
        signUpPanel.SetActive(true);

        EffectSoundManager.instance.ButtonSound();
    }

    //가입하기 버튼 off
    public void signUpPanelOff()
    {
        loginPanel.SetActive(true);
        signUpPanel.SetActive(false);

        logInID.text = string.Empty;
        logInPW.text = string.Empty;
        signUpID.text = string.Empty;
        signUpPW.text = string.Empty;

        EffectSoundManager.instance.ButtonSound();
    }

    public void logInButtonOn()
    {
        logInSync();

        EffectSoundManager.instance.ButtonSound();
    }

    async void logInSync()
    {
        await backendManager.Instance.GameLoginAsync(logInID.text, logInPW.text);

        SceneManager.LoadScene("Loading");
    }

    public void signUpButtonOn()
    {
        signUpSync();

        signUpPanelOff();

        EffectSoundManager.instance.ButtonSound();
    }

    async void signUpSync()
    {
        await Task.Run(() =>
        {
            backendManager.Instance.GameSignUp(signUpID.text, signUpPW.text);
        });
    }
}
