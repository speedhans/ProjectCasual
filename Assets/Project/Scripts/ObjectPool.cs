using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    static GameObject _gameobject = null;
    static Dictionary<string, List<GameObject>> m_DicPool = new Dictionary<string, List<GameObject>>();
    static List<GameObject> m_ListObjectStatic;

    [SerializeField]
    List<GameObject> m_ListObject = new List<GameObject>();

    static public GameObject GetObject(string _ObjectName)
    {
        List<GameObject> list = null;
        if (m_DicPool.TryGetValue(_ObjectName, out list))
        {
            if (list.Count > 0)
            {
                GameObject o = list[0];
                o.transform.parent = null;
                list.RemoveAt(0);
                return o;
            }
        }

        for (int i = 0; i < m_ListObjectStatic.Count; ++i)
        {
            if (m_ListObjectStatic[i].name == _ObjectName)
            {
                GameObject o = Instantiate(m_ListObjectStatic[i].gameObject);
                o.name = _ObjectName;
                return o;
            }
        }

        return null;
    }

    static public T GetObject<T>(string _ObjectName)
    {
        List<GameObject> list = null;
        if (m_DicPool.TryGetValue(_ObjectName, out list))
        {
            if (list.Count > 0)
            {
                GameObject o = list[0];
                T t = o.GetComponent<T>();
                if (t == null)
                {
                    return default;
                }

                o.transform.parent = null;
                list.RemoveAt(0);

                return t;
            }
        }

        for (int i = 0; i < m_ListObjectStatic.Count; ++i)
        {
            if (m_ListObjectStatic[i].name == _ObjectName)
            {
                GameObject o = Instantiate(m_ListObjectStatic[i].gameObject);
                o.name = _ObjectName;

                T t = o.GetComponent<T>();
                if (t == null)
                {
                    Destroy(o);
                    return default;
                }
                return t;
            }
        }

        return default;
    }

    static public void PushObject(GameObject _Object)
    {
        List<GameObject> list = null;
        if (m_DicPool.TryGetValue(_Object.name, out list))
            list.Add(_Object);
        else
            m_DicPool.Add(_Object.name, new List<GameObject>() { _Object });

        _Object.transform.SetParent(_gameobject.transform);
        _Object.gameObject.SetActive(false);
    }

    private void Awake()
    {
        _gameobject = gameObject;

        if (m_ListObject.Count > 0)
        {
            m_ListObjectStatic = m_ListObject;
        }

        GameObject[] list = Resources.LoadAll<GameObject>("SkillEffect");
        m_ListObjectStatic.AddRange(list);
    }

    private void OnDestroy()
    {
        foreach (List<GameObject> o in m_DicPool.Values)
        {
            o.Clear();
        }
        m_DicPool.Clear();
    }
}
