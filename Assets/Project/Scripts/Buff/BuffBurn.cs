using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BuffBurn : Buff
{
    public List<Fire> m_ListFire = new List<Fire>();
    public BuffBurn(Object _Self, string _BuffName, int _BuffID, float _LifeTime) : 
        base(_Self, _BuffName, _BuffID, _LifeTime)
    {
        AddDataUpdateAction(DataUpdateEvent);
    }

    protected override void Action(float _DeltaTime)
    {
        
    }

    void DataUpdateEvent(object _Value)
    {
        Transform t = m_ParentObject.transform.Find((string)_Value);
        if (t)
        {
            GameObject g = ObjectPool.GetObject("Fire");
            if (g)
            {
                g.SetActive(true);
                g.transform.SetParent(t);
                g.transform.localPosition = Vector3.zero;
                g.transform.localRotation = Quaternion.identity;
                Fire f = g.GetComponent<Fire>();
                f.Initialize();
                m_ListFire.Add(f);
            }
        }
    }

    public override void Destroy()
    {
        base.Destroy();

        foreach(Fire f in m_ListFire)
        {
            f.DestroyFire();
        }
        m_ListFire.Clear();
    }

    public override void DataUpdateEvent(object[] _Value)
    {
        throw new System.NotImplementedException();
    }
}
