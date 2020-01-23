using System;
using System.Collections.Generic;
using UnityEngine;

public class SetRandomMoveLocation : ActionNode
{
    [SerializeField]
    public float m_SearchRadius;
    [SerializeField]
    public float m_MinDistance;

    public override bool BehaviorUpdate(Character _Self, float _DeltaTime)
    {
        float f = m_SearchRadius - m_MinDistance;
        if (f < 0.0f) f = 0.2f;

        Vector3 find = Vector3.zero;
        FindSample(_Self, 3, f, ref find);
        if (find == Vector3.zero) return false;

        _Self.MoveToLocation(find);
        return true;
    }

    public override void TickUpdate(Character _Self, float _DeltaTime)
    {
        
    }

    float CalculatePoint()
    {
        float f = UnityEngine.Random.Range(-m_SearchRadius, m_SearchRadius);
        if (f >= 0.0f)
        {
            f *= (1.0f - (m_MinDistance / m_SearchRadius));
            f += m_MinDistance;
        }
        else
        {
            f *= -(1.0f - (m_MinDistance / m_SearchRadius));
            f -= m_MinDistance;
        }

        return f;
    }

    void FindSample(Character _Self, int _Count, float _Radius, ref Vector3 _Out)
    {
        if (_Count == 0) return;

        float x = CalculatePoint();
        float y = CalculatePoint();
        float z = CalculatePoint();

        Vector3 find = Vector3.zero;
        _Self.FindNavMeshSampleLocation(_Self.transform.position + new Vector3(x, y, z), _Radius);

        if (find == Vector3.zero)
        {
            FindSample(_Self, _Count - 1, _Radius, ref _Out);
        }
        else
        {
            _Out = find;
        }
    }
}
