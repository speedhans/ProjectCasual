using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPreviewUI : MonoBehaviour
{
    [SerializeField]
    RectTransform m_CharacterPosition;

    GameObject m_Model;
    Transform m_AttachRightHandPoint;

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
}
