using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class ASFlameBlastSub : MonoBehaviour
{
    PhotonView m_PhotonView;

    Character m_Caster;
    float m_Damage;
    E_DAMAGETYPE m_DamageType;

    [SerializeField]
    float m_MaxRange = 10.0f;
    [SerializeField]
    float m_Speed = 10.0f;

    GameObject m_TrailEffect;
    GameObject m_BombEffect;

    Ray m_Ray = new Ray();
    RaycastHit m_RayHit;
    Vector3 m_BeforePosition;

    private void Awake()
    {
        m_PhotonView = GetComponent<PhotonView>();
        m_TrailEffect = transform.GetChild(0).gameObject;
        m_BombEffect = transform.GetChild(1).gameObject;
    }

    public void Initialize(Character _Caster, float _Damage, E_DAMAGETYPE _DamageType)
    {
        m_Caster = _Caster;
        m_Damage = _Damage;
        m_DamageType = _DamageType;
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_TrailEffect.activeSelf) return;

        float deltatime = Time.deltaTime;
        float range = m_Speed * deltatime;
        m_MaxRange -= range;

        if (m_MaxRange <= 0.0f)
        {
            Bomb();
            return;
        }

        m_Ray.origin = transform.position;
        m_Ray.direction = transform.forward;

        if (Physics.Raycast(m_Ray, out m_RayHit, range, 1 << Common.STATIC_LAYER))
        {
            Bomb();
            return;
        }

        transform.position += m_Ray.direction * range;
    }

    void Bomb()
    {
        if (!m_TrailEffect.activeSelf) return;

        m_TrailEffect.SetActive(false);
        m_BombEffect.SetActive(true);
        Invoke("Destroy", 1.5f);
        if (!m_PhotonView.IsMine) return;

        List<Character> list = Character.FindEnemyAllArea(m_Caster, transform.position, 3.0f);
        if (list != null)
        {
            float[] damage = new float[] { m_Damage };
            int[] type = new int[] { (int)m_DamageType };
            foreach (Character c in list)
            {
                c.GiveToDamage(damage, type, m_Caster);
            }
        }
    }

    private void Destroy()
    {
        if (m_PhotonView.IsMine)
            PhotonNetwork.Destroy(gameObject);
    }
}
