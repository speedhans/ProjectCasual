using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class Spawner : MonoBehaviour
{
    protected SpawnManager m_SpawnManager;
    protected int m_SpawnerNumber;

    protected System.Action<int> m_SetManagementTargetEvent;

    [SerializeField]
    protected string m_FilePath;
    [SerializeField]
    protected GameObject m_MonsterPrefab;
    [HideInInspector]
    public Character m_ManagementTarget;

    protected PhotonView m_PhotonView;

    protected bool m_SpawnRun = false;

    protected virtual void Awake()
    {
        m_PhotonView = GetComponent<PhotonView>();
    }

    public virtual void SpawnRun() { }

    [PunRPC]
    protected void SetManagementTarget_RPC(int _ViewID)
    {
        m_SetManagementTargetEvent?.Invoke(_ViewID);
    }
}
