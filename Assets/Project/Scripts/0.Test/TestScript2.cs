using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
public class TestScript2 : MonoBehaviour
{
    [SerializeField]
    Transform m_Target;

    UnityEngine.AI.NavMeshAgent m_Agent;
    private void Awake()
    {
        m_Agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    private void Update()
    {
        m_Agent.SetDestination(m_Target.position);
    }
}
