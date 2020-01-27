using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalFollowCamera : CameraSystem
{
    static VerticalFollowCamera Instance = null;

    [HideInInspector]
    public Object m_Target;

    public float m_LocalPositionY;
    public float m_LocalPositionZ;
    float m_LocalPositionX;

    bool m_CameraWave = false;

    Coroutine m_CameraWaveCort;

    protected override void Awake()
    {
        Instance = this;
        base.Awake();
        StartCoroutine(C_Initialize());
    }

    IEnumerator C_Initialize()
    {
        Main main = null;
        while (!main)
        {
            main = GameManager.Instance.m_Main;
            yield return null;
        }
        main.m_Camera = this;

        while (m_Target == null)
        {
            m_Target = GameManager.Instance.m_MyCharacter;
            yield return null;
        }

        m_LocalPositionX = m_Target.transform.position.x;
        Vector3 tpos = m_Target.transform.position;
        Vector3 pos = m_Camera.transform.position;
        Vector3 fixedpos = new Vector3(pos.x, tpos.y + m_LocalPositionY, tpos.z + m_LocalPositionZ);
        m_Camera.transform.position = fixedpos;
        m_Camera.transform.forward = m_Target.transform.position - fixedpos;
    }

    private void Update()
    {
        if (!m_Target) return;
        if (m_CameraWave) return;

        Vector3 tpos = m_Target.transform.position;
        Vector3 pos = m_Camera.transform.position;
        Vector3 fixedpos = new Vector3(pos.x, tpos.y + m_LocalPositionY, tpos.z + m_LocalPositionZ);
        m_Camera.transform.position = fixedpos;
    }

    static public void CameraWave(float _Duration)
    {
        Instance.VerticalCameraWave(_Duration);
    }

    static public Transform GetTransform() { return Instance.transform; }

    protected void VerticalCameraWave(float _Duration)
    {
        m_CameraWave = true;
        if (m_CameraWaveCort != null)
            StopCoroutine(m_CameraWaveCort);
        m_CameraWaveCort = StartCoroutine(C_CameraWave(_Duration));
    }

    IEnumerator C_CameraWave(float _Duration)
    {
        Vector3 fwd = m_Camera.transform.forward;
        float timer = _Duration;
        while(timer > 0.0f)
        {
            m_Camera.transform.position += (fwd * Mathf.Sin(timer * 100.0f) * 0.2f);
            timer -= Time.deltaTime;
            yield return null;
        }

        Vector3 tpos = m_Target.transform.position;
        Vector3 pos = m_Camera.transform.position;
        Vector3 fixedpos = new Vector3(m_LocalPositionX, tpos.y + m_LocalPositionY, tpos.z + m_LocalPositionZ);
        m_Camera.transform.position = fixedpos;

        m_CameraWave = false;
    }
}
