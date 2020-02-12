using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameReadyCanvas : MonoBehaviour
{
    [SerializeField]
    TMPro.TMP_Text m_ReadyText;
    [SerializeField]
    TMPro.TMP_Text m_StartText;

    [SerializeField]
    float m_TextSpeed = 10.0f;
    public bool IsComplete { get; protected set; }

    public void Run()
    {
        StartCoroutine(C_Run());
    }

    IEnumerator C_Run()
    {
        RectTransform rectR = m_ReadyText.GetComponent<RectTransform>();
        while (true)
        {
            float deltatime = Time.deltaTime;
            Vector3 pos = rectR.localPosition;
            if (pos.x < 0.0f)
            {
                float minspeed = deltatime * m_TextSpeed;
                pos.x += Mathf.Clamp(Mathf.Abs(pos.x) * (deltatime * m_TextSpeed), minspeed, 999999.0f);
                rectR.localPosition = pos;
                if (pos.x >= 0.0f)
                    break;
            }
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        RectTransform rectS = m_StartText.GetComponent<RectTransform>();
        float startspeed = Mathf.Abs(rectR.position.x);
        while (true)
        {
            float deltatime = Time.deltaTime;
            Vector3 pos = rectS.localPosition;
            Vector3 posR = rectR.localPosition;
            if (pos.x < 0.0f)
            {
                float minspeed = deltatime * m_TextSpeed;
                float addX = Mathf.Clamp(Mathf.Abs(pos.x) * (deltatime * m_TextSpeed), minspeed, 999999.0f);
                pos.x += addX;
                posR.x += addX;
                rectS.localPosition = pos;
                rectR.localPosition = posR;
                if (pos.x >= 0.0f)
                    break;
            }
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        IsComplete = true;

        float timer = 1.0f;
        while (timer > 0.0f)
        {
            float deltatime = Time.deltaTime;
            timer += deltatime;
            Vector3 pos = rectS.localPosition;
            pos.x += deltatime * m_TextSpeed * startspeed;
            rectS.localPosition = pos;
            yield return null;
        }
    }
}
