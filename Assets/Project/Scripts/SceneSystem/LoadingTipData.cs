using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LoadingTipData", menuName = "CreateScriptableObject/LoadingTipData")]
public class LoadingTipData : ScriptableObject
{
    [TextArea]
    public string[] m_Tip;
}
