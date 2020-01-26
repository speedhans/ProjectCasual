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

    public LoadingManager m_LoadingManager;

    public void LoadScene(string _SceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("LoadingScene");
        StartCoroutine(C_Loading(_SceneName));
    }

    IEnumerator C_Loading(string _NextSceneName)
    {
        while (m_LoadingManager == null) yield return null;

        AsyncOperation Op = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(_NextSceneName);

        while(!Op.isDone)
        {
            m_LoadingManager.SetValue(Op.progress);
            if (Op.progress >= 0.9f)
            {
                m_LoadingManager.SetValue(1.0f);
                yield return new WaitForSeconds(1.0f);
                Op.allowSceneActivation = true;
            }
        }
    }
}
