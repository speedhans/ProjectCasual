using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class LoadingManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    UnityEngine.UI.Slider m_Slider;
    [SerializeField]
    TMPro.TMP_Text m_TipText;
    private void Awake()
    {
        SceneManager.Instance.m_LoadingManager = this;

        LoadingTipData data = Resources.Load<LoadingTipData>("LoadingTipData");
        m_TipText.text = data.m_Tip[Random.Range(0, data.m_Tip.Length - 1)];
    }

    public void SetValue(float _Value)
    {
        m_Slider.value = Mathf.Clamp01(_Value);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);

        SceneManager.Instance.LoadSceneDirect("Intro");
    }
}
