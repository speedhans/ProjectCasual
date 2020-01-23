using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataBaseManager : MonoBehaviour
{
    static GameObject _gameObject = null;
    static DataBaseManager single = null;
    static public DataBaseManager Instance
    {
        get
        {
            if (!single)
            {
                _gameObject = new GameObject("DataBaseManager");
                single = _gameObject.AddComponent<DataBaseManager>();
                single.Initialize();
            }
            return single;
        }
    }

    void Initialize()
    {
        DontDestroyOnLoad(gameObject);
    }

    public List<Item> GetItemDataList()
    {
        return null;
    }
}
