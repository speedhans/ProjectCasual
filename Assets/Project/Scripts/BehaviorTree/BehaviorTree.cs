using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT
{
    public enum E_NODE
    {
        NONE,
        Selector,
        Sequence,
        BooleanNode,
        WaitForTime,
        MoveToLocation,
        LocationComparisonNode,
        SetRandomMoveLocation,
        StateCheckNode,
        AnimationPlayNode,
        GlobalTimeCheckNode,
        CompareNode,
        PatrolMoveNode,
        SearchObject,
        StopMoving,
        HeadFocusNode,
    }
}

public class BehaviorTree : MonoBehaviour
{
    [SerializeField]
    float m_UpdateDelay = 0.5f;
    float m_UpdateTimer;

    [Space(2)]
    [Header("NodeList")]
    [SerializeField]
    RootNode m_RootNode;
    Character m_Character;

    [HideInInspector]
    public Object m_TargetObject;
    [HideInInspector]
    public Transform m_MoveTargetTransform;

    private void Awake()
    {
        m_UpdateTimer = m_UpdateDelay;
        m_Character = GetComponent<Character>();
    }

    private void Start()
    {
        m_RootNode.Initialzie(m_Character, this);
    }

    private void Update()
    {
        float deltatime = Time.deltaTime;
        m_UpdateTimer -= deltatime;
        if (m_UpdateTimer <= 0.0f)
        {
            m_UpdateTimer = m_UpdateDelay;
            // update
            m_RootNode.BehaviorUpdate(m_Character, deltatime);
        }
    }
}
