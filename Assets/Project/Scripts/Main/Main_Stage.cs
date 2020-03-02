using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class Main_Stage : Main
{
    public const string MultiPlayKey = "MultiKey";

    public enum E_GAMESEQUENCE
    {
        BEGINGAME,
        READY,
        START,
        NORMALSTAGESTART,
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
    public bool m_IsMultiplayGame { get; private set; }

    PlayerCharacter m_MyCharacter;
    public Shader m_DissolveShader;
    public Material m_LocalOutLineMaterial;
    public Material m_OtherOutLineMaterial;

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
    QuestData.S_DropItemData[] m_DropItemList;

    [SerializeField]
    Transform m_BossRoomStartPoint;

    [HideInInspector]
    public SpawnManager m_SpawnManager;

    protected List<List<Character>> m_ListCharacters = new List<List<Character>>();
    public GameSceneData m_GameSceneData { get; private set; }
    protected override void Awake()
    {
        base.Awake();

        for (int i = 0; i < (int)E_TEAMTYPE.MAX; ++i)
            m_ListCharacters.Add(new List<Character>());

        if (PhotonNetwork.IsMasterClient)
        {
            // timer set
            TimerNet.InsertTimer(m_SequenceTimerKey, 0.0f, true);
        }

        gameObject.AddComponent<FrameChecker>();
        Instantiate(m_StageDefaultCamera, Vector3.zero, Quaternion.identity);
        Resources.Load<Shader>(m_DissolveShader.name);
        m_GameSceneData = Resources.Load<GameSceneData>(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name + "_data");
        if (m_GameSceneData)
        {
            m_DropItemList = m_GameSceneData.m_DropItemList.ToArray();
        }

        StartCoroutine(C_Initialize());
    }

    void TestModeServerConnect()
    {
        StartCoroutine(C_TestModeServerConnect());
    }

    IEnumerator C_TestModeServerConnect()
    {
        SoundManager.Instance.LoadSoundData();
        NetworkManager.Instance.ServerConnet();
        while (!PhotonNetwork.IsConnectedAndReady || !PhotonNetwork.InLobby) yield return null;

        ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
        hash[MultiPlayKey] = false;
        NetworkManager.Instance.CreateRoom("TESTROOM01", "test01", hash);

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
        List<Item> droplist = new List<Item>();
        for (int i = 0; i < m_DropItemList.Length; ++i)
        {
            droplist.Add(m_DropItemList[i].m_Item);
        }
        m_GameUICanvas.Initialize(this, droplist);
        m_GameUICanvas.TimeCheck(false);
        gameUI.SetActive(false);
        m_SkillUICanvas = Instantiate(Resources.Load<GameObject>("SkillUICanvas"), Vector3.zero, Quaternion.identity);
        m_SkillUICanvas.SetActive(false);
        GameObject bossUI = Instantiate(Resources.Load<GameObject>("BossUICanvas"));
        m_BossUICanvas = bossUI.GetComponent<BossCharacterUI>();
        bossUI.SetActive(false);
        m_SpawnManager.Initialize();

        GameObject g = PhotonNetwork.Instantiate("PlayerCharacter", Vector3.zero, Quaternion.identity, 0, new object[] { -1, -1, -1, -1, -1, InventoryManager.Instance.GetPlayerModelName() });
        m_MyCharacter = g.GetComponent<PlayerCharacter>();
        m_MyCharacter.AddDeadEvent(MyCharacterDeadEvent);

        m_VerticalCamera = Instantiate(m_VerticalCamera, Vector3.zero, Quaternion.identity);
        VerticalFollowCamera verticalcamera = m_VerticalCamera.GetComponent<VerticalFollowCamera>();
        verticalcamera.Initialize(this, m_MyCharacter);
        m_VerticalCamera.SetActive(false);

        NetworkManager.Instance.RoomController.SetLocalPlayerProperties("InitComp", true);

        yield return null;

        while (!IsBeginLoadingComplete)
        {
            Player[] players = PhotonNetwork.PlayerList;
            int compcount = 0;
            int playercount = players.Length;

            foreach (Player p in players)
            {
                if (NetworkManager.Instance.RoomController.GetOtherPlayerPropertie<bool>(p, "InitComp"))
                    ++compcount;
            }

            if (compcount >= playercount)
            {
                IsBeginLoadingComplete = true;
            }
            yield return null;
        }

        m_IsMultiplayGame = NetworkManager.Instance.RoomController.GetRoomPropertie<bool>(MultiPlayKey);

        while (!IsAfterLoadingComplete)
        {
            Player[] players = PhotonNetwork.PlayerList;

            bool complete = true;
            for (int i = 0; i < players.Length; ++i)
            {
                if (!NetworkManager.Instance.RoomController.GetOtherPlayerPropertie<bool>(players[i], PlayerCharacter.m_CharacterReadyKey))
                {
                    complete = false;
                }
            }
            if (complete)
                InitializeComplete();
            yield return null;
        }
    }

    void InitializeComplete()
    {
        StageDefaultCamera.Instance.DestroyCamera();
        m_VerticalCamera.SetActive(true);
        m_GameUICanvas.gameObject.SetActive(true);
        IsAfterLoadingComplete = true;
    }

    private void Update()
    {
        if (!IsAfterLoadingComplete) return;

        if (m_SequenceProgress[(int)m_GameSequence]) return;

        float timer = TimerNet.GetTimer(m_SequenceTimerKey);

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
                        SetNextGameSequence();
                    }
                }
                break;
            case E_GAMESEQUENCE.NORMALSTAGESTART:
                {
                    m_SpawnManager.NormalMonsterSpawnStart();
                    m_ControlCanvas.SetActive(true);
                    m_SkillUICanvas.SetActive(true);
                    m_GameUICanvas.TimeCheck(true);
                    SetNextGameSequence();
                }
                break;
            case E_GAMESEQUENCE.NORMALSTAGE:
                {
                    if (m_SpawnManager.IsAllNormalMonsterSpawnOperationsCompleted())
                    {
                        SetNextGameSequence(1.5f);
                    }
                }
                break;
            case E_GAMESEQUENCE.NORMALSTAGE_END:
                {
                    if (timer <= 0.0f)
                    {
                        m_PianoEffect.CurtainClose();
                        SetNextGameSequence(2.0f);
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
                        DeleteTeamAllUnits(E_TEAMTYPE.BLUE);
                        DeleteTeamAllUnits(E_TEAMTYPE.GREEN);
                        DeleteTeamAllUnits(E_TEAMTYPE.YELLOW);
                        GameClearUIOpen();
                        m_GameUICanvas.TimeCheck(false);
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

    void SetNextGameSequence(float _WaitTime = 0.0f)
    {
        m_SequenceProgress[(int)m_GameSequence] = true;
        SetGameSequence(m_GameSequence + 1, _WaitTime);
    }

    void SetGameSequence(E_GAMESEQUENCE _Sequence, float _WaitTime)
    {
        if (PhotonNetwork.IsMasterClient)
            m_PhotonView.RPC("SetGameSequence_RPC", RpcTarget.AllViaServer, (int)_Sequence, _WaitTime);
    }

    [PunRPC]
    void SetGameSequence_RPC(int _Sequence, float _WaitTime)
    {
        m_SequenceProgress[(int)m_GameSequence] = true;
        m_GameSequence = (E_GAMESEQUENCE)_Sequence;
        TimerNet.SetTimer_s(m_SequenceTimerKey, _WaitTime);
    }

    public E_GAMESEQUENCE GetCurrentSequence() { return m_GameSequence; }

    public Item[] GetResultItemlist() // 임시. 네트워크 방식으로 변경해야함
    {
        List<Item> list = new List<Item>();
        for (int i = 0; i < m_DropItemList.Length; ++i)
        {
            int random = Random.Range(0, 100);
            if (m_DropItemList[i].m_DropChance >= random)
                list.Add(Instantiate(m_DropItemList[i].m_Item).GetComponent<Item>());
        }
        return list.ToArray();
    }

    public void GameClearUIOpen()
    {
        m_ControlCanvas.SetActive(false);
        m_SkillUICanvas.SetActive(false);
        GameObject clearui = Instantiate(Resources.Load<GameObject>("GameClearUICanvas"));
        GameClearUICanvas canvas = clearui.GetComponent<GameClearUICanvas>();
        canvas.Initialize(this, 2.0f);
        canvas.StartClearTextAnimation();
    }

    public void InsertTeamUnit(E_TEAMTYPE _Type, Character _Character)
    {
        m_ListCharacters[(int)_Type].Add(_Character);
    }

    public void RemoveTeamUnit(E_TEAMTYPE _Type, Character _Character)
    {
        m_ListCharacters[(int)_Type].Remove(_Character);
    }

    public List<Character> GetTeamUnits(E_TEAMTYPE _Type)
    {
        return m_ListCharacters[(int)_Type];
    }

    public void DeleteTeamAllUnits(E_TEAMTYPE _Type, bool _Network = true)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        Character[] c = m_ListCharacters[(int)_Type].ToArray();
        for (int i = 0; i < c.Length; ++i)
        {
            c[i].ObjectDestroyTimer();
        }
        m_ListCharacters[(int)_Type].Clear();
    }

    void ClearTeamUnitDate()
    {
        for (int i = 0; i < m_ListCharacters.Count; ++i)
            m_ListCharacters[i].Clear();
    }

    void MyCharacterDeadEvent()
    {
        m_GameUICanvas.GetDeadUI().Enable();
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