using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    static GameObject _gameobject = null;
    static SceneManager single = null;
    public static SceneManager Instance
    {
        get
        {
            if (!single)
            {
                _gameobject = new GameObject("SceneManager");
                single = _gameobject.AddComponent<SceneManager>();
                single.Initialize();
            }

            return single;
        }

        private set { }
    }

    void Initialize()
    {
        DontDestroyOnLoad(gameObject);
    }

    Coroutine m_LoadingCoroutine;
    public LoadingManager m_LoadingManager;

    public void LoadScene(string _SceneName)
    {
        if (m_LoadingCoroutine != null) StopCoroutine(m_LoadingCoroutine);

        UnityEngine.SceneManagement.SceneManager.LoadScene("LoadingScene");
        m_LoadingCoroutine = StartCoroutine(C_Loading(_SceneName));
    }

    public void LoadSceneDirect(string _SceneName)
    {
        if (m_LoadingCoroutine != null) StopCoroutine(m_LoadingCoroutine);
        UnityEngine.SceneManagement.SceneManager.LoadScene(_SceneName);
    }

    IEnumerator C_Loading(string _NextSceneName)
    {
        yield return new WaitForSeconds(3.0f);

        while (m_LoadingManager == null) yield return null;

        AsyncOperation Op = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(_NextSceneName);
        Op.allowSceneActivation = false;

        float progress = 0.0f;
        bool networkconnet = false;
        while(!Op.isDone)
        {
            if (!networkconnet)
            {
                if (Photon.Pun.PhotonNetwork.IsConnectedAndReady && (Photon.Pun.PhotonNetwork.InLobby || Photon.Pun.PhotonNetwork.InRoom))
                {
                    networkconnet = true;
                    progress += 0.4f;
                }
            }
            
            float fullprogress = ((Op.progress / 0.9f) * 0.6f) + progress;
            m_LoadingManager.SetValue(fullprogress);
            if (fullprogress >= 1.0f)
            {
                m_LoadingManager.SetValue(1.0f);
                yield return new WaitForSeconds(1.0f);
                Op.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}
