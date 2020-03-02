using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static GameObject _gameobject = null;
    static GameManager single = null;
    public static GameManager Instance
    {
        get 
        {
            if (!single)
            {
                _gameobject = new GameObject("GameManager");
                single = _gameobject.AddComponent<GameManager>();
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

    [HideInInspector]
    public bool m_TestMode = true;

    public Main m_Main;
    public PlayerData m_PlayerData;
    public PlayerCharacter m_MyCharacter;
    System.Action<Object> m_SkyEnvironmentEvent;

    public bool m_GameStop { get; private set; }

    public void CreatePlayerData()
    {
        m_PlayerData = new PlayerData();
        // 임시 데이터
        m_PlayerData.m_Name = "TestPlayer" + Random.Range(1,100).ToString();
        m_PlayerData.m_Level = 99;
        m_PlayerData.m_CurrentStamina = 20;
        m_PlayerData.m_CurrentExp = 5;
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void PauseGame()
    {
        m_GameStop = true;
        Time.timeScale = 0.0f;
    }

    public void StartGame()
    {
        m_GameStop = false;
        Time.timeScale = 1.0f;
    }

#region WorldTimer
    public enum E_TIMETYPE
    {
        MINUTE,
        HOUR,
        DAY,
    }

    int m_WorldTimeMinute;
    int m_WorldTimeHour;
    int m_WorldTimeDay;
    float m_PerSecondTimer;
    public int GetWorldTime(E_TIMETYPE _Type)
    {
        switch(_Type)
        {
            case E_TIMETYPE.MINUTE:
                return m_WorldTimeMinute;
            case E_TIMETYPE.HOUR:
                return m_WorldTimeHour;
            case E_TIMETYPE.DAY:
                return m_WorldTimeDay;
        }

        return 0;
    }

    IEnumerator C_WorldTimerUpdate()
    {
        while(true)
        {
            m_PerSecondTimer += Time.deltaTime;
            if (m_PerSecondTimer >= 1.0f)
            {
                m_PerSecondTimer = 0.0f;
                m_WorldTimeMinute++;
                if (m_WorldTimeMinute >= 60)
                {
                    m_WorldTimeMinute = 0;
                    m_WorldTimeHour++;
                    if (m_WorldTimeHour >= 24)
                    {
                        m_WorldTimeHour = 0;
                        m_WorldTimeDay++;
                    }
                }
            }
            yield return null;
        }
    }

#endregion WorldTimer

    public void AddSkyEnvironmentEvent(System.Action<Object> _Event) { m_SkyEnvironmentEvent += _Event; }
    public void SubSkyEnvironmentEvent(System.Action<Object> _Event) { m_SkyEnvironmentEvent -= _Event; }
    public void RunSkyEnvironmentEvent(Object _Self) { m_SkyEnvironmentEvent?.Invoke(_Self); }
}
