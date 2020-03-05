using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

public class MonsterSpawner : Spawner
{
    [SerializeField]
    GameObject m_SpawnEffectPrefab;
    GameObject m_SpawnEffect;

    [SerializeField]
    float m_SpawnDelay;
    string m_TimerKey;

    [SerializeField]
    int m_SpawnCount = 1;
    int m_CurrentSpawnCount;
    int m_DeadCount;

    protected override void Awake()
    {
        base.Awake();
        m_SetManagementTargetEvent += SetManagementTargetEvent;
        m_SpawnEffect = Instantiate(m_SpawnEffectPrefab, transform);
        m_SpawnEffect.transform.localPosition = new Vector3(0.0f, 0.1f, 0.0f);
        m_SpawnEffect.transform.localRotation = Quaternion.identity;
        StopParticle();
    }

    public void Initialize(int _Number, SpawnManager _SpawnManager)
    {
        m_SpawnManager = _SpawnManager;
        m_SpawnerNumber = _Number;
        Resources.Load<GameObject>(m_FilePath + "/" + m_MonsterPrefab.name);

        m_TimerKey = "Spawner" + m_SpawnerNumber.ToString();
        TimerNet.InsertTimer(m_TimerKey, 2.0f, true);
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_SpawnRun) return;
        if (m_ManagementTarget != null)
        {
            if (m_ManagementTarget.m_Live == Object.E_LIVE.DEAD)
            {
                ++m_DeadCount;
                TimerNet.SetTimer_s(m_TimerKey, false);
                m_ManagementTarget = null;
            }
            return;
        }

        if (m_CurrentSpawnCount < m_SpawnCount && TimerNet.GetTimer(m_TimerKey) <= 0.0f)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                GameObject g = PhotonNetwork.InstantiateSceneObject(m_FilePath + "/" + m_MonsterPrefab.name, transform.position, Quaternion.Euler(0.0f, 180.0f, 0.0f));
                if (g)
                {
                    m_ManagementTarget = g.GetComponent<Character>();
                    PhotonView view = g.GetComponent<PhotonView>();
                    m_PhotonView.RPC("SetManagementTarget_RPC", RpcTarget.AllViaServer, view.ViewID);
                    TimerNet.SetTimer_s(m_TimerKey, m_SpawnDelay, true);
                }
            }
        }
    }

    void SetManagementTargetEvent(int _ViewID)
    {
        ++m_CurrentSpawnCount;
        PlayParticle();
        PhotonView view = PhotonView.Find(_ViewID);
        if (view)
        {
            Character c = view.gameObject.GetComponent<Character>();
            if (c)
            {
                m_ManagementTarget = c;
                m_ManagementTarget.SetFreeze(0.5f);
                TimerNet.SetTimer_s(m_TimerKey, m_SpawnDelay, false);
            }
        }
    }

    public override void SpawnRun()
    {
        m_SpawnRun = true;
        TimerNet.SetTimer_s(m_TimerKey, 2.0f, false);
    }

    void PlayParticle()
    {
        foreach (ParticleSystem p in m_SpawnEffect.GetComponentsInChildren<ParticleSystem>())
        {
            p.Play();
        }
    }

    void StopParticle()
    {
        foreach (ParticleSystem p in m_SpawnEffect.GetComponentsInChildren<ParticleSystem>())
        {
            p.Stop();
        }
    }

    public bool IsAllSpawnOperationsCompleted()
    {
        return m_DeadCount >= m_SpawnCount ? true : false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position + new Vector3(0.0f, 0.5f, 0.0f), Vector3.one);
    }
#endif
}
