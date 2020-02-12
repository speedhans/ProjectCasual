using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TestModeDefaultItemList", menuName = "TestModeDefaultItemList")]
public class TestModeDefaultItemSetting : ScriptableObject
{
    [SerializeField]
    GameObject[] m_ListItems;

    public void SetTestDefaultItemInventory()
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
