using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class StreamComponent : MonoBehaviourPun
{
    protected PhotonView m_PhotonView;

    protected virtual void Start()
    {
        m_PhotonView = GetComponent<PhotonView>();
        if (m_PhotonView)
        {
            bool find = false;
            for (int i = 0; i < m_PhotonView.ObservedComponents.Count; ++i)
            {
                if (m_PhotonView.ObservedComponents[i] == null)
                {
                    m_PhotonView.ObservedComponents.RemoveAt(i);
                    continue;
                }
                if (m_PhotonView.ObservedComponents[i] == this)
                {
                    find = true;
                    break;
                }
            }

            if (!find)
            {
                m_PhotonView.ObservedComponents.Add(this);
                m_PhotonView.Synchronization = ViewSynchronization.UnreliableOnChange;
            }
        }
        else
        {
            this.enabled = false;
        }
    }

    public void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
}
