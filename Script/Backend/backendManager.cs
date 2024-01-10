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
        //���� �ʱ�ȭ
        var bro = Backend.Initialize(true);
    }

    public void GameSignUp(string id, string pw)
    {
        var bro = Backend.BMember.CustomSignUp(id, pw);

        if (bro.IsSuccess())
        {
            Debug.Log("ȸ������ ����");
        }
        else
        {
            Debug.LogError("ȸ������ ����");
        }
    }

    public void GameLogin(string id, string pw)
    {
        var bro = Backend.BMember.CustomLogin(id, pw);

        if (bro.IsSuccess())
        {
            //���̵� = �г��� ����
            Backend.BMember.UpdateNickname(id);

            Debug.Log("�α��� ����");
        }
        else
        {
            Debug.LogError("�α��� ����");
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
            Debug.Log("�г��� ���� ����");
        }
        else
        {
            Debug.LogError("�г��� ���� ����");
        }
    }
}