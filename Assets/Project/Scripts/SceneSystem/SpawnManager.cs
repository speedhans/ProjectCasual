using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    Main_Stage m_Main;

    [SerializeField]
    MonsterSpawner[] m_MonsterSpawner;
    [SerializeField]
    BossSpawner[] m_BossSpawner;

    private void Awake()
    {
        m_Main = FindObjectOfType<Main_Stage>();
        m_Main.m_SpawnManager = this;
    }

    public void Initialize()
    {
        for (int i = 0; i < m_MonsterSpawner.Length; ++i)
        {
            m_MonsterSpawner[i].Initialize(i, this);
        }

        for (int i = 0; i < m_BossSpawner.Length; ++i)
        {
            m_BossSpawner[i].Initialize(i, this);
        }
    }

    public void NormalMonsterSpawnStart()
    {
        for (int i = 0; i < m_MonsterSpawner.Length; ++i)
        {
            m_MonsterSpawner[i].SpawnRun();
        }
    }

    public void BossMonsterSpawnStart()
    {
        for (int i = 0; i < m_BossSpawner.Length; ++i)
        {
            m_BossSpawner[i].SpawnRun();
            m_BossSpawner[i].SpawnBoss();
        }
    }

    public bool IsAllNormalMonsterSpawnOperationsCompleted()
    {
        bool monsteralldie = true;
        for (int i = 0; i < m_MonsterSpawner.Length; ++i)
        {
            if (!m_MonsterSpawner[i].IsAllSpawnOperationsCompleted())
            {
                monsteralldie = false;
                break;
            }
        }

        return monsteralldie;
    }

    public bool IsAllBossMonsterSpawnOperationsCompleted()
    {
        bool monsteralldie = true;
        for (int i = 0; i < m_BossSpawner.Length; ++i)
        {
            if (!m_BossSpawner[i].IsAllSpawnOperationsCompleted())
            {
                monsteralldie = false;
                break;
            }
        }

        return monsteralldie;
    }
}
