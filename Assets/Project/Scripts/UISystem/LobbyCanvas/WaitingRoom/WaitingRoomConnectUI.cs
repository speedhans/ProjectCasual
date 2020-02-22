using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingRoomConnectUI : DefaultUI
{
    [SerializeField]
    UnityEngine.UI.Image m_ProgressCircle;
    [SerializeField]
    float m_CircleRotateSpeed;
    [SerializeField]
    TMPro.TMP_Text m_Text;
    [SerializeField]
    Gradient m_TextColorGradient;
    [SerializeField]
    float m_ColorChangingSpeed;

    // Update is called once per frame
    void Update()
    {
        float deltatime = Time.deltaTime;
        Vector3 rot = m_ProgressCircle.transform.eulerAngles;
        rot.z += deltatime * m_CircleRotateSpeed;
        if (rot.z > 360.0f)
            rot.z -= 360.0f;

        m_ProgressCircle.transform.eulerAngles = rot;

        m_Text.color = m_TextColorGradient.Evaluate(Mathf.Cos(Time.time * m_ColorChangingSpeed) + 0.45f);
    }
}
