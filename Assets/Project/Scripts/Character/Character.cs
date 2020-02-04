using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Character : Object
{
    public class HateTarget
    {
        public HateTarget(Character _Character, float _Hate) { m_Character = _Character; m_Hate = _Hate; m_Key = _Character.gameObject.GetInstanceID(); }

        public Character m_Character;
        public float m_Hate;
        public int m_Key;
    }

    public enum E_STATE
    {
        NONE,
        IDLE,
        MOVE,
        ATTACK,
        SPECIAL,
        HIT,
    }

    public enum E_ATTACHPOINT
    {
        HEAD,
        LEFTHAND,
        RIGHTHAND,
        ORIGIN,
    }

    public enum E_CHARACTERTYPE
    {
        PLAYER,
        NORMALNPC,
        BOSSNPC,
    }

    public E_TEAMTYPE m_Team;
    public E_STATE m_State;

    protected E_CHARACTERTYPE m_CharacterType;

    public Animator m_Animator { get; private set; }
    protected NavMeshController m_NavMeshController = new NavMeshController();

    public Transform m_HeadAxis { get; protected set; }
    public Transform m_LeftHandAxis { get; protected set; }
    public Transform m_RightHandAxis { get; protected set; }

    protected CapsuleCollider m_BodyCollider;
    
    public float m_MovePerSpeed = 2.0f;
    public float m_RotatePerSpeed = 360.0f;
    public float m_AttackDamage = 1.0f;
    public float[] m_AddAttackDamage = new float[(int)E_DAMAGETYPE.MAX] { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
    public float m_AttackSpeed = 1.0f;
    public float m_AttackRange = 0.5f;
    public float m_CriticalChance = 10.0f;
    public float m_CriticalMuliply = 1.5f;
    public E_DAMAGETYPE m_AttackType = E_DAMAGETYPE.WIND;
    [SerializeField]
    protected float m_AttackAfterDelay = 0.0f;
    protected float m_AttackDelayTimer;
    public string[] m_AttackAniKey;
    protected int m_AttackAniNumber;
    protected float m_AttackLevelConnetTimer;
    protected HateTarget m_AttackTarget;
    [SerializeField]
    bool m_PlayHitAnimation = true;
    public List<ActiveSkill> m_ListActiveSkill = new List<ActiveSkill>();
    public List<PassiveSkill> m_ListPassiveSkill = new List<PassiveSkill>();

    protected float m_FreezeTimer;
    protected bool m_IsCharging = false;

    public bool m_IsAutoPlay = true;
    [SerializeField]
    protected float m_AutoSearchRadius;
    System.Action m_AutoPlayLogic;

    protected Dictionary<int, HateTarget> m_DicHateTarget = new Dictionary<int, HateTarget>();


    protected bool m_IsInitializeComplete = false;
    protected override void Awake()
    {
        base.Awake();

        StartCoroutine(C_Initialize());

        m_HeadAxis = transform;
        m_LeftHandAxis = transform;
        m_RightHandAxis = transform;

        m_BodyCollider = GetComponentInChildren<CapsuleCollider>();

        if (m_AttackAniKey == null || m_AttackAniKey.Length < 1)
        {
            m_AttackAniKey = new string[] { "Attack1" };
        }

        AddDamageEvent(DamageEvent);

        AddDeadEvent(DeadEvent);

        if (m_ListActiveSkill.Count > 0)
        {
            for (int i = 0; i < m_ListActiveSkill.Count; ++i)
            {
                m_ListActiveSkill[i].Initialize(this);
            }
        }

        if (m_ListPassiveSkill.Count > 0)
        {
            for (int i = 0; i < m_ListPassiveSkill.Count; ++i)
            {
                m_ListPassiveSkill[i].Initialize(this);
            }
        }
    }

    IEnumerator C_Initialize()
    {
        while (m_Animator == null)
        {
            m_Animator = GetComponentInChildren<Animator>();
            yield return null;
        }
        m_Animator.gameObject.AddComponent<AnimationEventCall>();

        Transform lh = FindBone(m_Animator.transform, "LeftHandMiddle");
        if (lh)
        {
            GameObject attachpoint = new GameObject("LeftHandAxis");
            m_RightHandAxis = attachpoint.transform;
            m_RightHandAxis.transform.SetParent(lh);
            m_RightHandAxis.transform.localPosition = new Vector3(0.0f, 0.02f, 0.0f);
            m_RightHandAxis.transform.localRotation = Quaternion.Euler(0.0f, 90.0f, -90.0f);
        }

        Transform rh = FindBone(m_Animator.transform, "RightHandMiddle");
        if (rh)
        {
            GameObject attachpoint = new GameObject("RightHandAxis");
            m_RightHandAxis = attachpoint.transform;
            m_RightHandAxis.transform.SetParent(rh);
            m_RightHandAxis.transform.localPosition = new Vector3(0.0f, 0.02f, 0.0f);
            m_RightHandAxis.transform.localRotation = Quaternion.Euler(0.0f, -90.0f, -90.0f);
        }

        m_IsInitializeComplete = true;
    }

    protected override void Start()
    {
        base.Start();

        if (m_CharacterType == E_CHARACTERTYPE.BOSSNPC)
        {

        }
        else
        {
            CharacterUI ui = Instantiate(Resources.Load<GameObject>("CharacterUI")).GetComponent<CharacterUI>();
            if (ui)
            {
                ui.Initialize(this);
            }
        }
    }

    protected override void Update()
    {
        base.Update();

        float deltatime = Time.deltaTime;
        if (m_AttackDelayTimer > 0.0f)
        {
            m_AttackDelayTimer -= deltatime;
            if (m_AttackDelayTimer <= 0.0f)
                m_AttackDelayTimer = 0.0f;
        }
        
        if (m_FreezeTimer > 0.0f)
        {
            m_FreezeTimer -= deltatime;
            if (m_FreezeTimer <= 0.0f)
                m_FreezeTimer = 0.0f;
        }

        if (m_AttackLevelConnetTimer > 0.0f)
        {
            m_AttackLevelConnetTimer -= deltatime;
            if (m_AttackLevelConnetTimer <= 0.0f)
            {
                m_AttackLevelConnetTimer = 0.0f;
                m_AttackAniNumber = 0;
            }
        }

        if (m_AttackTarget != null)
        {
            if (m_AttackTarget.m_Character == null)
                m_AttackTarget = null;
        }
        
        if (m_DicHateTarget.Count > 0)
        {
            foreach (HateTarget h in m_DicHateTarget.Values)
            {
                h.m_Hate -= deltatime * (1.0f + h.m_Hate * 0.1f);
            }
        }

        if (PhotonNetwork.IsConnectedAndReady && !m_PhotonView.IsMine) return;

        if (m_IsAutoPlay)
        {
            if (m_DicHateTarget.Count > 0)
            {
                List<int> deletekeys = new List<int>();
                foreach (HateTarget h in m_DicHateTarget.Values)
                {
                    if (h.m_Character != null)
                    {
                        if (m_AttackTarget == null)
                        {
                            m_AttackTarget = h;
                            break;
                        }
                        else
                        {
                            if (h.m_Hate <= 0.0f || h.m_Character == null)
                            {
                                deletekeys.Add(h.m_Key);
                                continue;
                            }
                            else if (h.m_Hate > m_AttackTarget.m_Hate)
                            {
                                m_AttackTarget = h;
                            }
                        }
                    }
                }

                if (deletekeys.Count > 0)
                {
                    for (int i = 0; i < deletekeys.Count; ++i)
                    {
                        m_DicHateTarget.Remove(deletekeys[i]);
                    }
                }
            }
        }

        if (m_Live == E_LIVE.DEAD) return;
        if (PhotonNetwork.IsConnectedAndReady && !m_PhotonView.IsMine) return;
        if (m_IsCharging) return;
        if (m_FreezeTimer > 0.0f) return;

        if (m_IsAutoPlay)
        {
            if (m_ListActiveSkill.Count > 0)
            {
                for (int i = 0; i < m_ListActiveSkill.Count; ++i)
                    m_ListActiveSkill[i].AutoPlayLogic();
            }

            m_AutoPlayLogic?.Invoke();
        }

        if (m_State != E_STATE.IDLE)
        {
            if (m_AttackDelayTimer <= 0.0f && !m_NavMeshController.IsUpdate())
            {
                m_State = E_STATE.IDLE;
                SetStateAndAnimation(E_ANIMATION.IDLE, 0.25f, 1.0f, 0.0f);
            }
        }

        m_NavMeshController.UpdateTransform(transform, m_MovePerSpeed, m_RotatePerSpeed);
    }

    public void MoveToLocation(Vector3 _TargetLocation)
    {
        m_NavMeshController.SetMoveLocation(transform.position, _TargetLocation);
    }

    public void StopMoving()
    {
        m_NavMeshController.ClearPath();
    }

    public Vector3 FindNavMeshSampleLocation(Vector3 _Point, float _SearchRadius = 0.2f) { return m_NavMeshController.FindNavMeshSampleLocation(_Point, _SearchRadius); }

    public Vector3 GetLastTargetLocation() { return m_NavMeshController.m_MoveLocation; }
    public bool IsNavMeshRunning() { return m_NavMeshController.IsUpdate(); }

    public virtual void AttackMomentEvent()
    {
    }

    public E_CHARACTERTYPE GetCharacterType() { return m_CharacterType; }

    public void SetFreeze(float _Time)
    {
        m_FreezeTimer = _Time;
    }

    public bool IsFreeze()
    {
        if (m_FreezeTimer > 0.0f)
            return true;

        return false;
    }

    public bool IsCharging() { return m_IsCharging; }

    public Transform GetAttachPoint(E_ATTACHPOINT _Point)
    {
        switch (_Point)
        {
            case E_ATTACHPOINT.HEAD:
                return m_HeadAxis;
            case E_ATTACHPOINT.LEFTHAND:
                return m_LeftHandAxis;
            case E_ATTACHPOINT.RIGHTHAND:
                return m_RightHandAxis;
            case E_ATTACHPOINT.ORIGIN:
                return transform;
            default:
                break;
        }

        return transform;
    }

    public void SetStateAndAnimation(E_ANIMATION _ANIMATION, float _Duration, float _Speed, float _FreezeTime, bool _IsCharing = false, bool _Network = true)
    {
        switch (_ANIMATION)
        {
            case E_ANIMATION.IDLE:
                SetCharacterState(E_STATE.IDLE, _IsCharing, _Network);
                break;
            case E_ANIMATION.WALK:
            case E_ANIMATION.RUN:
                SetCharacterState(E_STATE.MOVE, _IsCharing, _Network);
                break;
            case E_ANIMATION.ATTACK:
                SetCharacterState(E_STATE.ATTACK, _IsCharing, _Network);
                break;
            case E_ANIMATION.SPECIAL1:
            case E_ANIMATION.SPECIAL2:
            case E_ANIMATION.SPECIAL3:
                SetCharacterState(E_STATE.SPECIAL, _IsCharing, _Network);
                break;
            case E_ANIMATION.HIT:
                SetCharacterState(E_STATE.SPECIAL, _IsCharing, _Network);
                break;
        }

        PlayAnimation(_ANIMATION, _Duration, _Speed, _FreezeTime, _Network);
    }

    public void SetCharacterState(E_STATE _State, bool _IsCharging, bool _Network = true)
    {
        if (PhotonNetwork.IsConnectedAndReady && _Network)
        {
            if (m_PhotonView.IsMine)
                m_PhotonView.RPC("SetCharacterState_RPC", RpcTarget.All, (int)_State, _IsCharging);
        }
        else
            SetCharacterState_RPC((int)_State, _IsCharging);
    }

    [PunRPC]
    public void SetCharacterState_RPC(int _State, bool _IsCharging)
    {
        m_State = (E_STATE)_State;
        m_IsCharging = _IsCharging;
    }

    public void SetCharging(bool _IsCharging)
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (m_PhotonView.IsMine)
                m_PhotonView.RPC("SetCharging_RPC", RpcTarget.All, _IsCharging);
        }
        else
            SetCharging_RPC(_IsCharging);
    }

    [PunRPC]
    public void SetCharging_RPC(bool _IsCharging) { m_IsCharging = _IsCharging; }

    public void PlayAnimation(E_ANIMATION _ANIMATION, float _Duration, float _Speed, float _FreezeTime, bool _Network = true)
    {
        if (PhotonNetwork.IsConnectedAndReady && _Network)
        {
            if (m_PhotonView.IsMine)
                m_PhotonView.RPC("PlayAnimation_RPC", RpcTarget.All, (int)_ANIMATION, _Duration, _Speed, _FreezeTime);
        }
        else
            PlayAnimation_RPC((int)_ANIMATION, _Duration, _Speed, _FreezeTime);
    }

    [PunRPC]
    public void PlayAnimation_RPC(int _ANIMATION, float _Duration, float _Speed, float _FreezeTime)
    {
        if (_FreezeTime > 0.0f)
            SetFreeze(_FreezeTime);

        if (!m_Animator) return;

        m_Animator.speed = _Speed;
        switch ((E_ANIMATION)_ANIMATION)
        {
            case E_ANIMATION.IDLE:
                m_Animator.CrossFade("Idle", _Duration);
                break;
            case E_ANIMATION.WALK:
                m_Animator.CrossFade("Walk", _Duration);
                break;
            case E_ANIMATION.RUN:
                m_Animator.CrossFade("Run", _Duration);
                break;
            case E_ANIMATION.DEAD:
                m_Animator.CrossFade("Dead", _Duration);
                break;
            case E_ANIMATION.ATTACK:
                {
                    m_Animator.CrossFade(m_AttackAniKey[m_AttackAniNumber], _Duration);
                    if (m_AttackAniKey.Length > 1)
                    {
                        if (m_AttackAniNumber + 1 >= m_AttackAniKey.Length)
                            m_AttackAniNumber = 0;
                        else
                            m_AttackAniNumber++;
                    }
                    m_AttackDelayTimer = 1.0f / m_AttackSpeed + m_AttackAfterDelay;
                    SetFreeze(m_AttackDelayTimer);
                    m_Animator.speed = 1.0f * m_AttackSpeed;

                    m_AttackLevelConnetTimer = m_AttackDelayTimer + 0.5f;
                }
                break;
            case E_ANIMATION.SPECIAL1:
                m_Animator.CrossFade("Special1", _Duration);
                break;
            case E_ANIMATION.SPECIAL2:
                m_Animator.CrossFade("Special2", _Duration);
                break;
            case E_ANIMATION.SPECIAL3:
                m_Animator.CrossFade("Special3", _Duration);
                break;
            case E_ANIMATION.HIT:
                m_Animator.CrossFade("HIT", _Duration);
                break;
        }
    }

    public float[] GetDamages()
    {
        float[] damages = (float[])m_AddAttackDamage.Clone();
        damages[(int)m_AttackType] += m_AttackDamage;

        return damages;
    }

    protected override void DamageEvent(float[] _Damage, float[] _CalculateDamage, int[] _DamageType, float _CalculateFullDamage, bool _Critical, Character _Attacker)
    {
        base.DamageEvent(_Damage, _CalculateDamage, _DamageType, _CalculateFullDamage, _Critical, _Attacker);

        LifeTimerWithObjectPool effect = ObjectPool.GetObject<LifeTimerWithObjectPool>(((E_DAMAGETYPE)_DamageType[0]).ToString()); // 데미지 이팩트
        if (effect)
        {
            effect.Initialize();
            effect.transform.localScale = Vector3.one * (m_BodyCollider.radius * 2.0f);
            effect.transform.position = transform.position + new Vector3(0.0f, m_BodyCollider.center.y, 0.0f);
            effect.gameObject.SetActive(true);
        }

        float YOffset = 0.0f;
        for (int i = 0; i < _DamageType.Length; ++i)
        {
            Billboard b = ObjectPool.GetObject<Billboard>("DamageText");
            if (b)
            {
                b.gameObject.SetActive(true);
                b.Initialize(_Critical ? ((int)_CalculateDamage[i]).ToString() + "!!" : ((int)_CalculateDamage[i]).ToString(),
                    transform.position + new Vector3(0.0f, 1.0f + (m_BodyCollider.height * 0.5f + m_BodyCollider.center.y + YOffset), 0.0f),
                    2.0f);
                b.SetColor(Common.m_DamageColor[_DamageType[i]]);

                YOffset += 0.33f;
            }
        }

        if (m_Health <= 0.0f)
        {
            m_Animator.speed = 1.0f;
            m_Animator.CrossFade("Dead", 0.15f);
            m_Live = E_LIVE.DEAD;
            m_IsCharging = false;
            m_FreezeTimer = 0.0f;
            return;
        }

        if (m_PlayHitAnimation)
        {
            m_Animator.CrossFade("Hit", 0.1f);
            SetFreeze(0.5f);
        }

        if (m_IsAutoPlay)
        {
            float damage = 0.0f;
            for (int i = 0; i < _Damage.Length; ++i)
            {
                damage += _Damage[i];
            }
            Character target = null;
            HateTarget t = null;
            target = _Attacker as Character;

            if (target != null)
            {
                if (m_DicHateTarget.TryGetValue(target.gameObject.GetInstanceID(), out t))
                {
                    t.m_Hate += damage;
                }
                else
                {
                    m_DicHateTarget.Add(target.gameObject.GetInstanceID(), new HateTarget(target, damage));
                }
            }
        }
    }

    void DeadEvent()
    {
        Main_Stage main = GameManager.Instance.m_Main as Main_Stage;
        if (main == null) return;
        if (main.m_DissolveShader == null)
        {
            Debug.LogError("not find shader");
            return;
        }
        Material mat = new Material(main.m_DissolveShader);
        if (mat == null)
        {
            Debug.LogError("failed material create");
            return;
        }
        //mat.SetTexture("_MainTex", m_Renderers[0].material.mainTexture);
        //mat.SetTexture("_LightingRamp", m_Renderers[0].material.GetTexture("_LightingRamp"));
        foreach (Renderer r in m_Renderers)
        {
            r.material = mat;
        }

        StartCoroutine(C_DeadDissolve(mat, 1.0f / m_DestroyDelay));
    }

    IEnumerator C_DeadDissolve(Material _Mat ,float _PerSpeed)
    {
        float progress = 0.0f;
        while (progress < 1.0f)
        {
            progress += Time.deltaTime * _PerSpeed;
            _Mat.SetFloat("_DissolveProgress", progress);
            yield return null;
        }
    }

    protected void SetAutoPlayLogic(System.Action _Function) { m_AutoPlayLogic = _Function; }

    protected virtual void AutoPlayLogic()
    {

    }

    // --- static functions ---
    #region Static Functions

    static public Transform FindBone(Transform _Root, string _BoneName)
    {
        return FindBone_s(_Root, _BoneName);
    }

    static Transform FindBone_s(Transform _Transform, string _BoneName)
    {
        if (_Transform.name.Contains(_BoneName)) return _Transform;

        for (int i = 0; i < _Transform.childCount; ++i)
        {
            Transform t = FindBone_s(_Transform.GetChild(i), _BoneName);
            if (t) return t;
        }

        return null;
    }

    static public Character FindEnemyShape(Character _Self, Vector3 _Location, Vector3 _Forward, float _Radius, float _HalfAngle, bool _CheckLive = true, bool _DeadOnly = false)
    {
        Character target = null;
        Collider[] colls = Physics.OverlapSphere(_Location, _Radius, 1 << Common.CHARACTER_LAYER);
        if (colls.Length > 0)
        {
            float distance = 99999999.9f;
            for (int i = 0; i < colls.Length; ++i)
            {
                if (colls[i] == _Self.m_BodyCollider) continue;
                Character c = colls[i].GetComponentInParent<Character>();
                if (c)
                {
                    if (c.m_Team == _Self.m_Team) continue;
                    if (_CheckLive && c.m_Live == E_LIVE.DEAD) continue;
                    if (_DeadOnly && c.m_Live != E_LIVE.DEAD) continue;

                    Vector3 dir = colls[i].transform.position - _Location;
                    float dot = Vector3.Dot(_Forward, dir.normalized);
                    if (dot > 1.0f - _HalfAngle / 180.0f)
                    {
                        float sqrm = dir.sqrMagnitude;
                        if (sqrm < distance)
                        {
                            target = c;
                            distance = sqrm;
                        }
                    }
                }
            }
        }

        return target;
    }

    static public Character FindTeamShape(Character _Self, E_TEAMTYPE _Team, Vector3 _Location, Vector3 _Forward, float _Radius, float _HalfAngle, bool _CheckLive = true, bool _DeadOnly = false)
    {
        Character target = null;
        Collider[] colls = Physics.OverlapSphere(_Location, _Radius, 1 << Common.CHARACTER_LAYER);
        if (colls.Length > 0)
        {
            float distance = 99999999.9f;
            for (int i = 0; i < colls.Length; ++i)
            {
                if (colls[i] == _Self.m_BodyCollider) continue;
                Character c = colls[i].GetComponentInParent<Character>();
                if (c)
                {
                    if (c.m_Team != _Team) continue;
                    if (_CheckLive && c.m_Live == E_LIVE.DEAD) continue;
                    if (_DeadOnly && c.m_Live != E_LIVE.DEAD) continue;

                    Vector3 dir = colls[i].transform.position - _Location;
                    float dot = Vector3.Dot(_Forward, dir.normalized);
                    if (dot > 1.0f - _HalfAngle / 180.0f)
                    {
                        float sqrm = dir.sqrMagnitude;
                        if (sqrm < distance)
                        {
                            target = c;
                            distance = sqrm;
                        }
                    }
                }
            }
        }

        return target;
    }

    static public List<Character> FindEnemyAllShape(Character _Self, Vector3 _Location, Vector3 _Forward, float _Radius, float _HalfAngle, bool _CheckLive = true, bool _DeadOnly = false)
    {
        Collider[] colls = Physics.OverlapSphere(_Location, _Radius, 1 << Common.CHARACTER_LAYER);
        if (colls.Length > 0)
        {
            List<Character> list = new List<Character>();
            for (int i = 0; i < colls.Length; ++i)
            {
                if (colls[i] == _Self.m_BodyCollider) continue;
                Character c = colls[i].GetComponentInParent<Character>();
                if (c)
                {
                    if (c.m_Team == _Self.m_Team) continue;
                    if (_CheckLive && c.m_Live == E_LIVE.DEAD) continue;
                    if (_DeadOnly && c.m_Live != E_LIVE.DEAD) continue;

                    Vector3 dir = colls[i].transform.position - _Location;
                    float dot = Vector3.Dot(_Forward, dir.normalized);
                    if (dot > 1.0f - _HalfAngle / 180.0f)
                    {
                        list.Add(c);
                    }
                }
            }

            if (list.Count < 1)
                return null;
            return list;
        }
        return null;
    }

    static public List<Character> FindTeamAllShape(Character _Self, E_TEAMTYPE _Team, Vector3 _Location, Vector3 _Forward, float _Radius, float _HalfAngle, bool _CheckLive = true, bool _DeadOnly = false)
    {
        Collider[] colls = Physics.OverlapSphere(_Location, _Radius, 1 << Common.CHARACTER_LAYER);
        if (colls.Length > 0)
        {
            List<Character> list = new List<Character>();
            for (int i = 0; i < colls.Length; ++i)
            {
                if (colls[i] == _Self.m_BodyCollider) continue;
                Character c = colls[i].GetComponentInParent<Character>();
                if (c)
                {
                    if (c.m_Team != _Team) continue;
                    if (_CheckLive && c.m_Live == E_LIVE.DEAD) continue;
                    if (_DeadOnly && c.m_Live != E_LIVE.DEAD) continue;

                    Vector3 dir = colls[i].transform.position - _Location;
                    float dot = Vector3.Dot(_Forward, dir.normalized);
                    if (dot > 1.0f - _HalfAngle / 180.0f)
                    {
                        list.Add(c);
                    }
                }
            }

            if (list.Count < 1)
                return null;
            return list;
        }
        return null;
    }

    static public Character FindEnemyArea(Character _Self, Vector3 _Location, float _Radius, bool _CheckLive = true, bool _DeadOnly = false)
    {
        Character target = null;
        Collider[] colls = Physics.OverlapSphere(_Location, _Radius, 1 << Common.CHARACTER_LAYER);

        if (colls.Length > 0)
        {
            float distance = 99999999.9f;
            for (int i = 0; i < colls.Length; ++i)
            {
                if (colls[i] == _Self.m_BodyCollider) continue;
                Character c = colls[i].GetComponentInParent<Character>();
                if (c)
                {
                    if (c.m_Team == _Self.m_Team) continue;
                    if (_CheckLive && c.m_Live == E_LIVE.DEAD) continue;
                    if (_DeadOnly && c.m_Live != E_LIVE.DEAD) continue;

                    Vector3 dir = colls[i].transform.position - _Location;
                    float sqrm = dir.sqrMagnitude;
                    if (sqrm < distance)
                    {
                        target = c;
                        distance = sqrm;
                    }
                }
            }
        }

        return target;
    }

    static public Character FindTeamArea(Character _Self, E_TEAMTYPE _Team, Vector3 _Location, float _Radius, bool _CheckLive = true, bool _DeadOnly = false)
    {
        Character target = null;
        Collider[] colls = Physics.OverlapSphere(_Location, _Radius, 1 << Common.CHARACTER_LAYER);

        if (colls.Length > 0)
        {
            float distance = 99999999.9f;
            for (int i = 0; i < colls.Length; ++i)
            {
                if (colls[i] == _Self.m_BodyCollider) continue;
                Character c = colls[i].GetComponentInParent<Character>();
                if (c)
                {
                    if (c.m_Team != _Team) continue;
                    if (_CheckLive && c.m_Live == E_LIVE.DEAD) continue;
                    if (_DeadOnly && c.m_Live != E_LIVE.DEAD) continue;

                    Vector3 dir = colls[i].transform.position - _Location;
                    float sqrm = dir.sqrMagnitude;
                    if (sqrm < distance)
                    {
                        target = c;
                        distance = sqrm;
                    }
                }
            }
        }

        return target;
    }

    static public List<Character> FindEnemyAllArea(Character _Self, Vector3 _Location, float _Radius, bool _CheckLive = true, bool _DeadOnly = false)
    {
        Collider[] colls = Physics.OverlapSphere(_Location, _Radius, 1 << Common.CHARACTER_LAYER);

        if (colls.Length > 0)
        {
            List<Character> list = new List<Character>();

            for (int i = 0; i < colls.Length; ++i)
            {
                if (colls[i] == _Self.m_BodyCollider) continue;
                Character c = colls[i].GetComponentInParent<Character>();
                if (c)
                {
                    if (c.m_Team == _Self.m_Team) continue;
                    if (_CheckLive && c.m_Live == E_LIVE.DEAD) continue;
                    if (_DeadOnly && c.m_Live != E_LIVE.DEAD) continue;

                    list.Add(c);
                }
            }

            if (list.Count < 1)
                return null;
            return list;
        }
        return null;
    }

    static public List<Character> FindTeamAllArea(Character _Self, E_TEAMTYPE _Team, Vector3 _Location, float _Radius, bool _OthersOnly = true ,bool _CheckLive = true, bool _DeadOnly = false)
    {
        Collider[] colls = Physics.OverlapSphere(_Location, _Radius, 1 << Common.CHARACTER_LAYER);

        if (colls.Length > 0)
        {
            List<Character> list = new List<Character>();

            for (int i = 0; i < colls.Length; ++i)
            {
                if (_OthersOnly && colls[i] == _Self.m_BodyCollider) continue;
                Character c = colls[i].GetComponentInParent<Character>();
                if (c)
                {
                    if (c.m_Team != _Team) continue;
                    if (_CheckLive && c.m_Live == E_LIVE.DEAD) continue;
                    if (_DeadOnly && c.m_Live != E_LIVE.DEAD) continue;

                    list.Add(c);
                }
            }

            if (list.Count < 1)
                return null;
            return list;
        }
        return null;
    }

    static public int ScopeEnemyCheck(Character _Self, Vector3 _Location, float _Radius, bool _OthersOnly = true, bool _CheckLive = true, bool _DeadOnly = false)
    {
        Collider[] colls = Physics.OverlapSphere(_Location, _Radius, 1 << Common.CHARACTER_LAYER);

        int count = 0;
        if (colls.Length > 0)
        {
            for (int i = 0; i < colls.Length; ++i)
            {
                if (_OthersOnly && colls[i] == _Self.m_BodyCollider) continue;
                Character c = colls[i].GetComponentInParent<Character>();
                if (c)
                {
                    if (c.m_Team == _Self.m_Team) continue;
                    if (_CheckLive && c.m_Live == E_LIVE.DEAD) continue;
                    if (_DeadOnly && c.m_Live != E_LIVE.DEAD) continue;

                    count++;
                }
            }
        }

        return count;
    }

    static public int ScopeTeamCheck(Character _Self, E_TEAMTYPE _Team, Vector3 _Location, float _Radius, bool _CheckLive = true, bool _DeadOnly = false)
    {
        Collider[] colls = Physics.OverlapSphere(_Location, _Radius, 1 << Common.CHARACTER_LAYER);

        int count = 0;
        if (colls.Length > 0)
        {
            for (int i = 0; i < colls.Length; ++i)
            {
                if (colls[i] == _Self.m_BodyCollider) continue;
                Character c = colls[i].GetComponentInParent<Character>();
                if (c)
                {
                    if (c.m_Team != _Team) continue;
                    if (_CheckLive && c.m_Live == E_LIVE.DEAD) continue;
                    if (_DeadOnly && c.m_Live != E_LIVE.DEAD) continue;

                    count++;
                }
            }
        }

        return count;
    }

    #endregion Static Functions
}
