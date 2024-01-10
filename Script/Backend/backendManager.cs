using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using System;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class backendManager : MonoBehaviour
{
    private static backendManager _instance = null;

    public static backendManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new backendManager();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        Screen.SetResolution(960, 540, false);
    }

    void Start()
    {
        //서버 초기화
        var bro = Backend.Initialize(true);
    }

    public void GameSignUp(string id, string pw)
    {
        var bro = Backend.BMember.CustomSignUp(id, pw);

        if (bro.IsSuccess())
        {
            Debug.Log("회원가입 성공");
        }
        else
        {
            Debug.LogError("회원가입 실패");
        }
    }

    public void GameLogin(string id, string pw)
    {
        var bro = Backend.BMember.CustomLogin(id, pw);

        if (bro.IsSuccess())
        {
            //아이디 = 닉네임 설정
            Backend.BMember.UpdateNickname(id);

            Debug.Log("로그인 성공");
        }
        else
        {
            Debug.LogError("로그인 실패");
        }
    }

    public async Task GameLoginAsync(string id, string password)
    {
        await Task.Run(() =>
        {
            GameLogin(id, password);
        });
    }

    public void GameNicknameChange(string nickname)
    {
        var bro = Backend.BMember.UpdateNickname(nickname);

        if (bro.IsSuccess())
        {
            Debug.Log("닉네임 변경 성공");
        }
        else
        {
            Debug.LogError("닉네임 변경 실패");
        }
    }
}