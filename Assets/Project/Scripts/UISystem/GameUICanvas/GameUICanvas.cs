using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUICanvas : DefaultUI
{
    Main_Stage m_Main;

    [SerializeField]
    GameMenuUI m_GameMenuUI;
    [SerializeField]
    float m_GameMenuUIAnimationSpeed;
    bool m_GameMenuUIActivate = false;
    Coroutine m_Coroutine;
    [SerializeField]
    GameDeadUI m_GameDeadUI;
    [SerializeField]
    TMPro.TMP_Text m_QuestNameText;
    [SerializeField]
    TMPro.TMP_Text m_TimeText;
    float m_Time = 0.0f;
    bool m_IsTimeCheck = false;
    public void Initialize(Main_Stage _Main, List<Item> _DropList)
    {
        m_Main = _Main;
        m_QuestNameText.text = m_Main.m_GameSceneData.m_StageName;
        m_GameMenuUI.Initialize(_DropList);
        m_GameDeadUI.Initialize();
    }

    private void Update()
    {
        if (!m_IsTimeCheck) return;
        m_Time += Time.deltaTime;
        m_TimeText.text = ((int)(m_Time / 60)).ToString() + ":" + ((int)(m_Time % 60)).ToString();
    }

    public void TimeCheck(bool _Enable)
    {
        m_IsTimeCheck = _Enable;
    }

    public void MenuButton()
    {
        if (m_GameMenuUIActivate)
        {
            if (m_Coroutine != null)
                StopCoroutine(m_Coroutine);
            m_Coroutine = StartCoroutine(C_MenuCloseAnimation());
        }
        else
        {
            if (m_Coroutine != null)
                StopCoroutine(m_Coroutine);
            m_Coroutine = StartCoroutine(C_MenuOpenAnimation());
        }
    }
    
    IEnumerator C_MenuOpenAnimation()
    {
        if (!m_Main.m_IsMultiplayGame)
            GameManager.Instance.PauseGame();
        m_GameMenuUIActivate = true;
        RectTransform t = m_GameMenuUI.GetComponent<RectTransform>();
        m_GameMenuUI.gameObject.SetActive(true);
        while (true)
        {
            float deltatime = Time.unscaledDeltaTime;

            Vector3 pos = t.localPosition;
            pos.x += deltatime * m_GameMenuUIAnimationSpeed;

            if (pos.x >= 0.0f)
            {
                pos.x = 0.0f;
                t.localPosition = pos;
                yield break;
            }

            t.localPosition = pos;
            yield return null;
        }
    }

    IEnumerator C_MenuCloseAnimation()
    {
        if (!m_Main.m_IsMultiplayGame)
            GameManager.Instance.StartGame();
        m_GameMenuUIActivate = false;
        RectTransform t = m_GameMenuUI.GetComponent<RectTransform>();

        while (true)
        {
            float deltatime = Time.unscaledDeltaTime;

            Vector3 pos = t.localPosition;
            pos.x -= deltatime * m_GameMenuUIAnimationSpeed;

            if (pos.x <= -500.0f)
            {
                pos.x = -500.0f;
                t.localPosition = pos;
                m_GameMenuUI.gameObject.SetActive(false);
                yield break;
            }

            t.localPosition = pos;
            yield return null;
        }
    }

    public float GetTime() { return m_Time; }
    public GameDeadUI GetDeadUI() { return m_GameDeadUI; }
}
