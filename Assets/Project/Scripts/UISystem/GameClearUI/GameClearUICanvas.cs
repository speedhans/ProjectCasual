using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameClearUICanvas : MonoBehaviour
{
    Main_Stage m_Main;

    [SerializeField]
    TMPro.TMP_Text m_ClearText;

    [SerializeField]
    Gradient m_TextGradientColor;
    [SerializeField]
    float m_ColorPointOffset;
    float m_ColorPointAdd;
    [SerializeField]
    float m_ColorChangeSpeed;

    Coroutine m_Coroutine;

    [SerializeField]
    ResultUI m_ResultUI;

    public void Initialize(Main_Stage _Main)
    {
        m_Main = _Main;

        m_ColorPointAdd = 0.0f;
    }

    public void StartClearTextAnimation()
    {
        m_Coroutine = StartCoroutine(C_ClearTextColorUpdate());
    }

    IEnumerator C_ClearTextColorUpdate()
    {
        while(true)
        {
            float deltatime = Time.deltaTime;
            m_ColorPointAdd = LoopValue01(m_ColorPointAdd + (deltatime * m_ColorChangeSpeed));

            TMPro.VertexGradient vtxgradient;
            vtxgradient.topLeft = m_TextGradientColor.Evaluate(LoopValue01(m_ColorPointAdd + (m_ColorPointOffset * 1)));
            vtxgradient.topRight = m_TextGradientColor.Evaluate(LoopValue01(m_ColorPointAdd + (m_ColorPointOffset * 2)));
            vtxgradient.bottomLeft = m_TextGradientColor.Evaluate(LoopValue01(m_ColorPointAdd + (m_ColorPointOffset * 3)));
            vtxgradient.bottomRight = m_TextGradientColor.Evaluate(LoopValue01(m_ColorPointAdd + (m_ColorPointOffset * 4)));

            m_ClearText.colorGradient = vtxgradient;

            yield return null;
        }
    }

    float LoopValue01(float _Value)
    {
        if (_Value > 1.0f)
            return 0.0f;
        else if (_Value < 0.0f)
            return 1.0f;
        return _Value;
    }

    public void TouchBackground()
    {
        if (m_ResultUI.m_Phase == 0)
        {
            if (m_Coroutine != null)
            {
                StopCoroutine(m_Coroutine);
            }

            Item[] items = m_Main.GetResultItemlist();
            List<Item> list = new List<Item>();
            list.AddRange(items);
            m_ResultUI.Initialize(Random.Range(10000, 100000), list); // db 작업 필요
            m_ResultUI.gameObject.SetActive(true);
            m_ResultUI.StartScoreAnimation();
            return;
        }

        if (m_ResultUI.m_Phase == 1 || m_ResultUI.m_Phase == 2)
        {
            m_ResultUI.DirectResult();
            return;
        }

        if (m_ResultUI.m_Phase > 2)
        {
            SceneManager.Instance.LoadScene("LobbyScene");
            return;
        }
    }
}
