using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(Rigidbody))]
public class Object : RootComponent
{
    public enum E_PHYSICSMODE
    {
        CHARACTER,
        STATIC,
        DYNAMIC,
        DYNAMIC_KINEMATIC,
        ATTACHED,
        OPERATING,
    }

    public enum E_LIVE
    {
        LIVE,
        DEAD,
    }

    public E_PHYSICSMODE m_PhysicsType;
    E_PHYSICSMODE m_ChangePhysicsType;
    public Rigidbody m_Rigid { get; private set; }
    protected Vector3 m_BeforeVelocity;

    protected Transform[] m_Transforms;
    protected Collider[] m_Colliders;
    protected Renderer[] m_Renderers;

    public E_LIVE m_Live;
    public float m_Health;
    [HideInInspector]
    public float m_MaxHealth;
    public float m_PerSecondHealthRegeneration;
    System.Action<float[], float[], int[], float, bool, Character> m_DamageEvent;
    System.Action m_DeadEvent;
    System.Action m_DestoryEvent;

    [SerializeField]
    protected S_RESISTANCE m_Resistance = new S_RESISTANCE();
    [Header("Added Resistance Values")]
    public float[] m_AddResistance = new float[(int)E_DAMAGETYPE.MAX];

    List<Buff> m_ListBuff = new List<Buff>();

    [SerializeField]
    bool m_HealthZeroIsDestroy = true;
    [SerializeField]
    protected float m_DestroyDelay;
    [Tooltip("null 이면 생성 안함")]
    [SerializeField]
    protected GameObject m_DestoryEffect;

    protected Object m_AttachTarget;

    [SerializeField]
    protected List<Transform> m_WeekPoint;

    //InstantiationData 0번 Physics Type
    //InstantiationData 1번 Attach Target PhotonView ID
    //InstantiationData 2번 Attach Path
    //InstantiationData 3번 Link Target Object PhotonView ID
    public PhotonView m_PhotonView { get; private set; }
    protected virtual void Awake()
    {
        m_MaxHealth = m_Health;

        m_Rigid = GetComponent<Rigidbody>();

        m_Transforms = GetComponentsInChildren<Transform>();
        m_Colliders = GetComponentsInChildren<Collider>();
        m_Renderers = GetComponentsInChildren<Renderer>();
        m_PhotonView = GetComponent<PhotonView>();

        m_ChangePhysicsType = m_PhysicsType;
    }

    protected virtual void Start()
    {
        if (m_PhotonView.IsMine)
            InitializePhotonInstantiationData();

        ResetPhysicsMode(false);

        if (!m_PhotonView.IsMine)
        {
            object[] o = m_PhotonView.InstantiationData;
            if (o != null)
            {
                SetPhysicsMode((E_PHYSICSMODE)o[0], false);
            }
        }
    }

    protected virtual void Update()
    {
        float deltatime = Time.deltaTime;
        
        for (int i = 0; i < m_ListBuff.Count; ++i)
        {
            m_ListBuff[i].Update(deltatime);
            if (m_ListBuff[i].m_LifeTime <= 0.0f)
            {
                if (m_PhotonView.IsMine)
                {
                    ReleaseBuff(m_ListBuff[i].m_BuffID);
                }
                m_ListBuff[i].Destroy();
                m_ListBuff.RemoveAt(i);
            }
        }

        GameManager.Instance.RunSkyEnvironmentEvent(this);
        RegenerationResistance(deltatime);
    }

    /// <summary>
    /// _DamageType 인수는 E_DAMAGETYPE 으로 변환해 사용하세요
    /// </summary>
    /// <param name="_Damage"> </param>
    /// <param name="_CalculrateDamage"> 모든 계산이 끝난 데미지 </param>
    /// <param name="_DamageType"> E_DAMAGETYPE 으로 변환해 사용하세요 </param>
    /// <param name="_AttackerID"></param>
    protected virtual void DamageEvent(float[] _Damage, float[] _CalculateDamage, int[] _DamageType, float _CalculateFullDamage, bool _Critical, Character _Attacker) { }

    protected void AddDamageEvent(System.Action<float[], float[], int[], float, bool, Character> _Event)
    {
        m_DamageEvent += _Event;
    }

    protected void SubDamageEvent(System.Action<float[], float[], int[], float, bool, Character> _Event)
    {
        m_DamageEvent -= _Event;
    }

