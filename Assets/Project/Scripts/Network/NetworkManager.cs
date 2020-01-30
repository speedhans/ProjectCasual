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

    string m_CreateRoomName;

    public void ServerConnet()
    {
        if (PhotonNetwork.IsConnectedAndReady) return;
        PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion = "1.0";
        
        PhotonNetwork.ConnectUsingSettings();
    }

    public void CreateRoom(string _RoomName, ExitGames.Client.Photon.Hashtable _DefaultPropertie = null, bool _IsOpen = true, bool _IsVisible = true, byte _MaxPlayer = 0)
    {
        RoomOptions op = new RoomOptions();
        op.IsOpen = _IsOpen;
        op.IsVisible = _IsVisible;
        op.MaxPlayers = _MaxPlayer;
        op.CustomRoomProperties = _DefaultPropertie;
        m_CreateRoomName = _RoomName;
        if (PhotonNetwork.CreateRoom(_RoomName, op, TypedLobby.Default))
            Debug.Log("Creaet Room: " + _RoomName);
        else
            Debug.Log("Failed Create Room");
    }

    public bool JoinRoom(string _RoomName)
    {
        if (PhotonNetwork.JoinRoom(_RoomName)) return true;

        return false;
    }

    public class RoomControl
    {
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
            object find = -1;
            PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(_Key, out find);
            return (T)find;
        }

        public T GetOtherPlayerPropertie<T>(Player _Player, string _Key)
        {
            object find = -1;
            _Player.CustomProperties.TryGetValue(_Key, out find);
            return (T)find;
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
        base.OnJoinedLobby();
        Debug.Log("OnJoinedLobby");
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("OnJoinedRoom");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.Log("OnCreateRoomFailed");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        Debug.Log("OnJoinRoomFailed");
    }

    #endregion DefaultMessageCallback
}
