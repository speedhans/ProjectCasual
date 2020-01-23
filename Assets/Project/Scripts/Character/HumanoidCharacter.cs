using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidCharacter : Character
{
    public enum E_PARTS
    {
        HEAD,
        BODY,
        MAX
    }

    GameObject[] m_PartsObject = new GameObject[(int)E_PARTS.MAX];

    protected override void Awake()
    {
        base.Awake();
    }

    void AttachNewModel(Transform _MainModel, E_PARTS _PartsType, GameObject _Source)
    {
        if (m_PartsObject[(int)_PartsType] != null)
            Destroy(m_PartsObject[(int)_PartsType]);

        SkinnedMeshRenderer meshrenderer = _Source.GetComponentInChildren<SkinnedMeshRenderer>();

        FindBones(meshrenderer, _MainModel);
        meshrenderer.rootBone = FindRootBone(meshrenderer.bones, meshrenderer.rootBone.name);
        meshrenderer.transform.SetParent(_MainModel.transform);

        m_PartsObject[(int)_PartsType] = meshrenderer.gameObject;
    }

    void FindBones(SkinnedMeshRenderer _Skin, Transform _Model)
    {
        Transform[] find_bones = new Transform[_Skin.bones.Length];

        for (int i = 0; i < _Skin.bones.Length; i++)
        {
            find_bones[i] = FindModelBone(_Model, _Skin.bones[i].name);
        }

        _Skin.bones = find_bones;
    }

    Transform FindModelBone(Transform _Model, string _BoneName)
    {
        Transform find_bone = null;

        for (int i = 0; i < _Model.childCount; i++)
        {
            Transform t = _Model.GetChild(i);
            if (t.name == _BoneName) return t;

            t = FindModelBone(t, _BoneName);
            if (t != null) find_bone = t;
        }

        return find_bone;
    }

    Transform FindRootBone(Transform[] _Bones, string _BoneName)
    {
        foreach (Transform t in _Bones)
        {
            if (t.name == _BoneName) return t;
        }

        return null;
    }
}