    protected void ClearDamageEvent()
    {
        m_DamageEvent = null;
    }

    /// <summary>
    /// _DamageType 인수는 E_DAMAGETYPE 으로 변환해 사용하세요
    /// </summary>
    /// <param name="_Damage"></param>
    /// <param name="_DamageType"> E_DAMAGETYPE 으로 변환해 사용하세요 </param>
    /// <param name="_AttackerID"></param>
    public void GiveToDamage(float[] _Damage, int[] _DamageType, Character _Self)
    {
        if (m_Live == E_LIVE.DEAD) return;

        bool critical = Random.Range(0, 100) < _Self.m_CriticalChance ? true : false;

        if (critical)
        {
            for (int i = 0; i < _Damage.Length; ++i)
            {
                _Damage[i] = critical ? _Damage[i] * _Self.m_CriticalMuliply : _Damage[i];
            }
        }

        if (PhotonNetwork.IsConnectedAndReady && _Self != null)
        {
            if (_Self.m_PhotonView.IsMine)
                m_PhotonView.RPC("GiveToDamage_RPC", RpcTarget.AllViaServer, _Damage, _DamageType, critical, _Self.m_PhotonView.ViewID);
        }
        else
            GiveToDamage_Local(_Damage, _DamageType, critical, _Self);
    }

    public void GiveToDamage(float[] _Damage, Character _Self)
    {
        if (m_Live == E_LIVE.DEAD) return;

        List<float> damages = new List<float>();
        List<int> types = new List<int>();

        bool critical = Random.Range(0, 100) < _Self.m_CriticalChance ? true : false;

        for (int i = 0; i < _Damage.Length; ++i)
        {
            if (_Damage[i] > 0.0f)
            {
                float damage = critical ? _Damage[i] * _Self.m_CriticalMuliply : _Damage[i];
                if ((E_DAMAGETYPE)i == _Self.m_AttackType)
                {
                    damages.Insert(0, damage);
                    types.Insert(0, i);
                }
                else
                {
                    damages.Add(damage);
                    types.Add(i);
                }
            }
        }
        if (types.Count < 1) return;

        if (PhotonNetwork.IsConnectedAndReady && _Self != null)
        {
            if (_Self.m_PhotonView.IsMine)
                m_PhotonView.RPC("GiveToDamage_RPC", RpcTarget.AllViaServer, damages.ToArray(), types.ToArray(), critical, _Self.m_PhotonView.ViewID);
        }
        else
            GiveToDamage_Local(damages.ToArray(), types.ToArray(), critical, _Self);
    }

    [PunRPC]
    public void GiveToDamage_RPC(float[] _Damage, int[] _DamageType, bool _Critical, int _AttackerID)
    {
        Character attacker = NetworkManager.Instance.RoomController.FindObjectWithPhotonViewID(_AttackerID) as Character;

        GiveToDamage_Local(_Damage, _DamageType, _Critical, attacker);
    }

