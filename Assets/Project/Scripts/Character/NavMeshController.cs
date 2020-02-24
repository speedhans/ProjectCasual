using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NavMeshController
{
    static Vector3 fwdRight = new Vector3(0.7f, 0.0f, 0.7f);
    static Vector3 fwdLeft = new Vector3(-0.7f, 0.0f, 0.7f);
    static Vector3 backRight = new Vector3(0.7f, 0.0f, -0.7f);
    static Vector3 backLeft = new Vector3(-0.7f, 0.0f, -0.7f);

    Character m_Character;

    [System.Serializable]
    public struct S_DynamicObjectCheckData
    {
        public int m_CheckLineCount;
        public float m_CheckLineLength;
        public float m_CheckRadius;
        public LayerMask m_AvoidLayer;
    }

    [SerializeField]
    protected S_DynamicObjectCheckData m_DynamicObjectAvoidedData;

    public bool m_UpdateRotate = true;
    public Vector3 m_MoveLocation;
    List<Vector3> m_ListNavPath = new List<Vector3>();
    List<Vector3> m_ListAvoidNavPath = new List<Vector3>();

    public void Initialize(Character _Character)
    {
        m_Character = _Character;
    }

    public void UpdateTransform(Transform _Transform, float _MovePerSpeed, float _RotatePerSpeed)
    {
        if (m_ListNavPath.Count == 0 && m_ListAvoidNavPath.Count == 0) return;

        float deltatime = Time.deltaTime;
        Vector3 dir = Vector3.zero;
        if (m_ListAvoidNavPath.Count > 0) 
            dir = m_ListAvoidNavPath[0] - _Transform.position;
        else
            dir = m_ListNavPath[0] - _Transform.position;

        Vector3 dirnormal = dir.normalized;
        float speed = deltatime * _MovePerSpeed;
        Vector3 nextpos = _Transform.position + dirnormal * speed;

        if (m_DynamicObjectAvoidedData.m_CheckLineCount > 1 && m_ListAvoidNavPath.Count < 10)
        {
            float weight = DynamicObjectCheck(_Transform, dirnormal);
            if (weight != 0.0f)
            {
                float constantweight = Mathf.Abs(weight);
                Vector3 v = new Vector3(0.0f, 0.0f, 1.0f - constantweight) + new Vector3(weight, 0.0f, 0.0f);
                Quaternion r = Quaternion.LookRotation(dirnormal, Vector3.up);
                List<Vector3> list = GetNavPath(_Transform.position, _Transform.position + ((r * v) * speed * (2.0f + (2 * constantweight))));
                if (list != null && list.Count > 0)
                {
                    m_ListAvoidNavPath.Clear();
                    m_ListAvoidNavPath.AddRange(list);
                
                    dir = m_ListAvoidNavPath[0] - _Transform.position;
                    dirnormal = dir.normalized;
                    speed = deltatime * _MovePerSpeed;
                    nextpos = _Transform.position + dirnormal * speed;
                }
            }
        }

        Vector3 Euler = _Transform.rotation.eulerAngles;
        Euler.x = 0.0f;
        Euler.z = 0.0f;
        if (m_UpdateRotate)
        {
            float addrotY = deltatime * _RotatePerSpeed;
            float dot = Vector3.Dot(_Transform.right, dirnormal);
            Vector3 dirRot = Quaternion.LookRotation(dirnormal).eulerAngles;
            if (Mathf.Abs(dirRot.y - Euler.y) > addrotY)
            {
                if (dot < 0.0f)
                    Euler.y -= addrotY;
                else
                    Euler.y += addrotY;
            }
            else
            {
                Euler.y = dirRot.y;
            }

            Euler.y = CustomMath.CorrectionDegree(Euler.y);
        }

        if (dir.magnitude > speed)
            _Transform.SetPositionAndRotation(nextpos, Quaternion.Euler(Euler));
        else
        {
            if (m_ListAvoidNavPath.Count > 0)
            {
                _Transform.SetPositionAndRotation(m_ListAvoidNavPath[0], Quaternion.Euler(Euler));
                m_ListAvoidNavPath.RemoveAt(0);
            }
            else
            {
                _Transform.SetPositionAndRotation(m_ListNavPath[0], Quaternion.Euler(Euler));
                m_ListNavPath.RemoveAt(0);
            }
        }
    }

    public void SetMoveLocation(Vector3 _CurrentLocation, Vector3 _TargetLocation)
    {
        m_ListNavPath.Clear();

        m_MoveLocation = FindNavMeshSampleLocation(_TargetLocation);
        m_ListNavPath.AddRange(GetNavPath(_CurrentLocation, _TargetLocation));
    }

    public Vector3 FindNavMeshSampleLocation(Vector3 _Point, float _SearchRadius = 0.2f)
    {
        UnityEngine.AI.NavMeshHit NavHit;
        if (UnityEngine.AI.NavMesh.SamplePosition(_Point, out NavHit, _SearchRadius, -1))
        {
            return NavHit.position;
        }

        return Vector3.zero;
    }

    List<Vector3> GetNavPath(Vector3 _StartPoint, Vector3 _TargetPoint)
    {
        List<Vector3> list = new List<Vector3>();
        UnityEngine.AI.NavMeshPath Path = new UnityEngine.AI.NavMeshPath();

        if (UnityEngine.AI.NavMesh.CalculatePath(_StartPoint, _TargetPoint, -1, Path))
        {
            if (Path.corners.Length > 1)
            {
                for (int i = 1; i < Path.corners.Length; ++i)
                {
                    list.Add(Path.corners[i]);
                }
            }
            else
            {
                list.Add(Path.corners[0]);
            }
        }

        return list;
    }

    public void ClearPath()
    {
        m_ListAvoidNavPath.Clear();
        m_ListNavPath.Clear();
    }

    public bool IsUpdate()
    {
        if (m_ListNavPath.Count == 0)
            return false;

        return true;
    }

    public int GetAvoidCheckLineCount() { return m_DynamicObjectAvoidedData.m_CheckLineCount; }
    public float GetAvoidCheckLineLength() { return m_DynamicObjectAvoidedData.m_CheckLineLength; }

    float DynamicObjectCheck(Transform _SourceTransform, Vector3 _Direction) // 양수가 right
    {
        _Direction.y = 0.0f;
        _Direction.Normalize();
        Quaternion calRot = Quaternion.LookRotation(_Direction, Vector3.up);
        Vector3 rightvector = calRot * Vector3.right;
        rightvector.Normalize();
        Vector3 leftvector3 = -rightvector;
        float offet = (m_DynamicObjectAvoidedData.m_CheckRadius * 2.0f) / (m_DynamicObjectAvoidedData.m_CheckLineCount - 1);
        float[] distance = new float[m_DynamicObjectAvoidedData.m_CheckLineCount];
        Vector3 startpos = _SourceTransform.position + new Vector3(0.0f, 0.5f, 0.0f) + (leftvector3 * m_DynamicObjectAvoidedData.m_CheckRadius);
        Vector3 addvector = offet * rightvector;
        RaycastHit rayhit;
        int hit = 0;
        float alllength = 0.0f;
        for (int i = 0; i < distance.Length; ++i)
        {
            if (Physics.Raycast(startpos, _Direction, out rayhit, m_DynamicObjectAvoidedData.m_CheckLineLength, m_DynamicObjectAvoidedData.m_AvoidLayer))
            {
                Character target = rayhit.transform.GetComponentInParent<Character>();
                if (target && target.m_Team == m_Character.m_Team)
                {
                    ++hit;
                    distance[i] = (startpos - rayhit.point).magnitude;
                    Debug.DrawLine(startpos, rayhit.point, Color.red);
                }
                else
                {
                    distance[i] = m_DynamicObjectAvoidedData.m_CheckLineLength;
                    Debug.DrawLine(startpos, startpos + (_Direction * m_DynamicObjectAvoidedData.m_CheckLineLength), Color.green);
                }
            }
            else
            {
                distance[i] = m_DynamicObjectAvoidedData.m_CheckLineLength;
                Debug.DrawLine(startpos, startpos + (_Direction * m_DynamicObjectAvoidedData.m_CheckLineLength), Color.green);
            }
            alllength += distance[i];
            startpos += addvector;
        }

        int halfcount = (int)(m_DynamicObjectAvoidedData.m_CheckLineCount * 0.5f);
        float weight = 0.0f;

        for (int i = 0; i < halfcount; ++i)
        {
            weight -= (distance[i] / m_DynamicObjectAvoidedData.m_CheckLineLength) / halfcount;
        }
        for (int i = distance.Length - 1; i >= halfcount; --i)
        {
            weight += (distance[i] / m_DynamicObjectAvoidedData.m_CheckLineLength) / halfcount;
        }

        float percent = alllength / (m_DynamicObjectAvoidedData.m_CheckLineCount * m_DynamicObjectAvoidedData.m_CheckLineLength);
        if (percent > 0.5f)
        {
            weight = weight * (1.0f + weight) * (1.0f + percent);
        }

        return weight;
    }
}
