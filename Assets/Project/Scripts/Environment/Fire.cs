using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    [SerializeField]
    float m_FireDamage;
    int[] m_DamageType = new int[1] { (int)E_DAMAGETYPE.FIRE };

    float m_DamageDelay = 1.0f;

    Vector3 m_Overlap;

    public void Initialize()
    {
        m_DamageDelay = 1.0f;

        Collider c = transform.parent.GetComponentInChildren<Collider>();
        if (!c) ObjectPool.PushObject(gameObject);
        Transform parent = transform.parent;
        transform.parent = null;
        
        BoxCollider b = c as BoxCollider;
        if (b)
        {
            transform.localScale = CustomMath.MultiplyVector3(b.size, parent.localScale);
        }
        else
        {
            SphereCollider s = c as SphereCollider;
            if (s)
            {
                Vector3 scale = parent.localScale;
                float max = scale.x;
                if (scale.y > max)
                    max = scale.y;
                if (scale.z > max)
                    max = scale.z;
                float v = s.radius * 2.0f * max;
                transform.localScale = new Vector3(v, v, v);
            }
            else
            {
                CapsuleCollider cc = c as CapsuleCollider;
                if (cc)
                {
                    float v = cc.radius * 2.0f;
                    transform.localScale = CustomMath.MultiplyVector3(new Vector3(v, cc.height, v), parent.localScale) * 0.7f;
                }
                else
                {
                    MeshCollider mc = c as MeshCollider;
                    if (mc)
                    {
                        transform.localScale = c.bounds.size;
                    }
                }
            }
        }

        //transform.localScale = parent.lossyScale * 0.8f;
        m_Overlap = transform.localScale * 0.7f;
        transform.parent = parent;

        //Renderer r = transform.parent.GetComponentInChildren<Renderer>();
        //if (!r) ObjectPool.PushObject(gameObject);
        //Matrix4x4 matrix = transform.worldToLocalMatrix;
        //Vector3 scale = CustomMath.CalculateGlobalScale(matrix, transform.parent.lossyScale);
        //scale.x = Mathf.Abs(scale.x);
        //scale.y = Mathf.Abs(scale.y);
        //scale.z = Mathf.Abs(scale.z);
        //transform.localScale = scale;
        //m_Overlap = transform.lossyScale * 0.7f;
    }

    //private void Update()
    //{
    //    m_DamageDelay -= Time.deltaTime;

    //    if (m_DamageDelay <= 0.0f)
    //    {
    //        m_DamageDelay = 1.0f;

    //        Collider[] colls = Physics.OverlapBox(transform.position, m_Overlap, transform.rotation, Common.OBJECTLAYERMASK);
    //        float[] damage = new float[] { m_FireDamage };
    //        for (int i = 0; i < colls.Length; ++i)
    //        {
    //            Object o = colls[i].GetComponentInParent<Object>();
    //            if (o)
    //            {
    //                o.GiveToDamage(damage, m_DamageType, -1);
    //                if (o.CheckResistanceOver(E_DAMAGETYPE.FIRE))
    //                {
    //                    bool check = false;
    //                    for (int c = 0; c < colls[i].transform.childCount; ++c)
    //                    {
    //                        if (colls[i].transform.GetChild(c).name == gameObject.name)
    //                        {
    //                            check = true;
    //                            break;
    //                        }
    //                    }

    //                    if (!check)
    //                    {
    //                        Buff b = o.FindBuff(0);
    //                        if (b == null)
    //                        {
    //                            BuffManager.Instance.CreateBuff(o, 0);
    //                            string path = Common.FindTransformPath(colls[i].transform);
    //                            o.BuffDataUpdate(0, path);
    //                        }
    //                        else
    //                        {
    //                            string path = Common.FindTransformPath(colls[i].transform);
    //                            o.BuffDataUpdate(0, path);
    //                        }

    //                        //GameObject g = ObjectPool.GetObject(gameObject.name);
    //                        //if (g)
    //                        //{
    //                        //    g.SetActive(true);
    //                        //    g.transform.SetParent(colls[i].transform);
    //                        //    g.transform.localPosition = Vector3.zero;
    //                        //    g.transform.localRotation = Quaternion.identity;
    //                        //    Fire f = g.GetComponent<Fire>();
    //                        //    f.Initialize();
    //                        //    BuffBurn b = o.FindBuff(0) as BuffBurn;
    //                        //    if (b == null)
    //                        //    {
    //                        //        b = new BuffBurn(o, "Burn", 0, 9999999.0f);
    //                        //        b.m_ListFire.Add(f);
    //                        //        o.AddBuff(b);
    //                        //    }
    //                        //    else
    //                        //        b.m_ListFire.Add(f);
    //                        //}
    //                    }
    //                }
    //            }
    //        }
    //    }
    //}


    public void DestroyFire()
    {
        ObjectPool.PushObject(gameObject);
    }
    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.blue;
    //    Gizmos.DrawCube(transform.position, m_Overlap * 2);
    //}
}
