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
    public struct S_DynamicObjectAvoidedData
    {
        [Header("[The number of sensors must be odd]")]
        public int m_SencorCount;
        public float m_SencorLength;
        public float m_SencorAngle;
        public float m_UpdateDelay;
        public LayerMask m_SencorCenterLayer;
        public LayerMask m_SencorBothLayer;
    }

    [SerializeField]
    protected S_DynamicObjectAvoidedData m_DynamicObjectAvoidedData;
    [SerializeField]
    bool m_UseObstacleAvoided = true;
    float m_UpdateTimer;

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

        if (m_UseObstacleAvoided)
        {
            m_UpdateTimer -= deltatime;
            if (m_UpdateTimer <= 0.0f)
            {
                m_UpdateTimer = m_DynamicObjectAvoidedData.m_UpdateDelay;
                Vector3 avoideddirection = CheckAvoidedLocation(_Transform, dirnormal);
                if (avoideddirection != Vector3.zero)
                {
                    List<Vector3> list = FindNavMeshPath(_Transform.position, _Transform.position + avoideddirection);
                    if (list.Count > 0)
                    {
                        m_ListAvoidNavPath.Clear();
                        m_ListAvoidNavPath.AddRange(list);

                        dir = m_ListAvoidNavPath[0] - _Transform.position;
                        dirnormal = dir.normalized;
                    }
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

    public void SetMoveLocation(Vector3 _StartLocation, Vector3 _TargetLocation)
    {
        m_ListNavPath.Clear();

        Vector3 samplelocation = FindNavMeshSampleLocation(_TargetLocation);
        if (samplelocation == Vector3.zero) return;
        m_MoveLocation = samplelocation;
        m_ListNavPath.AddRange(CalculateNavMeshPath(_StartLocation, m_MoveLocation));
    }

    public List<Vector3> FindNavMeshPath(Vector3 _StartLocation, Vector3 _TargetLocation)
    {
        Vector3 samplelocation = FindNavMeshSampleLocation(_StartLocation, 0.5f);
        if (samplelocation == Vector3.zero) return null;
        return CalculateNavMeshPath(samplelocation, _TargetLocation);
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

    List<Vector3> CalculateNavMeshPath(Vector3 _StartPoint, Vector3 _TargetPoint)
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

    public int GetAvoidCheckLineCount() { return m_DynamicObjectAvoidedData.m_SencorCount; }
    public float GetAvoidCheckLineLength() { return m_DynamicObjectAvoidedData.m_SencorLength; }

    Vector3 CheckAvoidedLocation(Transform _SourceTransform, Vector3 _Direction) // 양수가 right
    {
        Ray ray = new Ray();
        RaycastHit centerrayhit;
        Vector3 raystart = _SourceTransform.position + new Vector3(0.0f, 0.5f, 0.0f);
        int centernumber = m_DynamicObjectAvoidedData.m_SencorCount / 2;
        float interval = m_DynamicObjectAvoidedData.m_SencorAngle / (m_DynamicObjectAvoidedData.m_SencorCount - 1);
        float halfangle = m_DynamicObjectAvoidedData.m_SencorAngle * 0.5f;

        ray.origin = raystart;
        ray.direction = _Direction.normalized;
        if (Physics.Raycast(ray, out centerrayhit, m_DynamicObjectAvoidedData.m_SencorLength, m_DynamicObjectAvoidedData.m_SencorCenterLayer)) // 센터 검사 성공시, 외각 검사 실행
        {
            Debug.DrawLine(raystart, centerrayhit.point, Color.red);

            Character hittarget = centerrayhit.transform.GetComponent<Character>();
            if (hittarget.m_Team == m_Character.m_Team)
            {
                Vector3 leftsafedirection = Vector3.zero;
                Vector3 rightsafedirection = Vector3.zero;
                int leftweight = 0;
                int rightweight = 0;
                RaycastHit leftrayhit;
                RaycastHit rightrayhit;
                for (int i = 0; i < centernumber; ++i) // left
                {
                    ++leftweight;
                    ray.origin = raystart;
                    ray.direction = (Quaternion.AngleAxis(-((i + 1) * interval), Vector3.up) * _Direction).normalized;
                    if (Physics.Raycast(ray, out leftrayhit, m_DynamicObjectAvoidedData.m_SencorLength, m_DynamicObjectAvoidedData.m_SencorBothLayer))
                    {
                        Debug.DrawLine(raystart, leftrayhit.point, Color.red);
                    }
                    else
                    {
                        leftsafedirection = ray.direction;
                        Debug.DrawLine(raystart, raystart + (ray.direction * m_DynamicObjectAvoidedData.m_SencorLength), Color.green);
                        break;
                    }
                }

                for (int i = 0; i < centernumber; ++i) // right
                {
                    ++rightweight;
                    ray.origin = raystart;
                    ray.direction = (Quaternion.AngleAxis((i + 1) * interval, Vector3.up) * _Direction).normalized;
                    if (Physics.Raycast(ray, out rightrayhit, m_DynamicObjectAvoidedData.m_SencorLength, m_DynamicObjectAvoidedData.m_SencorBothLayer))
                    {
                        Debug.DrawLine(raystart, rightrayhit.point, Color.red);
                    }
                    else
                    {
                        rightsafedirection = ray.direction;
                        Debug.DrawLine(raystart, raystart + (ray.direction * m_DynamicObjectAvoidedData.m_SencorLength), Color.green);
                        break;
                    }
                }

                if (leftweight == rightweight)
                {
                    Vector3 rightvector = Vector3.Cross(_Direction, Vector3.up);
                    float dot = Vector3.Dot(rightvector, (centerrayhit.point - _SourceTransform.position).normalized);
                    if (dot < 0.0f) // left
                    {
                        --rightweight;
                    }
                    else // right
                    {
                        --leftweight;
                    }
                }

                if (leftweight > rightweight)
                {
                    return rightsafedirection * m_DynamicObjectAvoidedData.m_SencorLength;
                }
                else
                {
                    return leftsafedirection * m_DynamicObjectAvoidedData.m_SencorLength;
                }
            }
        }
        else
        {
            Debug.DrawLine(raystart, raystart + (ray.direction * m_DynamicObjectAvoidedData.m_SencorLength), Color.green);
        }

        return Vector3.zero;
    }
}
