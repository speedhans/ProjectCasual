using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentSlotUI : CustomTouchEvent
{

    UnityEngine.UI.Image m_ItemImage;

    public void Initialize()
    {
        m_ItemImage = transform.Find("ItemImage").GetComponent<UnityEngine.UI.Image>();
    }

    public override void OnPointerDown(PointerEventData eventData) // 장비 목록 보는 작업 필요
    {
        base.OnPointerDown(eventData);
    }
}
