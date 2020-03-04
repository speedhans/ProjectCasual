using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TestModeDefaultItemList", menuName = "CreateScriptableObject/TestModeDefaultItemList")]
public class TestModeDefaultItemSetting : ScriptableObject
{
    [SerializeField]
    GameObject[] m_ListItems;
    [SerializeField]
    GameObject[] m_ListCharacterItems;
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

        bool firstcharacter = true;
        for (int i = 0; i < m_ListCharacterItems.Length; ++i)
        {
            item = Instantiate(Resources.Load<GameObject>("Character/" + m_ListCharacterItems[i].name));
            if (i < 3)
            {
                CharacterItem eitem = item.GetComponent<CharacterItem>();
                eitem.m_UniqueID = Random.Range(0, 1000000);
                InventoryManager.Instance.InserItem(eitem);
                if (firstcharacter)
                {
                    firstcharacter = false;
                    InventoryManager.Instance.SetEquipmentCharacterItem(eitem);
                }
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