    void GiveToDamage_Local(float[] _Damage, int[] _DamageType, bool _Critical, Character _Attacker)
    {
        if (m_Live == E_LIVE.DEAD) return;

        float fulldamage = 0.0f;

        bool weekpointhit = false;
        //if (m_WeekPoint.Count > 0)
        //{
        //    for (int i = 0; i < m_WeekPoint.Count; ++i)
        //    {
        //        if (m_WeekPoint[i].gameObject.activeSelf && 
        //            0.7f <= Vector3.Dot(m_WeekPoint[i].forward, (_Attacker.transform.position - transform.position).normalized))
        //        {
        //            weekpointhit = true;
        //            break;
        //        }
        //    }
        //}

        float[] caculratedamages = (float[])_Damage.Clone();
        for (int i = 0; i < _Damage.Length; ++i)
        {
            float damage = _Damage[i];// weekpointhit ? _Damage[i] * 1.35f : _Damage[i];
            switch ((E_DAMAGETYPE)_DamageType[i])
            {
                case E_DAMAGETYPE.FIRE:
                    damage *= (100.0f - Mathf.Clamp((m_Resistance.Fire + m_AddResistance[0]), 0.0f, 100.0f)) * 0.01f;
                    break;
                case E_DAMAGETYPE.ICE:
                    damage *= (100.0f - Mathf.Clamp((m_Resistance.Ice + m_AddResistance[1]), 0.0f, 100.0f)) * 0.01f;
                    break;
                case E_DAMAGETYPE.ELECTRIC:
                    damage *= (100.0f - Mathf.Clamp((m_Resistance.Electric + m_AddResistance[2]), 0.0f, 100.0f)) * 0.01f;
                    break;
                case E_DAMAGETYPE.WIND:
                    damage *= (100.0f - Mathf.Clamp((m_Resistance.Wind + m_AddResistance[3]), 0.0f, 100.0f)) * 0.01f;
                    break;
                case E_DAMAGETYPE.LIGHT:
                    damage *= (100.0f - Mathf.Clamp((m_Resistance.Light + m_AddResistance[4]), 0.0f, 100.0f)) * 0.01f;
                    break;
                case E_DAMAGETYPE.DARK:
                    damage *= (100.0f - Mathf.Clamp((m_Resistance.Dark + m_AddResistance[5]), 0.0f, 100.0f)) * 0.01f;
                    break;
            }
            caculratedamages[i] = damage;
            m_Health -= damage;
            fulldamage += damage;
        }

        if (m_Health <= 0.0f)
        {
            ObjectDestroyWithDelay(m_DestroyDelay);
        }

        m_DamageEvent?.Invoke(_Damage, caculratedamages, _DamageType, fulldamage, _Critical, _Attacker);
    }

    public void ObjectDestroyWithDelay(float _DestroyDelay = 2.5f)
    {
        m_Live = E_LIVE.DEAD;
        m_DeadEvent?.Invoke();
        if (m_HealthZeroIsDestroy)
            StartCoroutine(C_DestroyTimer(_DestroyDelay));
    }

    IEnumerator C_DestroyTimer(float _Timer)
    {
        yield return new WaitForSeconds(_Timer);
        if (PhotonNetwork.IsConnectedAndReady && m_PhotonView.IsMine)
            ObjectDestroy();
        else
            ObjectDestroy();
    }

    public void ObjectDestroy()
    {
        if (PhotonNetwork.InRoom)
        {
            if (m_PhotonView.isRuntimeInstantiated)
                m_PhotonView.RPC("ObjectDestroy_RPC", RpcTarget.AllViaServer, true);
            else
                m_PhotonView.RPC("ObjectDestroy_RPC", RpcTarget.AllBufferedViaServer, false);
        }
        else
        {
            ObjectDestroy_RPC(false);
        }
    }

    [PunRPC]
    protected void ObjectDestroy_RPC(bool _RuntimeInstaiated = true)
    {
        if (PhotonNetwork.InRoom)
        {
            if (m_PhotonView.IsMine && _RuntimeInstaiated)
                PhotonNetwork.Destroy(gameObject);
        }
        else
            Destroy(gameObject);
    }

    public void AddBuff(int _BuffID, object[] _BuffData = null)
    {
        m_PhotonView.RPC("AddBuff_RPC", RpcTarget.AllViaServer, _BuffID, _BuffData);
    }

    public void AddBuff(E_BUFF _BuffID, object[] _BuffData = null)
    {
        m_PhotonView.RPC("AddBuff_RPC", RpcTarget.AllViaServer, (int)_BuffID, _BuffData);
    }

    [PunRPC]
    public void AddBuff_RPC(int _BuffID, object[] _BuffData = null)
    {
        Buff b = FindBuff(_BuffID);
        if (b != null)
        {
            b.DataUpdate(_BuffData);
        }
        else
        {
            Buff buff = Buff.CreateBuff(_BuffID, this, _BuffData);

            m_ListBuff.Add(buff);
        }
    }

    public Buff FindBuff(int _BuffID)
    {
        for (int i = 0; i< m_ListBuff.Count; ++i)
        {
            if (m_ListBuff[i].m_BuffID == _BuffID)
            {
                return m_ListBuff[i];
            }
        }

        return null;
    }

    public List<Buff> GetBuffList() { return m_ListBuff; }

    public void BuffDataUpdate(int _BuffID, object[] _Value)
    {
        m_PhotonView.RPC("BuffDataUpdate_RPC", RpcTarget.AllViaServer, _BuffID, _Value);
    }

