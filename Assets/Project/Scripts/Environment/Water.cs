using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Water : MonoBehaviour
{
    Rigidbody m_Rigid;

    [SerializeField]
    float m_IceDamage;
    int[] m_DamageType = new int[1] { (int)E_DAMAGETYPE.ICE };

    [SerializeField]
    float m_LifeTime;
    float m_LifeTimer;

    private void Awake()
    {
        m_Rigid = GetComponent<Rigidbody>();
        m_LifeTimer = m_LifeTime;
    }

    public void Initialize()
    {
        m_LifeTimer = m_LifeTime;
    }

    public Rigidbody GetRigidbody() { return m_Rigid; }

    private void OnTriggerEnter(Collider other)
    {
        Object o = other.GetComponentInParent<Object>();
        if (o)
        {
            o.GiveToDamage(new float[] { m_IceDamage }, m_DamageType, null);
            ObjectPool.PushObject(gameObject);
        }
    }

    private void Update()
    {
        m_LifeTimer -= Time.deltaTime;
        if (m_LifeTimer <= 0.0f)
        {
            ObjectPool.PushObject(gameObject);
        }
    }
}
