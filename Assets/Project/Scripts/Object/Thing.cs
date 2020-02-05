using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Thing : Object
{
    MeshRenderer[] m_MeshRenderers;
    Material m_OriginalMaterial;

    [SerializeField]
    float m_Hardness = 2.5f;

    public bool m_IsCollision { get; private set; }
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();

        m_MeshRenderers = GetComponentsInChildren<MeshRenderer>();
        m_OriginalMaterial = new Material(m_MeshRenderers[0].material);
        m_OriginalMaterial.shader = Shader.Find("Custom/ObejctGeometryShader");
        foreach (MeshRenderer r in m_MeshRenderers)
        {
            r.material = m_OriginalMaterial;
        }

        AddDamageEvent(DamageEvent);
    }

    protected override void Start()
    {
        base.Start();
        m_PhotonView.OwnershipTransfer = OwnershipOption.Takeover;
    }

    protected override void DamageEvent(float[] _Damage, float[] _CalculateDamage, int[] _DamageType, float _CalculateFullDamage, bool _Critical, Character _Attacker)
    {
    }

    protected void OnCollisionEnter(Collision collision) // 부딪칠때의 자해 데미지
    {
        if (gameObject.layer != Common.DYNAMIC_LAYER) return;
        if (PhotonNetwork.IsConnectedAndReady && !m_PhotonView.IsMine) return;

        float shock = m_BeforeVelocity.magnitude - m_Hardness;
        if (shock > 0.0f)
        {
            GiveToDamage(new float[] { shock }, new int[] { (int)E_DAMAGETYPE.WIND }, null);
        }
    }

    protected virtual void OnTriggerStay(Collider other)
    {
        if (gameObject.layer != Common.OPERATING_LAYER) return;
        if (m_IsCollision) return;
        if ((other.gameObject.layer != Common.STATIC_LAYER) && (other.gameObject.layer != Common.DYNAMIC_LAYER)) return;

        m_IsCollision = true;
        Material newMaterial = new Material(m_MeshRenderers[0].material);
        newMaterial.shader = Shader.Find("Custom/ObejctTransparentShader");
        newMaterial.SetColor("_Color", Color.red);
        foreach (MeshRenderer r in m_MeshRenderers)
        {
            r.material = newMaterial;
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (gameObject.layer != Common.OPERATING_LAYER) return;

        m_IsCollision = false;

        foreach (MeshRenderer r in m_MeshRenderers)
        {
            r.material = m_OriginalMaterial;
        }
    }
}
