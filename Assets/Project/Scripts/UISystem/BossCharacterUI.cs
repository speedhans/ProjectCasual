using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCharacterUI : DefaultUI
{
    static BossCharacterUI single = null;
    static public BossCharacterUI Instance
    {
        get 
        {
            if (single == null)
            {
                GameObject g = Instantiate(Resources.Load<GameObject>("BossUICanvas"));
                single = g.GetComponent<BossCharacterUI>();
            }

            return single;
        }
        protected set
        {
            single = value;
        }
    }

    private class BossUIData
    {
        public Transform m_Slot;
        public RectTransform m_Slider;
        List<UnityEngine.UI.Image> m_ListBuffIcon = new List<UnityEngine.UI.Image>();
        public TMPro.TMP_Text m_NameField;
        public Character m_Character;

        private BossUIData() { }
        public BossUIData(Character _Character, Transform _Slot)
        {
            m_Character = _Character;
            m_Slot = _Slot;
            m_NameField = m_Slot.transform.Find("Name_Text").GetComponent<TMPro.TMP_Text>();
            m_NameField.text = m_Character.name.Replace("(Clone)", "").Trim();
            Transform bufflist = m_Slot.Find("BuffList");
            m_Slider = m_Slot.transform.Find("HpBar/Slider").GetComponent<RectTransform>();
            for (int i = 0; i < bufflist.childCount; ++i)
            {
                m_ListBuffIcon.Add(bufflist.GetChild(i).GetComponent<UnityEngine.UI.Image>());
            }
            for (int i = 0; i < m_ListBuffIcon.Count; ++i)
            {
                m_ListBuffIcon[i].gameObject.SetActive(false);
            }
        }

        public void SliderUpdate()
        {
            m_Slider.localScale = new Vector3(Mathf.Clamp01(m_Character.m_Health / m_Character.m_MaxHealth), 1.0f, 1.0f);
        }


        public void BuffIconUpdate()
        {
            List<Buff> list = m_Character.GetBuffList();

            for (int i = 0; i < m_ListBuffIcon.Count; ++i)
            {
                if (list.Count <= i)
                {
                    m_ListBuffIcon[i].gameObject.SetActive(false);
                    continue;
                }

                m_ListBuffIcon[i].sprite = list[i].m_BuffIcon;
                m_ListBuffIcon[i].gameObject.SetActive(true);
            }
        }

        public void Clear()
        {
            m_Slot = null;
            m_Slider = null;
            m_ListBuffIcon.Clear();
            m_NameField = null;
            m_Character = null;
        }
    }

    [SerializeField]
    Transform m_SlotGroupTransform;
    UnityEngine.UI.GridLayoutGroup m_SlotGroupGrid;
    [SerializeField]
    GameObject[] m_Slot;

    List<BossUIData> m_ListBossData = new List<BossUIData>();

    private void Awake()
    {
        if (single == null)
            single = this;

        m_SlotGroupGrid = m_SlotGroupTransform.GetComponent<UnityEngine.UI.GridLayoutGroup>();
        for (int i = 0; i < m_Slot.Length; ++i)
        {
            m_Slot[i].SetActive(false);
        }
    }

    private void Update()
    {
        for (int i = 0; i < m_ListBossData.Count; ++i)
        {
            BossUIData data = m_ListBossData[i];
            data.SliderUpdate();
            data.BuffIconUpdate();
        }
    }

    public void InsertUI(Character _InsertData)
    {
        Debug.Log("insert");
        m_SlotGroupGrid.cellSize = new Vector2(600.0f - (m_ListBossData.Count * 125.0f), 75.0f);
        BossUIData data = new BossUIData(_InsertData, m_Slot[m_ListBossData.Count].transform);

        data.m_Slot = m_Slot[m_ListBossData.Count].transform;
        m_Slot[m_ListBossData.Count].transform.SetParent(m_SlotGroupTransform);
        m_Slot[m_ListBossData.Count].SetActive(true);
        m_ListBossData.Add(data);
    }

    public void RemoveUI(Character _UIWithCharacter)
    {
        for (int i = 0; i < m_ListBossData.Count; ++i)
        {
            if (m_ListBossData[i].m_Character == _UIWithCharacter)
            {
                m_ListBossData[i].Clear();
                m_ListBossData.RemoveAt(i);
                m_Slot[m_ListBossData.Count].transform.SetParent(transform);
                m_Slot[m_ListBossData.Count].SetActive(false);
            }
        }
    }
}
