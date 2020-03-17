using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerCharacter : HumanoidCharacter
{
    public const string m_CharacterReadyKey = "CharacterReady";

    [SerializeField]
    RuntimeAnimatorController[] m_AnimatorControllers;

    public bool m_IsPlayerCharacterInitializeComplete { get; protected set; }
    protected override void Awake()
    {
        base.Awake();
        m_CharacterType = E_CHARACTERTYPE.PLAYER;
        if (m_PhotonView.IsMine)
            GameManager.Instance.m_MyCharacter = this;

        string modelname = (string)m_PhotonView.InstantiationData[5];
        Instantiate(Resources.Load<GameObject>(modelname + "/" + modelname), Vector3.zero, Quaternion.identity, transform);

        StartCoroutine(C_Initialize());
    }

    IEnumerator C_Initialize()
    {
        Main main = null;
        while(main == null)
        {
            main = GameManager.Instance.m_Main;
            yield return null;
        }

        while (!main.IsBeginLoadingComplete) yield return null;

        main.AddPlayerCharacter(this);

        while(!m_IsInitializeComplete) yield return null;

        Material mat = null;
        if (m_PhotonView.IsMine)
            mat = ((Main_Stage)GameManager.Instance.m_Main).m_LocalOutLineMaterial;
        else
            mat = ((Main_Stage)GameManager.Instance.m_Main).m_OtherOutLineMaterial;
        SkinnedMeshRenderer[] renderers = m_Animator.GetComponentsInChildren<SkinnedMeshRenderer>();
        for (int i = 0; i < renderers.Length; ++i)
        {
            Material[] newmat = new Material[renderers[i].materials.Length + 1];
            for (int j = 0; j < renderers[i].materials.Length; ++j)
            {
                newmat[j] = renderers[i].materials[j];
            }
            newmat[newmat.Length - 1] = mat;
            renderers[i].materials = newmat;
        }

        if (m_PhotonView.IsMine)
        {
            bool attachweapon = false;
            for (int i = 0; i < 3; ++i)
            {
                EquipmentItem item = InventoryManager.Instance.GetEquippedItem(i);
                if (item)
                {
                    GameObject weapon = item.GetWeaponModel();
                    if (weapon)
                    {
                        attachweapon = true;
                        m_AttackType = item.m_ItemElement;
                        AttachWeapon(weapon.name, item.GetWeaponType());
                        break;
                    }
                }
            }

            if (!attachweapon)
                AttachWeapon("", E_WEAPONTYPE.PUNCH);

            for (int i = 0; i < 3; ++i)
            {
                EquipmentItem item = InventoryManager.Instance.GetEquippedItem(i);
                if (item)
                {
                    item.EquipAction(this);
                    ActiveSkill sk = item.GetActiveSkill();
                    if (sk)
                    {
                        sk.Initialize(this, true);
                    }

                    List<PassiveSkill> list = item.GetPassiveSkills();
                    if (list != null)
                    {
                        for (int j = 0; j < list.Count; ++j)
                        {
                            list[j].Initialize(this, true);
                        }
                    }
                }
            }
            NetworkManager.Instance.RoomController.SetLocalPlayerProperties(m_CharacterReadyKey, true);
        }
        m_IsPlayerCharacterInitializeComplete = true;
    }

    protected override void Update()
    {
        base.Update();
    }

    public void AttachWeapon(string _WeaponPath, E_WEAPONTYPE _WeaponType)
    {
        if (m_RightHandAxis == null) return;

        m_PhotonView.RPC("AttachWeapon_RPC", Photon.Pun.RpcTarget.AllBuffered, _WeaponPath, (int)_WeaponType);
    }

    [PunRPC]
    protected void AttachWeapon_RPC(string _WeaponPath, int _WeaponType)
    {
        if (m_RightHandAxis == null) return;

        if (_WeaponPath == "")
        {
            SwitchAnimator(E_WEAPONTYPE.PUNCH);
        }
        else
        {
            GameObject weapon = Instantiate(Resources.Load<GameObject>(_WeaponPath));
            if (weapon)
            {
                weapon.transform.SetParent(m_RightHandAxis);
                weapon.transform.localPosition = Vector3.zero;
                weapon.transform.localRotation = Quaternion.identity;
                weapon.transform.transform.localScale = Vector3.one;
                SwitchAnimator((E_WEAPONTYPE)_WeaponType);

                if (PhotonNetwork.InRoom)
                {
                    Material mat = null;
                    if (m_PhotonView.IsMine)
                        mat = ((Main_Stage)GameManager.Instance.m_Main).m_LocalOutLineMaterial;
                    else
                        mat = ((Main_Stage)GameManager.Instance.m_Main).m_OtherOutLineMaterial;

                    MeshRenderer[] renderers = weapon.GetComponentsInChildren<MeshRenderer>();
                    for (int i = 0; i < renderers.Length; ++i)
                    {
                        Material[] newmat = new Material[renderers[i].materials.Length + 1];
                        for (int j = 0; j < renderers[i].materials.Length; ++j)
                        {
                            newmat[j] = renderers[i].materials[j];
                        }
                        newmat[newmat.Length - 1] = mat;
                        renderers[i].materials = newmat;
                    }
                }
            }
        }
    }

    void SwitchAnimator(E_WEAPONTYPE _Type)
    {
        m_WeaponType = _Type;
        m_AttackSound = SoundManager.Instance.GetPlayerAttackmonetClip(m_WeaponType);
        RuntimeAnimatorController anim = FindAnimator(_Type.ToString());
        if (anim)
            StartCoroutine(C_SwitchAnimator(anim));
    }

    IEnumerator C_SwitchAnimator(RuntimeAnimatorController _Anim)
    {
        while (m_Animator == null) yield return null;
        m_Animator.runtimeAnimatorController = _Anim;
    }

    RuntimeAnimatorController FindAnimator(string _Name)
    {
        for (int i = 0; i < m_AnimatorControllers.Length; ++i)
        {
            if (m_AnimatorControllers[i].name.Contains(_Name))
            {
                return m_AnimatorControllers[i];
            }
        }

        return null;
    }

    public void MoveDirection2D(Vector2 _Direction)
    {
        if (m_Live == E_LIVE.DEAD) return;
        if (IsFreeze()) return;

        Vector3 dir = new Vector3(_Direction.x, 0.0f, _Direction.y);

        if (!AttackAttempt(dir, 1.0f))
        {
            if (m_State != E_STATE.MOVE)
            {
                SetStateAndAnimationNetwork(E_ANIMATION.RUN, 0.25f, 1.0f, 0.0f);
            }
            MoveToLocation(transform.position + (dir * Time.deltaTime * m_MovePerSpeed * 5.0f));
        }
    }

    public bool AttackAttempt(Vector3 _Forward, float _Radius)
    {
        if (m_AttackDelayTimer > 0.0f) return false;

        Character c = FindEnemyShape(this, this.transform.position, _Forward, m_AttackRange, 40.0f);
        if (c)
        {
            if (m_NavMeshController.IsUpdate())
                m_NavMeshController.ClearPath();

            if (c.m_Live == E_LIVE.DEAD) return true;
            
            if (m_AttackTarget == null)
            {
                HateTarget target = new HateTarget(c, 1);
                m_AttackTarget = target;
            }
            else if (m_AttackTarget.m_Character != c)
            {
                HateTarget target = new HateTarget(c, 1);
                m_AttackTarget = target;
            }

            transform.forward = m_AttackTarget.m_Character.transform.position - transform.position;
            
            SetStateAndAnimationNetwork(E_ANIMATION.ATTACK, 0.25f, 1.0f, 0.0f);

            StartCoroutine(C_AttackStep(_Forward, m_AttackSpeed));

            return true;
        }

        return false;
    }

    IEnumerator C_AttackStep(Vector3 _Direction, float _Speed)
    {
        GetRigidbody().velocity = _Direction * 1.75f;
        yield return new WaitForSeconds(0.2f / m_AttackSpeed);
        GetRigidbody().velocity = (-_Direction) * 1.75f;
    }

    public override void AttackMomentEvent()
    {
        if (m_Live == E_LIVE.DEAD) return;
        base.AttackMomentEvent();

        if (m_AttackTarget == null) return;

        VerticalFollowCamera.CameraWave(Mathf.Clamp(1.0f / m_AttackSpeed * 0.5f, 0.0f, 0.3f));
        m_AttackTarget.m_Character.GiveToDamage(GetDamages(), this);
    }

    public void FootEvent(int _Value)
    {

    }
}
