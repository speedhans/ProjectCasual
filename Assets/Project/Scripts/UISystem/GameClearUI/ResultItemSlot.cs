using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultItemSlot : MonoBehaviour
{
    UnityEngine.UI.Image m_Image;
    Animator m_Animator;

    Item m_Item;

    public void Initialize(Item _Item)
    {
        m_Image = transform.Find("Image").GetComponent<UnityEngine.UI.Image>();
        m_Animator = GetComponent<Animator>();
        m_Item = _Item;

        m_Image.sprite = m_Item.m_ItemImage;
    }

    public void StartAnimation()
    {
        m_Animator.speed = 1;
        m_Animator.SetTrigger("Run");
    }

    public void DirectResult()
    {
        m_Animator.speed = 100;
        m_Animator.SetTrigger("Run");
    }
}
