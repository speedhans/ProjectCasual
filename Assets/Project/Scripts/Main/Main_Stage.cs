﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class Main_Stage : Main
{
    public enum E_GAMESEQUENCE
    {
        BEGINGAME,
        READY,
        START,
        NORMALSTAGE,
        NORMALSTAGE_END,
        BOSSSTAGESTART,
        BOSSSTAGE,
        BOSSSTAGE_END,
        AFTERGAME,
        MAX
    }

    E_GAMESEQUENCE m_GameSequence = E_GAMESEQUENCE.BEGINGAME;
    bool[] m_SequenceProgress = new bool[(int)E_GAMESEQUENCE.MAX];
    string m_SequenceTimerKey = "GameSequence";

    PlayerCharacter m_MyCharacter;
    public Shader m_DissolveShader;

    PianoEffectCanvas m_PianoEffect;
    GameReadyCanvas m_GameReadyCanvas;
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
    Transform m_BossRoomStartPoint;

    [HideInInspector]
    public SpawnManager m_SpawnManager;

    protected override void Awake()
    {
        base.Awake();
        
        if (PhotonNetwork.IsMasterClient)
        {
            // timer set
            TimerNet.InsertTimer(m_SequenceTimerKey, 0.0f, true);
        }

        gameObject.AddComponent<FrameChecker>();
        Instantiate(m_StageDefaultCamera, Vector3.zero, Quaternion.identity);
        Resources.Load<Shader>(m_DissolveShader.name);

        StartCoroutine(C_Initialize());
    }

    void TestModeServerConnect()
    {
        StartCoroutine(C_TestModeServerConnect());
    }

    IEnumerator C_TestModeServerConnect()
    {
        NetworkManager.Instance.ServerConnet();
        while (!PhotonNetwork.IsConnectedAndReady || !PhotonNetwork.InLobby) yield return null;
        NetworkManager.Instance.CreateRoom("TESTROOM01");

    }

    IEnumerator C_Initialize()
    {
        if (GameManager.Instance.m_TestMode)
        {
            TestModeDefaultItemSetting list = Resources.Load<TestModeDefaultItemSetting>("TestModeDefaultItemList");
            list.SetTestDefaultItemInventory();

            TestModeServerConnect();
            while (!PhotonNetwork.InRoom) yield return null;
        }

        if (PhotonNetwork.IsMasterClient)
        {
            // timer set
            TimerNet.InsertTimer(m_SequenceTimerKey, 0.0f, true);
        }

        yield return null;

        GameObject pianoUI = Instantiate(Resources.Load<GameObject>("PianoEffectCanvas"));
        m_PianoEffect = pianoUI.GetComponent<PianoEffectCanvas>();
        m_PianoEffect.Initialize();
        GameObject readyUI = Instantiate(Resources.Load<GameObject>("GameReadyCanvas"));
        m_GameReadyCanvas = readyUI.GetComponent<GameReadyCanvas>();
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
        m_SpawnManager.Initialize();

        GameObject g = PhotonNetwork.Instantiate("PlayerCharacter", Vector3.zero, Quaternion.identity, 0, new object[] { -1, -1, -1, -1, -1, InventoryManager.Instance.GetPlayerModelName() });
        m_MyCharacter = g.GetComponent<PlayerCharacter>();

        m_VerticalCamera = Instantiate(m_VerticalCamera, Vector3.zero, Quaternion.identity);
        VerticalFollowCamera verticalcamera = m_VerticalCamera.GetComponent<VerticalFollowCamera>();
        verticalcamera.Initialize(this, m_MyCharacter);
        m_VerticalCamera.SetActive(false);

        NetworkManager.Instance.RoomController.SetLocalPlayerProperties("InitComp", true);

        yield return null;

        while (true)
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
                InitializeComplete();
                yield break;
            }

            yield return null;
        }
    }

    void InitializeComplete()
    {
        StageDefaultCamera.Instance.DestroyCamera();
        m_VerticalCamera.SetActive(true);
        m_GameUICanvas.gameObject.SetActive(true);
        IsLoadingComplete = true;

        TimerNet.SetTimer_s(m_SequenceTimerKey, 1.0f, false);
    }

    private void Update()
    {
        if (!IsLoadingComplete) return;

        float timer = TimerNet.GetTimer(m_SequenceTimerKey);

        if (m_SequenceProgress[(int)m_GameSequence]) return;
        switch (m_GameSequence)
        {
            case E_GAMESEQUENCE.BEGINGAME:
                {
                    if (timer <= 0.0f)
                    {
                        SetNextGameSequence();
                    }
                }
                break;
            case E_GAMESEQUENCE.READY:
                {
                    m_GameReadyCanvas.Run();
                    SetNextGameSequence();
                }
                break;
            case E_GAMESEQUENCE.START:
                {
                    if (m_GameReadyCanvas.IsComplete)
                    {
                        m_SpawnManager.NormalMonsterSpawnStart();
                        m_ControlCanvas.SetActive(true);
                        m_SkillUICanvas.SetActive(true);
                        SetNextGameSequence();
                    }
                }
                break;
            case E_GAMESEQUENCE.NORMALSTAGE:
                {
                    if (m_SpawnManager.IsAllNormalMonsterSpawnOperationsCompleted())
                    {
                        TimerNet.SetTimer_s(m_SequenceTimerKey, 1.5f);
                        SetNextGameSequence();
                    }
                }
                break;
            case E_GAMESEQUENCE.NORMALSTAGE_END:
                {
                    if (timer <= 0.0f)
                    {
                        m_PianoEffect.CurtainClose();
                        TimerNet.SetTimer_s(m_SequenceTimerKey, 2.0f);
                        SetNextGameSequence();
                    }
                }
                break;
            case E_GAMESEQUENCE.BOSSSTAGESTART:
                {
                    if (timer <= 0.0f)
                    {
                        m_PianoEffect.CurtainOpen();
                        m_BossUICanvas.gameObject.SetActive(true);
                        m_MyCharacter.transform.position = m_BossRoomStartPoint.position;
                        m_MyCharacter.transform.rotation = Quaternion.identity;
                        VerticalFollowCamera.SetDistanceSmooth(5.0f, 1.5f);
                        SetNextGameSequence();
                    }
                }
                break;
            case E_GAMESEQUENCE.BOSSSTAGE:
                {
                    m_SpawnManager.BossMonsterSpawnStart();
                    SetNextGameSequence();
                }
                break;
            case E_GAMESEQUENCE.BOSSSTAGE_END:
                {
                    if (m_SpawnManager.IsAllBossMonsterSpawnOperationsCompleted())
                    {
                        GameClearUIOpen();
                        SetNextGameSequence();
                    }
                }
                break;
            case E_GAMESEQUENCE.AFTERGAME:
                {
                    m_SequenceProgress[(int)m_GameSequence] = true;
                }
                break;
            default:
                break;
        }
    }

    void SetNextGameSequence()
    {
        m_SequenceProgress[(int)m_GameSequence] = true;
        SetGameSequence(m_GameSequence + 1);
    }

    void SetGameSequence(E_GAMESEQUENCE _Sequence)
    {
        if (PhotonNetwork.IsMasterClient)
            m_PhotonView.RPC("SetGameSequence_RPC", RpcTarget.AllViaServer, (int)_Sequence);
    }

    [PunRPC]
    void SetGameSequence_RPC(int _Sequence)
    {
        m_SequenceProgress[(int)m_GameSequence] = true;
        m_GameSequence = (E_GAMESEQUENCE)_Sequence;
    }

    public Item[] GetResultItemlist() // 임시. 네트워크 방식으로 변경해야함
    {
        List<Item> list = new List<Item>();
        for (int i = 0; i < m_ResultItemlist.Length; ++i)
        {
            list.Add(Instantiate(m_ResultItemlist[i].gameObject).GetComponent<Item>());
        }

        InventoryManager.Instance.InserItem(list);

        return list.ToArray();
    }

    public void GameClearUIOpen()
    {
        m_ControlCanvas.SetActive(false);
        m_SkillUICanvas.SetActive(false);
        GameObject clearui = Instantiate(Resources.Load<GameObject>("GameClearUICanvas"));
        GameClearUICanvas canvas = clearui.GetComponent<GameClearUICanvas>();
        canvas.Initialize(this);
        canvas.StartClearTextAnimation();
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


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (m_BossRoomStartPoint == null) return;

        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(m_BossRoomStartPoint.position + new Vector3(0.0f, 0.5f, 0.0f), Vector3.one);
    }
#endif
}