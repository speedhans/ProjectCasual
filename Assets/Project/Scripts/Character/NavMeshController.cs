using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NavMeshController
{
    [SerializeField]
    float m_AvoidRadius;

    public bool m_UpdateRotate = true;
    public Vector3 m_MoveLocation;
    List<Vector3> m_ListNavPath = new List<Vector3>();

    public void UpdateTransform(Transform _Transform, float _MovePerSpeed, float _RotatePerSpeed)
    {
        if (m_ListNavPath.Count == 0) return;

        float deltatime = Time.deltaTime;
        Vector3 dir = m_ListNavPath[0] - _Transform.position;
        Vector3 dirnormal = dir.normalized;

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
        float speed = deltatime * _MovePerSpeed;
        if (dir.magnitude > speed)
            _Transform.SetPositionAndRotation(_Transform.position + dirnormal * speed, Quaternion.Euler(Euler));
        else
        {
            _Transform.SetPositionAndRotation(m_ListNavPath[0], Quaternion.Euler(Euler));
            m_ListNavPath.RemoveAt(0);
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
            for (int i = 1; i < Path.corners.Length; ++i)
            {
                list.Add(Path.corners[i]);
            }
        }

        return list;
    }

    public void ClearPath()
    {
        m_ListNavPath.Clear();
    }

    public bool IsUpdate()
    {
        if (m_ListNavPath.Count == 0)
            return false;

        return true;
    }

    public float GetAvoidRadius() { return m_AvoidRadius; }
}
