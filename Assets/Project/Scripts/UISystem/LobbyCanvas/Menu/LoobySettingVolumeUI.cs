using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoobySettingVolumeUI : LobbyUI
{
    const string m_BGMMuteKey = "BGMMute";
    const string m_BGMVolumeKey = "BGMVolume";
    const string m_EffectMuteKey = "EffectMute";
    const string m_EffectVolumeKey = "EffectVolume";

    [SerializeField]
    Slider m_BGMSlider;
    [SerializeField]
    Slider m_EffectSlider;

    [SerializeField]
    Image m_BGMSpeakerImage;
    [SerializeField]
    Image m_EffectSpeakerImage;
    [SerializeField]
    Sprite m_SpeakerImage;
    [SerializeField]
    Sprite m_MuteSpeakerImage;

    public override void Initialize(LobbyCanvasUI _LobbyCanvasUI)
    {
        base.Initialize(_LobbyCanvasUI);

        if (PlayerPrefs.GetInt(m_BGMMuteKey, 0) == 0)
        {
            m_BGMSpeakerImage.sprite = m_SpeakerImage;
            float volume = PlayerPrefs.GetFloat(m_BGMVolumeKey, 1.0f);
            m_BGMSlider.value = volume;
            SetBGMVolume(volume);
        }
        else
        {
            m_BGMSpeakerImage.sprite = m_MuteSpeakerImage;
            SoundManager.Instance.SetBGMVolume(0.0f);
        }

        if (PlayerPrefs.GetInt(m_EffectMuteKey, 0) == 0)
        {
            m_EffectSpeakerImage.sprite = m_SpeakerImage;
            float volume = PlayerPrefs.GetFloat(m_EffectVolumeKey, 1.0f);
            m_EffectSlider.value = volume;
            SetEffectVolume(volume);
        }
        else
        {
            m_EffectSpeakerImage.sprite = m_MuteSpeakerImage;
            SoundManager.Instance.SetEffectVolume(0.0f);
        }
    }

    public override void Open()
    {
        base.Open();
        gameObject.SetActive(true);
    }

    public override void Close()
    {
        base.Close();
        gameObject.SetActive(false);
    }

    public void BGMVolumeChangeEvent()
    {
        SetBGMVolume(m_BGMSlider.value);
    }

    public void EffectVolumeChangeEvent()
    {
        SetEffectVolume(m_EffectSlider.value);
    }

    public void SetBGMVolume(float _Volume)
    {
        SoundManager.Instance.SetBGMVolume(Mathf.Clamp01(_Volume));
        PlayerPrefs.SetFloat(m_BGMVolumeKey, _Volume);
    }

    public void SetEffectVolume(float _Volume)
    {
        SoundManager.Instance.SetEffectVolume(Mathf.Clamp01(_Volume));
        PlayerPrefs.SetFloat(m_EffectVolumeKey, _Volume);
    }

    public void BGMMuteButton()
    {
        if (PlayerPrefs.GetInt(m_BGMMuteKey, 0) == 0)
        {
            m_BGMSpeakerImage.sprite = m_MuteSpeakerImage;
            PlayerPrefs.SetInt(m_BGMMuteKey, 1);
            SoundManager.Instance.SetBGMVolume(0.0f);
        }
        else
        {
            m_BGMSpeakerImage.sprite = m_SpeakerImage;
            PlayerPrefs.SetInt(m_BGMMuteKey, 0);
            SetBGMVolume(PlayerPrefs.GetFloat(m_BGMVolumeKey, 1.0f));
        }
    }

    public void EffectMuteButton()
    {
        if (PlayerPrefs.GetInt(m_EffectMuteKey, 0) == 0)
        {
            m_EffectSpeakerImage.sprite = m_MuteSpeakerImage;
            PlayerPrefs.SetInt(m_EffectMuteKey, 1);
            SoundManager.Instance.SetEffectVolume(0.0f);
        }
        else
        {
            m_EffectSpeakerImage.sprite = m_SpeakerImage;
            PlayerPrefs.SetInt(m_EffectMuteKey, 0);
            SetEffectVolume(PlayerPrefs.GetFloat(m_EffectVolumeKey, 1.0f));
        }
    }
}