    [PunRPC]
    protected void BuffDataUpdate_RPC(int _BuffID, object[] _Value)
    {
        Buff b = FindBuff(_BuffID);
        if (b == null) return;

        b.DataUpdate(_Value);
    }

    public void ReleaseBuff(int _BuffID)
    {
        m_PhotonView.RPC("ReleaseBuff_RPC", RpcTarget.AllViaServer, _BuffID);
    }

    [PunRPC]
    protected void ReleaseBuff_RPC(int _BuffID)
    {
        Buff b = FindBuff(_BuffID);
        if (b == null) return;

        b.m_LifeTime = 0.0f;
    }

    void RegenerationResistance(float _DeltaTime)
    {
        if (m_MaxHealth > m_Health)
        {
            m_Health += m_PerSecondHealthRegeneration * _DeltaTime;
            if (m_MaxHealth < m_Health)
                m_Health = m_MaxHealth;
        }
    }

    public void AddDeadEvent(System.Action _Action)
    {
        m_DeadEvent += _Action;
    }

    public void SubDeadEvent(System.Action _Action)
    {
        m_DeadEvent -= _Action;
    }

    protected void AddDestroyEvent(System.Action _Action)
    {
        m_DestoryEvent += _Action;
    }

    protected void SubDestoryEvent(System.Action _Action)
    {
        m_DestoryEvent -= _Action;
    }

    public Rigidbody GetRigidbody() { return m_Rigid; }

    protected void FixedUpdate()
    {
        m_BeforeVelocity = m_Rigid.velocity;
    }

    protected virtual void OnDestroy()
    {
        if (m_DestoryEffect) Instantiate(m_DestoryEffect, transform.position, transform.rotation);
        m_DestoryEvent?.Invoke();
    }

    public void SetWorldPositionAndRotation(Vector3 _Position, Quaternion _Rotation)
    {
        m_PhotonView.RPC("SetWorldPositionAndRotation_RPC", RpcTarget.AllViaServer, _Position, _Rotation);
    }

    [PunRPC]
    public void SetWorldPositionAndRotation_RPC(Vector3 _Position, Quaternion _Rotation)
    {
        transform.SetPositionAndRotation(_Position, _Rotation);
    }

    public void SetLocalPositionAndRotation(Vector3 _Position, Quaternion _Rotation)
    {
        m_PhotonView.RPC("SetLocalPositionAndRotation_RPC", RpcTarget.AllViaServer, _Position, _Rotation);
    }

    [PunRPC]
    public void SetLocalPositionAndRotation_RPC(Vector3 _Position, Quaternion _Rotation)
    {
        transform.localPosition = _Position;
        transform.localRotation = _Rotation;
    }

    public void ResyncPositionAndRotation()
    {
        m_PhotonView.RPC("ResyncPositionAndRotation_RPC", m_PhotonView.Owner, PhotonNetwork.LocalPlayer);
    }

    void ResyncPositionAndRotation_RPC(Player _Target) // callback function
    {
        m_PhotonView.RPC("SetLocalPositionAndRotation_RPC", _Target, transform.localPosition, transform.localRotation);
    }

    public void SetPhysicsMode(E_PHYSICSMODE _Mode, bool _Network = true)
    {
        if (_Network)
            m_PhotonView.RPC("SetPhysicsMode_RPC", RpcTarget.AllViaServer, _Mode);
        else
            SetPhysicsMode_RPC(_Mode);
    }

