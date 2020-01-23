using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class TransformNet : StreamComponent, IPunObservable
{
    [SerializeField]
    bool m_StreamPosition = true;
    [SerializeField]
    float m_TeleportDistance = 3.0f;
    [SerializeField]
    bool m_StreamRotation = true;

    float m_Fraction;

    Vector3 m_FixedPosition;
    Quaternion m_FixedRotation;

    protected override void Start()
    {
        base.Start();
        m_FixedPosition = transform.position;
        m_FixedRotation = transform.rotation;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            if (m_StreamPosition)
                stream.SendNext(transform.position);
            if (m_StreamRotation)
                stream.SendNext(transform.rotation);

        }
        else
        {
            if (m_StreamPosition)
                m_FixedPosition = (Vector3)stream.ReceiveNext();
            if (m_StreamRotation)
                m_FixedRotation = (Quaternion)stream.ReceiveNext();

            m_Fraction = 0.0f;
            if ((transform.position - m_FixedPosition).magnitude >= m_TeleportDistance)
                transform.position = m_FixedPosition;
        }   
    }

    private void Update()
    {
        if (!enabled) return;
        if (!PhotonNetwork.IsConnectedAndReady) return;
        if (m_PhotonView.IsMine) return;

        float deltatime = Time.deltaTime;

        m_Fraction += deltatime * 7;
        if (m_StreamPosition)
        {
            transform.position = Vector3.Lerp(transform.position, m_FixedPosition, m_Fraction);
        }
        if (m_StreamRotation)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, m_FixedRotation, m_Fraction);
        }
    }
}
