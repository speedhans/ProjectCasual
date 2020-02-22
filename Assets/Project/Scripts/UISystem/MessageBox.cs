using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MessageBox : DefaultUI
{
    static GameObject m_MessageBoxPoolObject = null;

    [SerializeField]
    TMP_Text m_MainText;
    [SerializeField]
    GameObject m_Button1;
    [SerializeField]
    GameObject m_Button2;
    [SerializeField]
    TMP_Text m_ButtonText1;
    [SerializeField]
    TMP_Text m_ButtonText2;

    System.Action m_ButtonEvent1;
    System.Action m_ButtonEvent2;

    static public void CreateTwoButtonType(string _MainText, string _ButtonText1, System.Action _ButtonEvent1, string _ButtonText2 = "Close", System.Action _ButtonEvent2 = null)
    {
        GameObject box = null;

        if (m_MessageBoxPoolObject && m_MessageBoxPoolObject.transform.childCount > 0)
        {
            box = m_MessageBoxPoolObject.transform.GetChild(0).gameObject;
        }
        else
        {
            box = Instantiate(Resources.Load<GameObject>("MessageBox/MessageBox"));
        }

        if (box)
        {
            MessageBox mb = box.GetComponent<MessageBox>();
            if (mb)
            {
                mb.Initialzie(_MainText, _ButtonText1, _ButtonEvent1, _ButtonText2, _ButtonEvent2, true);
            }
        }
    }

    static public void CreateOneButtonType(string _MainText, string _ButtonText1 = "Close", System.Action _ButtonEvent1 = null)
    {
        GameObject box = null;

        if (m_MessageBoxPoolObject && m_MessageBoxPoolObject.transform.childCount > 0)
        {
            box = m_MessageBoxPoolObject.transform.GetChild(0).gameObject;
        }
        else
        {
            box = Instantiate(Resources.Load<GameObject>("MessageBox/MessageBox"));
        }

        if (box)
        {
            MessageBox mb = box.GetComponent<MessageBox>();
            if (mb)
            {
                mb.Initialzie(_MainText, _ButtonText1, _ButtonEvent1, null, null, false);
            }
        }
    }

    void Initialzie(string _MainText, string _ButtonText1, System.Action _ButtonEvent1, string _ButtonText2, System.Action _ButtonEvent2, bool _TwoButton)
    {
        gameObject.SetActive(true);

        m_MainText.text = _MainText;
        m_ButtonText1.text = _ButtonText1;
        m_ButtonEvent1 = _ButtonEvent1;

        if (!_TwoButton)
        {
            Vector3 pos = m_Button1.transform.localPosition;
            pos.x = 0.0f;
            m_Button1.transform.localPosition = pos;
            m_Button2.SetActive(false);
        }
        else
        {
            m_ButtonText2.text = _ButtonText2;

            Vector3 pos = m_Button1.transform.localPosition;
            pos.x = -180.0f;
            m_Button1.transform.localPosition = pos;
            m_ButtonEvent2 = _ButtonEvent2;
            m_Button2.SetActive(true);
        }
    }

    void PushToPool()
    {
        if (m_MessageBoxPoolObject == null)
        {
            m_MessageBoxPoolObject = new GameObject("MessageBoxPool");
        }

        transform.SetParent(m_MessageBoxPoolObject.transform);
        gameObject.SetActive(false);
    }

    public void ButtonEvent1()
    {
        PushToPool();
        m_ButtonEvent1?.Invoke();
    }

    public void ButtonEvent2()
    {
        PushToPool();
        m_ButtonEvent2?.Invoke();
    }
}
