using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class Main_Stage : Main
{
    PlayerCharacter m_MyCharacter;
    public Shader m_DissolveShader;

    GameObject m_ControlCanvas;
    GameUICanvas m_GameUICanvas;
    GameObject m_SkillUICanvas;
    BossCharacterUI m_BossUICanvas;

    [SerializeField]
    GameObject m_StageDefaultCamera;
    [SerializeField]
    GameObject m_VerticalCamera;
    [SerializeField]
    Item[] m_ResultItemlist;

    [SerializeField]
    Character m_BossCharacter;
    [HideInInspector]
    public int m_MonsterDeadCount;
    public int m_BossWakeUpCount;
    [SerializeField]
    Transform m_BossRoomStartPoint;

    protected override void Awake()
    {
        base.Awake();
        
        gameObject.AddComponent<FrameChecker>();
        Instantiate(m_StageDefaultCamera, Vector3.zero, Quaternion.identity);

        StartCoroutine(C_Initialize());
    }

    IEnumerator C_Initialize()
    {
        m_ControlCanvas = Instantiate(Resources.Load<GameObject>("ControlCanvas"));
        m_ControlCanvas.SetActive(false);
        GameObject gameUI = Instantiate(Resources.Load<GameObject>("GameUICanvas"));
        m_GameUICanvas = gameUI.GetComponent<GameUICanvas>();
        m_GameUICanvas.Initialize();
        gameUI.SetActive(false);
        m_SkillUICanvas = Instantiate(Resources.Load<GameObject>("SkillUICanvas"), Vector3.zero, Quaternion.identity);
        m_SkillUICanvas.SetActive(false);
        GameObject bossUI = Instantiate(Resources.Load<GameObject>("BossUICanvas"));
        m_BossUICanvas = bossUI.GetComponent<BossCharacterUI>();
        bossUI.SetActive(false);

        GameObject g = PhotonNetwork.Instantiate("PlayerCharacter", Vector3.zero, Quaternion.identity, 0, new object[] { -1, -1, -1, -1, -1, InventoryManager.Instance.GetPlayerModelName() });
        m_MyCharacter = g.GetComponent<PlayerCharacter>();

        GameObject vcamera = Instantiate(m_VerticalCamera, Vector3.zero, Quaternion.identity);
        VerticalFollowCamera verticalcamera = vcamera.GetComponent<VerticalFollowCamera>();
        verticalcamera.Initialize(this, m_MyCharacter);
        vcamera.SetActive(false);

        yield return null;

        NetworkManager.Instance.RoomController.SetLocalPlayerProperties("InitComp", true);

        while(true)
        {
            Dictionary<int, Player> playercontainer = PhotonNetwork.CurrentRoom.Players;
            int compcount = 0;
            int playercount = playercontainer.Count;

            foreach (Player p in playercontainer.Values)
            {
                if (NetworkManager.Instance.RoomController.GetOtherPlayerPropertie<bool>(p, "InitComp"))
                    ++compcount;
            }

            if (compcount >= playercount)
            {
                StageDefaultCamera.Instance.DestroyCamera();
                vcamera.SetActive(true);
                InitializeComplete();
                yield break;
            }

            yield return null;
        }
    }

    void InitializeComplete()
    {
        m_ControlCanvas.SetActive(true);
        m_GameUICanvas.gameObject.SetActive(true);
        m_SkillUICanvas.SetActive(true);
        m_BossUICanvas.gameObject.SetActive(true);
        m_BossCharacter.SetFreeze(999999.9f);
        IsLoadingComplete = true;
    }

    public Item[] GetResultItemlist() 
    {
        List<Item> list = new List<Item>();
        for (int i = 0; i < m_ResultItemlist.Length; ++i)
        {
            list.Add(Instantiate(m_ResultItemlist[i].gameObject).GetComponent<Item>());
        }

        InventoryManager.Instance.InserItem(list);

        return list.ToArray();
    }

    bool m_BossWakeUp = false;
    public void MonsterDead()
    {
        ++m_MonsterDeadCount;
        if (!m_BossWakeUp && m_MonsterDeadCount >= m_BossWakeUpCount)
        {
            BossWakeUp();
        }
    }

    public void BossWakeUp()
    {
        m_BossWakeUp = true;
        m_BossCharacter.SetFreeze(0.0f);
        m_BossUICanvas.gameObject.SetActive(true);
        m_BossUICanvas.InsertUI(m_BossCharacter);
        StartCoroutine(C_BossStateCheck());
    }

    IEnumerator C_BossStateCheck()
    {
        m_MyCharacter.transform.position = m_BossRoomStartPoint.position;
        m_MyCharacter.transform.rotation = Quaternion.identity;
        VerticalFollowCamera.SetDistanceSmooth(5.0f, 1.5f);

        while (true)
        {
            if (m_BossCharacter.m_Live == Object.E_LIVE.DEAD)
            {
                m_ControlCanvas.SetActive(false);
                m_SkillUICanvas.SetActive(false);
                GameObject clearui = Instantiate(Resources.Load<GameObject>("GameClearUICanvas"));
                GameClearUICanvas canvas = clearui.GetComponent<GameClearUICanvas>();
                canvas.Initialize(this);
                canvas.StartClearTextAnimation();
                yield break;
            }
            yield return null;
        }
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
    }

    public override void OnConnected()
    {
        base.OnConnected();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
    }
}