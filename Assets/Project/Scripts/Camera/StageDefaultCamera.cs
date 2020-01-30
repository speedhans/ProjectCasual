using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageDefaultCamera : MonoBehaviour
{
    static public StageDefaultCamera Instance;

    [SerializeField]
    UnityEngine.UI.Image m_ProgressCircle;
    [SerializeField]
    float m_RotateSpeed;
    [SerializeField]
    TMPro.TMP_Text m_Text;
    [SerializeField]
    Gradient m_TextColorGradient;
    [SerializeField]
    float m_ColorChangingSpeed;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        float deltatime = Time.deltaTime;
        Vector3 rot = m_ProgressCircle.transform.eulerAngles;
        rot.z += deltatime * m_RotateSpeed;
        if (rot.z > 360.0f)
            rot.z -= 360.0f;

        m_ProgressCircle.transform.eulerAngles = rot;

        m_Text.color = m_TextColorGradient.Evaluate(Mathf.Cos(Time.time * m_ColorChangingSpeed) + 0.45f);
    }

    public void DestroyCamera()
    {
        Destroy(gameObject);
    }
}
