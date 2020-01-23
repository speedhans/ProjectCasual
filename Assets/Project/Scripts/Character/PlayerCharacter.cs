using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerCharacter : HumanoidCharacter
{
    public bool m_IsPlayerCharacterInitializeComplete { get; protected set; }
    protected override void Awake()
    {
        base.Awake();

        if (true)
            GameManager.Instance.m_MyCharacter = this;

        string modelpath = (string)m_PhotonView.InstantiationData[5];
        Instantiate(Resources.Load<GameObject>(modelpath), Vector3.zero, Quaternion.identity, transform);

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

        main.AddPlayerCharacter(this);

        while(!m_IsInitializeComplete) yield return null;

        if (!m_PhotonView.IsMine) yield break;

        for (int i = 0; i < 3; ++i)
        {
            EquipmentItem item =  InventoryManager.Instance.GetEquippedItem(i);
            GameObject weapon = item.GetWeaponModel();
            if (weapon)
            {
                m_AttackType = item.m_ItemElement;
                AttachWeapon(weapon.name);
                break;
            }
        }

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

        m_IsPlayerCharacterInitializeComplete = true;
    }

    protected override void Update()
    {
        base.Update();
    }

    public void AttachWeapon(string _WeaponPath)
    {
        if (m_RightHandAxis == null) return;

        m_PhotonView.RPC("AttachWeapon_RPC", Photon.Pun.RpcTarget.AllBuffered, _WeaponPath);
    }

    [PunRPC]
    protected void AttachWeapon_RPC(string _WeaponPath)
    {
        if (m_RightHandAxis == null) return;

        GameObject weapon = Instantiate(Resources.Load<GameObject>(_WeaponPath));
        if (weapon)
        {
            weapon.transform.SetParent(m_RightHandAxis);
            weapon.transform.localPosition = Vector3.zero;
            weapon.transform.localRotation = Quaternion.identity;
        }
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
                SetStateAndAnimation(E_ANIMATION.RUN, 0.25f, 1.0f, 0.0f);
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
            
            SetStateAndAnimation(E_ANIMATION.ATTACK, 0.25f, 1.0f, 0.0f);

            StartCoroutine(C_AttackStep(_Forward, m_AttackSpeed));
            VerticalFollowCamera.CameraWave(1.0f / m_AttackSpeed);

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

        m_AttackTarget.m_Character.GiveToDamage(GetDamages(), this);
    }

    public void FootEvent(int _Value)
    {

    }
}
