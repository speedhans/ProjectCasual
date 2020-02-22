using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    static GameObject _gameObject = null;
    static NetworkManager single = null;
    static public NetworkManager Instance
    {
        get 
        {
            if (!single)
            {
                _gameObject = new GameObject("NetworkManager");
                single = _gameObject.AddComponent<NetworkManager>();
                single.Initialize();
            }
            return single;
        }
    }

    void Initialize()
    {
        DontDestroyOnLoad(gameObject);
    }

    RoomControl m_RoomControl;
    public RoomControl RoomController
    {
        get
        {
            if (m_RoomControl == null)
            {
                m_RoomControl = new RoomControl();
            }
            return m_RoomControl;
        }
        private set { }
    }

    static public TypedLobby m_DefaultLobby = new TypedLobby("GameLobby01", LobbyType.SqlLobby);
    string m_CreateRoomName;
    string m_NetVersion = "1.0";
    public string NetVersion { get { return m_NetVersion; } private set { } }
    public System.Action m_JoinLobbyCallback;
    public System.Action m_JoinRoomCallback;
    public System.Action<short, string> m_CreateRoomFailedCallback;
    public System.Action<short, string> m_JoinRoomFailedCallback;
    public System.Action<List<RoomInfo>> m_RoomUpdateCallback;
    public System.Action<ExitGames.Client.Photon.Hashtable> m_RoomPropertiesUpdateCallback;
    public System.Action<Player> m_MasterClientSwitchedCallback;

    public void ServerConnet(bool _Test = false)
    {
        if (PhotonNetwork.IsConnectedAndReady) return;
        if (_Test)
            PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion = m_NetVersion;
        else
            PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion = m_NetVersion + "test";

        PhotonNetwork.ConnectUsingSettings();
    }

    public void ServerDisconnect()
    {
        PhotonNetwork.Disconnect();
    }

    public void CreateRoom(string _RoomName, string _Filter, ExitGames.Client.Photon.Hashtable _DefaultPropertie = null, bool _IsOpen = true, bool _IsVisible = true, byte _MaxPlayer = 3)
    {
        RoomOptions op = new RoomOptions();
        op.IsOpen = _IsOpen;
        op.IsVisible = _IsVisible;
        op.MaxPlayers = _MaxPlayer;
        op.CustomRoomProperties = _DefaultPropertie;
        if (op.CustomRoomProperties != null)
        {
            op.CustomRoomProperties["C0"] = _Filter;
        }
        else
        {
            op.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "C0", _Filter } };
        }
        op.CustomRoomPropertiesForLobby = new string[] { "C0" };
        m_CreateRoomName = _RoomName;
        if (PhotonNetwork.CreateRoom(_RoomName, op, m_DefaultLobby))
            Debug.Log("Creaet Room: " + _RoomName);
        else
            Debug.Log("Failed Create Room");
    }

    public string CreateInstanceRoomName()
    {
        string RoomName = "";
        int copir = PhotonNetwork.CountOfPlayersInRooms;
        int cor = PhotonNetwork.CountOfRooms;

        string rs = Random.Range(1, 99).ToString();
        string nv = NetworkManager.Instance.NetVersion.Replace(".", "");

        int a = int.Parse(rs + nv + copir.ToString() + cor.ToString());
        int b = (int)(PhotonNetwork.Time * 0.5);
        RoomName = (a + b).ToString();
        return RoomName;
    }

    public bool JoinRoom(string _RoomName)
    {
        if (PhotonNetwork.JoinRoom(_RoomName)) return true;

        return false;
    }

    public bool GetRoomList(string _Filter)
    {
        string filter = "C0 = '" + _Filter + "'";
        if (PhotonNetwork.GetCustomRoomList(m_DefaultLobby, filter))
            return true;
        else
            return false;
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        m_RoomUpdateCallback?.Invoke(roomList);
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        m_RoomPropertiesUpdateCallback?.Invoke(propertiesThatChanged);
    }

    public class RoomControl
    {
        public void LeaveRoom()
        {
            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer);
                PhotonNetwork.LeaveRoom();
            }
        }

        public int PlayerCount { get { return PhotonNetwork.CurrentRoom.PlayerCount; } }

        public void SetRoomProperties(string _Keys, object _Values)
        {
            ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
            hash[_Keys] = _Values;

            PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
        }

        public void SetRoomProperties(string[] _Keys, object[] _Values)
        {
            ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
            for (int i = 0; i < _Keys.Length; ++i)
            {
                hash[_Keys[i]] = _Values[i];
            }

            PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
        }

        public T GetRoomPropertie<T>(string _Key)
        {
            object find = -1;
            PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(_Key, out find);
            return (T)find;
        }

        public void SetLocalPlayerProperties(string _Keys, object _Values)
        {
            ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
            hash[_Keys] = _Values;

            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }

        public void SetLocalPlayerProperties(string[] _Keys, object[] _Values)
        {
            ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
            for (int i = 0; i < _Keys.Length; ++i)
            {
                hash[_Keys[i]] = _Values[i];
            }

            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }

        public T GetLocalPlayerPropertie<T>(string _Key)
        {
            object find = null;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(_Key, out find))
                return (T)find;

            return default(T);
        }

        public T GetOtherPlayerPropertie<T>(Player _Player, string _Key)
        {
            object find = null;
            if (_Player.CustomProperties.TryGetValue(_Key, out find))
                return (T)find;

            return default(T);
        }

        public Object FindObjectWithPhotonViewID(int _ViewID)
        {
            PhotonView view = PhotonView.Find(_ViewID);
            if (view)
            {
                Object o = view.GetComponentInParent<Object>();
                if (o)
                {
                    return o;
                }
            }
            return null;
        }

        public T FindObjectWithPhotonViewID<T>(int _ViewID) where T : Object
        {
            PhotonView view = PhotonView.Find(_ViewID);
            if (view)
            {
                Object o = view.GetComponentInParent<Object>();
                if (o)
                {
                    return o as T;
                }
            }
            return null;
        }
    }

    // DefaultMessageCallback
    #region DefaultMessageCallback

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("OnConnectedToMaster");
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    public override void OnConnected()
    {
        base.OnConnected();
        Debug.Log("OnConnected");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        Debug.Log("Disconnected: " + cause.ToString());
    }

    public override void OnJoinedLobby()
    {
        PhotonNetwork.LocalPlayer.NickName = GameManager.Instance.m_TestMode ? "TestModePlayer" : GameManager.Instance.m_PlayerData.m_Name;
        m_JoinLobbyCallback?.Invoke();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("join room");
        m_JoinRoomCallback?.Invoke();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.Log("OnCreateRoomFailed");
        m_CreateRoomFailedCallback?.Invoke(returnCode, message);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        Debug.Log("OnJoinRoomFailed");
        m_JoinRoomFailedCallback?.Invoke(returnCode, message);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        base.OnMasterClientSwitched(newMasterClient);
        m_MasterClientSwitchedCallback?.Invoke(newMasterClient);
    }

    #endregion DefaultMessageCallback
}
