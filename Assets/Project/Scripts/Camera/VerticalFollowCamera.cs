using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalFollowCamera : CameraSystem
{
    static VerticalFollowCamera DefaultInstance = null;

    [HideInInspector]
    public Object m_Target;

    public float m_LocalPositionY;
    public float m_LocalPositionZ;
    public float m_AddDistanceX;
    public float m_AddDistanceZ;
    float m_LocalPositionX;

    bool m_CameraWave = false;

    Coroutine m_CameraWaveCort;
    Coroutine m_CameraDistanceChangeCort;

    protected override void Awake()
    {
        DefaultInstance = this;
        base.Awake();
    }

    public void Initialize(Main _Main, Object _CameraTarget)
    {
        _Main.m_Camera = this;
        m_Target = _CameraTarget;

        m_LocalPositionX = m_Target.transform.position.x;
        Vector3 tpos = m_Target.transform.position;
        Vector3 pos = m_Camera.transform.position;
        Vector3 fixedpos = new Vector3(pos.x, tpos.y + m_LocalPositionY + m_AddDistanceX, tpos.z + m_LocalPositionZ + m_AddDistanceZ);
        m_Camera.transform.position = fixedpos;
        m_Camera.transform.forward = m_Target.transform.position - fixedpos;
    }

    private void Update()
    {
        if (!m_Target) return;
        if (m_CameraWave) return;

        Vector3 tpos = m_Target.transform.position;
        Vector3 pos = m_Camera.transform.position;
        Vector3 fixedpos = new Vector3(pos.x, tpos.y + m_LocalPositionY + m_AddDistanceX, tpos.z + m_LocalPositionZ + m_AddDistanceZ);
        m_Camera.transform.position = fixedpos;
    }

    static public void CameraWave(float _Duration)
    {
        DefaultInstance.VerticalCameraWave(_Duration);
    }

    static public Transform GetTransform() { return DefaultInstance.transform; }
    static public void SetDistanceSmooth(float _Distance, float _PerSpeed)
    {
        DefaultInstance.SmoothDistanceChange(_Distance, _PerSpeed);
    }

    static public void SetDistance(float _Distance)
    {
        DefaultInstance.DistanceChange(_Distance);
    }

    protected void SmoothDistanceChange(float _Distance, float _PerSpeed)
    {
        if (m_CameraDistanceChangeCort != null) StopCoroutine(m_CameraDistanceChangeCort);
        m_CameraDistanceChangeCort = StartCoroutine(C_SmoothDistanceChange(_Distance, _PerSpeed));
    }

    IEnumerator C_SmoothDistanceChange(float _Distance, float _PerSpeed)
    {
        float speed = _PerSpeed;
        float progress = 0.0f;
        float currentx = m_AddDistanceX;
        float currentz = m_AddDistanceZ;

        Vector2 v2n = GetVector2Normalize(m_LocalPositionY, m_LocalPositionZ);

        float targetx = _Distance * v2n.x;
        float targetz = _Distance * v2n.y;
        while (progress < 1.0f)
        {
            progress = Mathf.Clamp01(progress + (Time.deltaTime * speed));
            m_AddDistanceX = Mathf.Lerp(currentx, targetx, progress);
            m_AddDistanceZ = Mathf.Lerp(currentz, targetz, progress);
            yield return null;
        }
    }

    protected void DistanceChange(float _Distance)
    {
        if (m_CameraDistanceChangeCort != null) StopCoroutine(m_CameraDistanceChangeCort);

        Vector2 v2n = GetVector2Normalize(m_LocalPositionY, m_LocalPositionZ);
        m_AddDistanceX = _Distance * v2n.x;
        m_AddDistanceZ = _Distance * v2n.y;
    }

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
        Vector3 fixedpos = new Vector3(pos.x, tpos.y + m_LocalPositionY + m_AddDistanceX, tpos.z + m_LocalPositionZ + m_AddDistanceZ);
        m_Camera.transform.position = fixedpos;

        m_CameraWave = false;
    }

    Vector2 GetVector2Normalize(float _Value1, float _Value2)
    {
        float length = Mathf.Sqrt((_Value1 * _Value1) + (_Value2 * _Value2));
        return new Vector2(_Value1, _Value2) / length;
    }
}
