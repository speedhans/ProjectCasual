using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundClipComponent : MonoBehaviour
{
    public AudioClip m_Clip;
    [SerializeField]
    bool m_Loop = false;

    private void Awake()
    {
        if (!m_Loop) return;

        AudioSource audio = gameObject.AddComponent<AudioSource>();
        SoundManager.Instance.AddAudio(audio);
        audio.loop = true;
        audio.clip = m_Clip;
    }

    private void OnEnable()
    {
        if (m_Loop) return;

        SoundManager.Instance.PlayEffectSound(m_Clip);
    }
}
