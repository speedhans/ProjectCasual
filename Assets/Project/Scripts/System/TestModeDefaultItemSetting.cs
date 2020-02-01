using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestModeDefaultItemSetting : MonoBehaviour
{
    [SerializeField]
    GameObject[] m_ListItems;

    private void Start()
    {
        GameObject item = null;
        for (int i = 0; i < m_ListItems.Length; ++i)
        {
            item = Instantiate(Resources.Load<GameObject>("Equipment/" + m_ListItems[i].name));
            if (i < 3)
            {
                EquipmentItem eitem = item.GetComponent<EquipmentItem>();
                eitem.m_UniqueID = Random.Range(0, 1000000);
                InventoryManager.Instance.InserItem(eitem);
                InventoryManager.Instance.SetEquipmentItem(i, eitem);
            }
            else
            {
                Item itemscript = item.GetComponent<Item>();
                itemscript.m_UniqueID = Random.Range(0, 1000000);
                InventoryManager.Instance.InserItem(item.GetComponent<Item>());
            }
        }
    }

}
