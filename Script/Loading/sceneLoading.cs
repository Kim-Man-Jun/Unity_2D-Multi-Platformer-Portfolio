using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class sceneLoading : MonoBehaviour
{
    private static sceneLoading _instance = null;

    public static sceneLoading Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new sceneLoading();
            }
            return _instance;
        }
    }

    public string nextScene;
    public Slider loadingGauge;

    private void Start()
    {
        StartCoroutine(LoadNextSceneAsync());
    }

    IEnumerator LoadNextSceneAsync()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nextScene);

        float delay = 2f;

        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);

            loadingGauge.value = progress;

            yield return new WaitForSeconds(delay);
        }
    }
}
