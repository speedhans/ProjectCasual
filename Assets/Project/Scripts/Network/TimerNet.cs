using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TimerNet : MonoBehaviourPun, IPunObservable
{
    static TimerNet single = null;
    static TimerNet Instance
    {
        get
        {
            //if (single == null)
            //{
            //    _gameObject = new GameObject("TimerNet");
            //    single = _gameObject.AddComponent<TimerNet>();
            //    single.m_PhotonView = _gameObject.AddComponent<PhotonView>();
            //    single.m_PhotonView.ObservedComponents = new List<Component>();
            //    single.m_PhotonView.ObservedComponents.Add(single);
            //    single.m_PhotonView.Synchronization = ViewSynchronization.UnreliableOnChange;
            //    single.m_PhotonView.ViewID = 999;
            //}
            return single;
        }
        set
        {
            single = value;
        }
    }
    static GameObject _gameObject = null;

    static public void CreateDefaultTimer()
    {
        if (PhotonNetwork.IsMasterClient && single == null)
        {
            PhotonNetwork.InstantiateSceneObject("TimerNet", Vector3.zero, Quaternion.identity);
        }
    }

    public class TimerData
    {
        public string m_Name;
        public float m_CurrentTime;
        public float m_StartTime;
        public bool IsStop;
    }

    PhotonView m_PhotonView;
    List<TimerData> m_ListTimerData = new List<TimerData>();
    int m_StreamNumber;

    private void Awake()
    {
        _gameObject = gameObject;
        single = _gameObject.GetComponent<TimerNet>();
        m_PhotonView = GetComponent<PhotonView>();
    }

    //public void OnEnable()
    //{
    //    PhotonNetwork.AddCallbackTarget(this);
    //}

    //public void OnDisable()
    //{
    //    PhotonNetwork.RemoveCallbackTarget(this);
    //}

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            if (m_ListTimerData.Count < 1) return;

            stream.SendNext(m_StreamNumber);
            stream.SendNext(m_ListTimerData[m_StreamNumber].m_Name);
            stream.SendNext(m_ListTimerData[m_StreamNumber].m_CurrentTime);
            stream.SendNext(m_ListTimerData[m_StreamNumber].IsStop);

            ++m_StreamNumber;
            if (m_StreamNumber >= m_ListTimerData.Count)
                m_StreamNumber = 0;
        }
        else 
        {
            int number = (int)stream.ReceiveNext();
            string name = (string)stream.ReceiveNext();
            float time = (float)stream.ReceiveNext();
            bool stop = (bool)stream.ReceiveNext();

            FixedTimerData(number, name, time, stop);
        }
    }

    static public TimerData GetTimerData(string _Name)
    {
        return Instance.FindTimer(_Name);
    }

    static public float GetTimer(string _Name)
    {
        TimerData timer = Instance.FindTimer(_Name);
        if (timer == null) return 999999.9f;
        return timer.m_CurrentTime;
    }

    static public void InsertTimer(string _Name, float _StartTime, bool _IsStop = false)
    {
        if (PhotonNetwork.InRoom && !PhotonNetwork.IsMasterClient) return;
        Instance.m_PhotonView.RPC("InsertTimer_RPC", RpcTarget.AllBufferedViaServer, _Name, _StartTime, _IsStop);
    }
    
    [PunRPC]
    protected void InsertTimer_RPC(string _Name, float _StartTime, bool _IsStop = false)
    {
        TimerData timer = FindTimer(_Name);
        if (timer != null)
        {
            timer.m_Name = _Name;
            timer.m_StartTime = _StartTime;
            timer.m_CurrentTime = _StartTime;
            timer.IsStop = _IsStop;
            return;
        }

        AddTimer(_Name, _StartTime, _IsStop);
    }

    static public void SetTimer_s(string _Name, bool _IsStop)
    {
        Instance.SetTimer(_Name, _IsStop);
    }

    static public void SetTimer_s(string _Name, float _Time, bool _IsStop = false)
    {
        Instance.SetTimer(_Name, _Time, _IsStop);
    }

    static public void SetTimer_s(string _Name, float _Time, float _StartTime, bool _IsStop)
    {
        Instance.SetTimer(_Name, _Time, _StartTime, _IsStop);
    }

    protected void SetTimer(string _Name, bool _IsStop)
    {
        m_PhotonView.RPC("SetTimer1_RPC", RpcTarget.AllViaServer, _Name, _IsStop);
    }

    protected void SetTimer(string _Name, float _Time, bool _IsStop)
    {
        m_PhotonView.RPC("SetTimer2_RPC", RpcTarget.AllViaServer, _Name, _Time, _IsStop);
    }

    protected void SetTimer(string _Name, float _Time, float _StartTime, bool _IsStop)
    {
        m_PhotonView.RPC("SetTimer3_RPC", RpcTarget.AllViaServer, _Name, _Time, _StartTime, _IsStop);
    }

    [PunRPC]
    protected void SetTimer1_RPC(string _Name, bool _IsStop)
    {
        TimerData timer = FindTimer(_Name);
        if (timer != null)
        {
            timer.IsStop = _IsStop;
        }
    }

    [PunRPC]
    protected void SetTimer2_RPC(string _Name, float _Time, bool _IsStop)
    {
        TimerData timer = FindTimer(_Name);
        if (timer != null)
        {
            timer.m_CurrentTime = _Time;
            timer.IsStop = _IsStop;
        }
    }

    [PunRPC]
    protected void SetTimer3_RPC(string _Name, float _Time, float _StartTime, bool _IsStop)
    {
        TimerData timer = FindTimer(_Name);
        if (timer != null)
        {
            timer.m_CurrentTime = _Time;
            timer.m_StartTime = _StartTime;
            timer.IsStop = _IsStop;
        }
    }

    void AddTimer(string _Name, float _StartTime, bool _IsStop = false)
    {
        m_ListTimerData.Add(new TimerData() { m_Name = _Name, m_StartTime = _StartTime, m_CurrentTime = _StartTime, IsStop = _IsStop });
    }

    void FixedTimerData(int _Number, string _Name, float _Time, bool _IsStop)
    {
        if (m_ListTimerData.Count - 1 < _Number)
        {
            int interval = _Number - (m_ListTimerData.Count - 1);
            for (int i = 0; i < interval; ++i)
            {
                m_ListTimerData.Add(new TimerData() { m_Name = "", m_StartTime = 0.0f, m_CurrentTime = 0.0f, IsStop = true });
            }
        }

        m_ListTimerData[_Number].m_Name = _Name;
        if (Mathf.Abs(m_ListTimerData[_Number].m_CurrentTime - _Time) > 0.5f)
            m_ListTimerData[_Number].m_CurrentTime = _Time;
        m_ListTimerData[_Number].IsStop = _IsStop;
    }

    TimerData FindTimer(string _Name)
    {
        for (int i = 0; i < m_ListTimerData.Count; ++i)
        {
            if (m_ListTimerData[i].m_Name == _Name) return m_ListTimerData[i];
        }

        return null;
    }

    private void Update()
    {
        float deltatime = Time.deltaTime;

        for (int i = 0; i < m_ListTimerData.Count; ++i)
        {
            if (!m_ListTimerData[i].IsStop)
                m_ListTimerData[i].m_CurrentTime -= deltatime;
            if (m_ListTimerData[i].m_CurrentTime <= 0.0f)
            {
                m_ListTimerData[i].m_CurrentTime = 0.0f;
                m_ListTimerData[i].IsStop = true;
            }
        }
    }
}
