using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPreviewUI : MonoBehaviour
{
    [SerializeField]
    RectTransform m_CharacterPosition;

    GameObject m_Model;
    Animator m_Animator;
    Transform m_AttachRightHandPoint;

    [SerializeField]
    RuntimeAnimatorController[] m_ListAnimator;

    public void Initialize()
    {

    }

    private void Update()
    {
        m_CharacterPosition.rotation = Quaternion.Euler(m_CharacterPosition.rotation.eulerAngles + new Vector3(0.0f, 10.0f * Time.deltaTime, 0.0f));
    }

    public void SetPreviewModel(GameObject _Model)
    {
        if (m_Model)
        {
            if (m_AttachRightHandPoint)
                Destroy(m_AttachRightHandPoint.gameObject);
            Destroy(m_Model);
        }

        m_Model = _Model;
        m_Animator = m_Model.GetComponentInChildren<Animator>();

        Transform t = Character.FindBone(m_Model.transform, "Character1_RightHandMiddle1");
        if (t)
        {
            GameObject attachpoint = new GameObject("AttachRightHandPoint");
            m_AttachRightHandPoint = attachpoint.transform;
            m_AttachRightHandPoint.transform.SetParent(t);
            m_AttachRightHandPoint.transform.localPosition = new Vector3(0.0f, 0.02f, 0.0f);
            m_AttachRightHandPoint.transform.localRotation = Quaternion.Euler(0.0f, -90.0f, -90.0f);
        }

        m_Model.transform.SetParent(m_CharacterPosition);
        m_Model.transform.localPosition = Vector3.zero;
        m_Model.transform.localRotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
    }

    public void SetPerviewWeapon(string _WeaponPath, E_WEAPONTYPE _WeaponType)
    {
        if (m_AttachRightHandPoint.childCount > 0)
        {
            for (int i = 0; i < m_AttachRightHandPoint.childCount; ++i)
            {
                Destroy(m_AttachRightHandPoint.GetChild(i).gameObject);
            }
        }

        GameObject weapon = Instantiate(Resources.Load<GameObject>(_WeaponPath));
        if (weapon)
        {
            weapon.transform.SetParent(m_AttachRightHandPoint);
            weapon.transform.localPosition = Vector3.zero;
            weapon.transform.localRotation = Quaternion.identity;
            SwitchAnimator((E_WEAPONTYPE)_WeaponType);
        }
    }

    void SwitchAnimator(E_WEAPONTYPE _Type)
    {
        RuntimeAnimatorController anim = FindAnimator(_Type.ToString());
        if (anim)
            m_Animator.runtimeAnimatorController = anim;
    }

    RuntimeAnimatorController FindAnimator(string _Name)
    {
        for (int i = 0; i < m_ListAnimator.Length; ++i)
        {
            if (m_ListAnimator[i].name.Contains(_Name))
            {
                return m_ListAnimator[i];
            }
        }

        return null;
    }
}
