using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    static GameObject _gameobject = null;
    static SoundManager single = null;
    public static SoundManager Instance
    {
        get
        {
            if (!single)
            {
                _gameobject = new GameObject("SoundManager");
                single = _gameobject.AddComponent<SoundManager>();
                single.Initialize();
            }

            return single;
        }

        private set { }
    }

    void Initialize()
    {
        DontDestroyOnLoad(gameObject);

        GameObject bgm = new GameObject("AudioBGM");
        bgm.transform.SetParent(gameObject.transform);
        m_AudioBGM = bgm.AddComponent<AudioSource>();

        GameObject touch = new GameObject("AudioTouch");
        touch.transform.SetParent(gameObject.transform);
        m_AudioTouch = touch.AddComponent<AudioSource>();

        for (int i = 0; i < 3; ++i)
        {
            GameObject g = new GameObject("Audio" + (i + 1).ToString());
            g.transform.SetParent(gameObject.transform);
            m_ListAudio.Add(g.AddComponent<AudioSource>());
            m_ListAudio[i].playOnAwake = false;
            m_ListAudio[i].Stop();
        }

        m_TouchSound = Resources.Load<AudioClip>("");
    }

    // 0 = BGM
    // 1 = Touch
    // 2 = AttackMoment
    // 3 ~ 4 = Effect
    AudioSource m_AudioBGM = null;
    AudioSource m_AudioTouch = null;
    List<AudioSource> m_ListAudio = new List<AudioSource>();
    AudioClip m_TouchSound;
    AudioClip m_BGMLoopClip;

    List<AudioClip> m_ListPlayerAttackMomentClip = new List<AudioClip>();

    public void AddAudio(AudioSource _Audio)
    {
        m_ListAudio.Add(_Audio);
    }

    public void RemoveAudio(AudioSource _Audio)
    {
        m_ListAudio.Remove(_Audio);
    }

    public void SetBGMVolume(float _Value)
    {
        m_AudioBGM.volume = _Value;
    }

    public void SetTouchVolume(float _Value)
    {
        m_AudioTouch.volume = _Value;
    }

    public void SetEffectVolume(float _Value)
    {
        foreach(AudioSource a in m_ListAudio)
        {
            a.volume = _Value;
        }
    }

    public void LoadSoundData()
    {
        m_ListPlayerAttackMomentClip.Clear();
        SoundClipData data = Resources.Load<SoundClipData>("SoundClipData");
        m_ListPlayerAttackMomentClip.AddRange(data.m_AttackHitSound);
    }

    public AudioClip GetPlayerAttackmonetClip(E_WEAPONTYPE _Type)
    {
        return m_ListPlayerAttackMomentClip[(int)_Type];
    }

    public void PlayBGM(AudioClip _StartClip, AudioClip _LoopClip)
    {
        StopBGM();
        m_AudioBGM.loop = false;
        m_AudioBGM.clip = _StartClip;
        m_AudioBGM.Play();
        m_BGMLoopClip = _LoopClip;

        StartCoroutine(C_PlayBGM());
    }

    IEnumerator C_PlayBGM()
    {
        while(true)
        {
            if (!m_AudioBGM.isPlaying)
            {
                m_AudioBGM.loop = true;
                m_AudioBGM.clip = m_BGMLoopClip;
                m_AudioBGM.Play();
                yield break;
            }
            yield return null;
        }
    }

    public void StopBGM()
    {
        m_ListAudio[0].Stop();
    }

    public void PlayTouchSound()
    {
        m_AudioTouch.PlayOneShot(m_TouchSound);
    }

    public void PlayAttackMomentSound(AudioClip _Clip)
    {
        m_ListAudio[0].PlayOneShot(_Clip);
    }

    public void PlayAttackMomentSound(E_WEAPONTYPE _Type)
    {
        m_ListAudio[0].PlayOneShot(m_ListPlayerAttackMomentClip[(int)_Type]);
    }

    public void PlayEffectSound(AudioClip _Clip)
    {
        m_ListAudio[1].PlayOneShot(_Clip);
    }
}