    [PunRPC]
    public void SetPhysicsMode_RPC(E_PHYSICSMODE _Mode)
    {
        m_ChangePhysicsType = _Mode;
        if (m_PhotonView.IsMine)
        {
            SetPhotonInstantiationData(new int[] { 0 }, new object[] { m_ChangePhysicsType });
        }
        switch (_Mode)
        {
            case E_PHYSICSMODE.STATIC:
                {
                    foreach (Transform t in m_Transforms)
                    {
                        t.gameObject.layer = Common.STATIC_LAYER;
                    }

                    m_Rigid.useGravity = false;
                    m_Rigid.isKinematic = true;
                    m_Rigid.interpolation = RigidbodyInterpolation.None;
                    m_Rigid.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

                    foreach (Collider c in m_Colliders)
                    {
                        c.isTrigger = false;
                    }
                }
                break;
            case E_PHYSICSMODE.DYNAMIC:
                {
                    foreach (Transform t in m_Transforms)
                    {
                        t.gameObject.layer = Common.DYNAMIC_LAYER;
                    }

                    m_Rigid.useGravity = true;
                    m_Rigid.isKinematic = false;
                    m_Rigid.interpolation = RigidbodyInterpolation.Interpolate;
                    m_Rigid.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

                    foreach (Collider c in m_Colliders)
                    {
                        c.isTrigger = false;
                    }
                }
                break;
            case E_PHYSICSMODE.DYNAMIC_KINEMATIC:
                {
                    foreach (Transform t in m_Transforms)
                    {
                        t.gameObject.layer = Common.DYNAMIC_LAYER;
                    }

                    m_Rigid.useGravity = false;
                    m_Rigid.isKinematic = true;
                    m_Rigid.interpolation = RigidbodyInterpolation.None;
                    m_Rigid.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

                    foreach (Collider c in m_Colliders)
                    {
                        c.isTrigger = true;
                    }
                }
                break;
            case E_PHYSICSMODE.ATTACHED:
                {
                    foreach (Transform t in m_Transforms)
                    {
                        t.gameObject.layer = Common.ATTACHED_LAYER;
                    }

                    m_Rigid.useGravity = false;
                    m_Rigid.isKinematic = true;
                    m_Rigid.interpolation = RigidbodyInterpolation.None;
                    m_Rigid.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

                    foreach (Collider c in m_Colliders)
                    {
                        c.isTrigger = true;
                    }
                }
                break;
            case E_PHYSICSMODE.OPERATING:
                {
                    foreach (Transform t in m_Transforms)
                    {
                        t.gameObject.layer = Common.OPERATING_LAYER;
                    }

                    m_Rigid.useGravity = false;
                    m_Rigid.isKinematic = true;
                    m_Rigid.interpolation = RigidbodyInterpolation.None;
                    m_Rigid.collisionDetectionMode = CollisionDetectionMode.Discrete;

                    foreach (Collider c in m_Colliders)
                    {
                        c.isTrigger = true;
                    }
                }
                break;
            case E_PHYSICSMODE.CHARACTER:
                {
                    foreach (Transform t in m_Transforms)
                    {
                        gameObject.layer = Common.CHARACTER_LAYER;
                    }

                    m_Rigid.useGravity = true;
                    m_Rigid.isKinematic = false;
                    m_Rigid.interpolation = RigidbodyInterpolation.Interpolate;
                    m_Rigid.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

                    m_Rigid.constraints = RigidbodyConstraints.FreezeRotation;
                }
                break;
        }
    }

    public void ResetPhysicsMode(bool _Network = true)
    {
        SetPhysicsMode(m_PhysicsType, _Network);
    }

    public void SetVelocity(Vector3 _Velocity)
    {
        m_PhotonView.RPC("SetVelocity_RPC", RpcTarget.AllViaServer, _Velocity);
    }

    [PunRPC]
    public void SetVelocity_RPC(Vector3 _Velocity)
    {
        m_Rigid.velocity = _Velocity;
    }

    void SetPhotonInstantiationData(int[] _Number, object[] _Value)
    {
        object[] data = m_PhotonView.InstantiationData;

        if (data == null)
            data = new object[5];

        for (int i = 0; i < _Number.Length; ++i)
        {
            data[_Number[i]] = _Value[i];
        }
        m_PhotonView.InstantiationData = data;
    }

    void InitializePhotonInstantiationData()
    {
        int[] keys = new int[] { 0, 1, 2, 3, 4 };
        object[] values = new object[] { m_ChangePhysicsType, -1, "", -1, -1 };

        object[] data = m_PhotonView.InstantiationData;

        if (data == null)
            data = new object[6] { -1, -1, -1, -1, -1, -1 };

        for (int i = 0; i < keys.Length; ++i)
        {
            if (data[i] == values[i]) continue;
            data[keys[i]] = values[i];
        }
        m_PhotonView.InstantiationData = data;
    }

    //private void OnDrawGizmos()
    //{
    //    for (int i = 0; i < transform.childCount; ++i)
    //    {
    //        Renderer r = transform.GetChild(i).GetComponent<Renderer>();
    //        if (r)
    //        {
    //            Gizmos.color = Color.red;
    //            Gizmos.DrawCube(transform.GetChild(i).position, r.bounds.size);
    //        }
    //    }
    //}
}
