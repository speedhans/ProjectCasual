using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingManager : MonoBehaviour
{
    [SerializeField]
    UnityEngine.UI.Slider m_Slider;

    private void Awake()
    {
        SceneManager.Instance.m_LoadingManager = this;
    }

    public void SetValue(float _Value)
    {
        m_Slider.value = Mathf.Clamp01(_Value);
    }
}
