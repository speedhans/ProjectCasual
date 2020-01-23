using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{

    [SerializeField]
    float m_LifeTimer;

    [SerializeField]
    Vector3 m_MoveDirection;
    [SerializeField]
    float m_MoveDirectionRandomRange;
    Vector3 m_AddVelocity;
    [SerializeField]
    bool m_UseGravity;
    [SerializeField]
    [Range(0.01f, 10.0f)]
    float m_GravitySpeed = 1.0f;
    float m_GravityAccumulate = 0.0f;

    [SerializeField]
    bool m_Fade;
    [SerializeField]
    float m_FadeSpeed = 1.0f;

    Camera m_MainCamera;
    TMPro.TMP_Text m_Text;
    Color m_OriColor;
    private void Awake()
    {
        m_MainCamera = Camera.main;
        m_Text = GetComponentInChildren<TMPro.TMP_Text>();
        m_OriColor = m_Text.color;
    }

    public void Initialize(string _Text, Vector3 _Location, float _LifeTimer = 2.0f)
    {
        m_Text.text = _Text;
        m_LifeTimer = _LifeTimer;
        transform.position = _Location;
        m_GravityAccumulate = 0.0f;
        Color c = m_Text.color;
        m_Text.color = new Color(c.r,c.g,c.b, 1.0f);
        if (m_MoveDirectionRandomRange > 0.0f)
        {
            m_AddVelocity = new Vector3(m_MoveDirection.x + Random.Range(-m_MoveDirectionRandomRange, m_MoveDirectionRandomRange),
                m_MoveDirection.y + Random.Range(-m_MoveDirectionRandomRange, m_MoveDirectionRandomRange) - m_GravityAccumulate,
                m_MoveDirection.z + Random.Range(-m_MoveDirectionRandomRange, m_MoveDirectionRandomRange));
        }
    }

    public void SetColor(Color _Color)
    {
        m_Text.color = _Color;
    }

    public void ResetColor()
    {
        m_Text.color = m_OriColor;
    }

    // Update is called once per frame
    void Update()
    {
        float delatime = Time.deltaTime;
        if (m_LifeTimer > 0.0f)
        {
            m_LifeTimer -= delatime;
            if (m_LifeTimer <= 0.0f)
            {
                ObjectPool.PushObject(gameObject);
                return;
            }
        }

        if (!m_MainCamera) return;

        if (m_UseGravity && m_GravityAccumulate < 9.81f)
        {
            m_GravityAccumulate += delatime * m_GravitySpeed;
        }

        if (m_MoveDirectionRandomRange > 0.0f)
        {
            transform.position += m_AddVelocity * delatime + (Vector3.down * m_GravityAccumulate);
        }

        if (m_Fade)
        {
            Color c = m_Text.color;
            m_Text.color = new Color(c.r, c.g, c.b, c.a -= delatime * m_FadeSpeed);
        }

        transform.forward = m_MainCamera.transform.forward;
    }
}
