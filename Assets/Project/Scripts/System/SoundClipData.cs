using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundClipData", menuName = "CreateScriptableObject/SoundClipData")]
public class SoundClipData : ScriptableObject
{
    public AudioClip m_LobbyBGM;
    public AudioClip[] m_AttackHitSound = new AudioClip[(int)E_WEAPONTYPE.MAX];
}
