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

        m_TextText = GameObject.Find("TestText/Canvas/Text").GetComponent<TMPro.TMP_Text>();
    }

    [HideInInspector]
    public TMPro.TMP_Text m_TextText;

    public Main m_Main;
    public PlayerCharacter m_MyCharacter;
    System.Action<Object> m_SkyEnvironmentEvent;

    public bool m_GameStop = false;

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
    #endregion WorldTimer
    private void Update()
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
    }

    public void AddSkyEnvironmentEvent(System.Action<Object> _Event) { m_SkyEnvironmentEvent += _Event; }
    public void SubSkyEnvironmentEvent(System.Action<Object> _Event) { m_SkyEnvironmentEvent -= _Event; }
    public void RunSkyEnvironmentEvent(Object _Self) { m_SkyEnvironmentEvent?.Invoke(_Self); }
}
