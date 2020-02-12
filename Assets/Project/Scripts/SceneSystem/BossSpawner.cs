using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

public class BossSpawner : MonoBehaviour
{
    SpawnManager m_SpawnManager;
    int m_SpawnerNumber;

    [SerializeField]
    string m_FilePath;
    [SerializeField]
    GameObject m_MonsterPrefab;
    [HideInInspector]
    public Character m_ManagementTarget;

    PhotonView m_PhotonView;

    int m_SpawnCount = 1;
    int m_DeadCount;

    bool m_SpawnRun = false;

    private void Awake()
    {
        m_PhotonView = GetComponent<PhotonView>();
    }

    public void Initialize(int _Number, SpawnManager _SpawnManager)
    {
        m_SpawnManager = _SpawnManager;
        m_SpawnerNumber = _Number;
        Resources.Load<GameObject>(m_FilePath + "/" + m_MonsterPrefab.name);
    }

    private void Update()
    {
        if (!m_SpawnRun) return;
        if (m_ManagementTarget != null)
        {
            if (m_ManagementTarget.m_Live == Object.E_LIVE.DEAD)
            {
                ++m_DeadCount;
                m_ManagementTarget = null;
            }
            return;
        }
    }

    public void SpawnBoss()
    {
        if (PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                GameObject g = PhotonNetwork.InstantiateSceneObject(m_FilePath + "/" + m_MonsterPrefab.name, transform.position, Quaternion.Euler(0.0f, 180.0f, 0.0f));
                if (g)
                {
                    PhotonView view = g.GetComponent<PhotonView>();
                    m_PhotonView.RPC("SetManagementTarget_RPC", RpcTarget.All, view.ViewID);
                }
            }
        }
        else
        {
            GameObject g = Instantiate(m_MonsterPrefab, transform.position, Quaternion.Euler(0.0f, 180.0f, 0.0f));
            Character c = g.GetComponent<Character>();
            if (c)
            {
                m_ManagementTarget = c;
            }
        }
    }

    [PunRPC]
    void SetManagementTarget_RPC(int _ViewID)
    {
        PhotonView view = PhotonView.Find(_ViewID);
        if (view)
        {
            Character c = view.gameObject.GetComponent<Character>();
            if (c)
            {
                m_ManagementTarget = c;
                BossCharacterUI.Instance.InsertUI(c);
            }
        }
    }

    public void SpawnRun()
    {
        m_SpawnRun = true;
    }

    public bool IsAllSpawnOperationsCompleted()
    {
        return m_DeadCount >= m_SpawnCount ? true : false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.color = Color.gray;
        Gizmos.DrawCube(transform.position + new Vector3(0.0f, 0.75f, 0.0f), Vector3.one * 1.5f);
    }
#endif
}
