using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkyEnvironment : MonoBehaviour
{
    protected float m_Timer = 10.0f;

    protected abstract void Awake();

    protected virtual void Update()
    {
        if (m_Timer > 0.0f)
        {
            m_Timer -= Time.deltaTime;
            if (m_Timer <= 0.0f)
            {
                Destroy(gameObject);
            }
        }
    }

    public void SetTimer(float _Timer)
    {
        m_Timer = _Timer;
    }

    public abstract void EnvironmentEffect(Object _Self);

    protected abstract void OnDestroy();
}
