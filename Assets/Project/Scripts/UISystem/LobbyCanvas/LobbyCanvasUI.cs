using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyCanvasUI : MonoBehaviour
{
    [SerializeField]
    protected StatusUI m_StatusUI;
    [SerializeField]
    protected InventoryUI m_InventoryUI;
    [SerializeField]
    protected MenuUI m_MenuUI;
    [SerializeField]
    protected QuestUI m_QuestUI;
    [SerializeField]
    protected ItemDataViewer m_ItemDataViewer;
    [SerializeField]
    protected LobbyDefaultUI m_LobbyDefaultUI;

    List<LobbyUI> m_ListLobbyUIDepth = new List<LobbyUI>();

    private void Awake()
    {
        if (!m_StatusUI)
            transform.Find("StatusUI").GetComponent<StatusUI>();
        m_StatusUI.Initialize(this);
        if (!m_InventoryUI)
            transform.Find("InventoryUI").GetComponent<InventoryUI>();
        m_InventoryUI.Initialize(this);
        if (!m_MenuUI)
            transform.Find("MenuUI").GetComponent<MenuUI>();
        m_MenuUI.Initialize(this);
        if (!m_QuestUI)
            transform.Find("QuestUI").GetComponent<QuestUI>();
        m_QuestUI.Initialize(this);
        if (!m_LobbyDefaultUI)
            transform.Find("LobbyDefaultUI").GetComponent<LobbyDefaultUI>();
        m_LobbyDefaultUI.Initialize(this);
        if (!m_ItemDataViewer)
            transform.Find("ItemDataViewer").GetComponent<ItemDataViewer>();
        m_ItemDataViewer.Initialize(this);
    }

    public StatusUI GetStatusUI() { return m_StatusUI; }

    public InventoryUI GetInventoryUI() { return m_InventoryUI; }

    public MenuUI GetMenuUI() { return m_MenuUI; }

    public QuestUI GetQuestUI() { return m_QuestUI; }

    public ItemDataViewer GetItemDataViewer() { return m_ItemDataViewer; }
    public void ResetUIDepth() 
    {
        LobbyUI[] uis = m_ListLobbyUIDepth.ToArray();

        for (int i = 0; i < uis.Length; ++i)
            uis[i].Close();
        m_ListLobbyUIDepth.Clear();
    }
    public void AddUIDepth(LobbyUI _LobbyUI) 
    {
        m_ListLobbyUIDepth.Remove(_LobbyUI);
        m_ListLobbyUIDepth.Add(_LobbyUI); 
    }
    public void SubUIDepth(LobbyUI _LobbyUI) 
    { 
        m_ListLobbyUIDepth.Remove(_LobbyUI);
    }
    public void CloseLastUIDepth()
    {
        if (m_ListLobbyUIDepth.Count < 1) return;
        m_ListLobbyUIDepth[m_ListLobbyUIDepth.Count - 1].Close();
    }
}
