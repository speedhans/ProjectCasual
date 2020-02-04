using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[System.Serializable]
public struct S_RESISTANCE
{
    public float Fire;
    public float Ice;
    public float Electric;
    public float Wind;
    public float Light;
    public float Dark;
}

public enum E_DAMAGETYPE
{
    FIRE,       // 화염
    ICE,        // 얼음
    ELECTRIC,   // 전기
    WIND,        // 바람
    LIGHT,      // 때리기
    DARK,     // 찌르기
    MAX,
}

public enum E_ANIMATION
{
    NONE,
    IDLE,
    WALK,
    RUN,
    DEAD,
    ATTACK,
    SPECIAL1,
    SPECIAL2,
    SPECIAL3,
    HIT,
}

public enum E_TEAMTYPE
{
    RED,
    BLUE,
    GREEN,
    YELLOW
}

public enum E_WEAPONTYPE
{
    NONE,
    PUNCH,
    ONEHANDSWORD,
    TWOHANDSWORD,
}

public class Common
{
    static public Color[] m_DamageColor = new Color[(int)E_DAMAGETYPE.MAX] {
        Color.red, new Color(0.2f, 0.3f, 1.0f), Color.yellow,
        new Color(0.8f, 0.8f, 0.8f), new Color(0.2f, 0.2f, 0.2f), new Color(0.7f, 0.3f, .7f) };

    static public float BLOCK_SIZE = 0.5f;
    static public int STATIC_LAYER = 10;
    static public int DYNAMIC_LAYER = 11;
    static public int ATTACHED_LAYER = 12;
    static public int OPERATING_LAYER = 13;
    static public int CHARACTER_LAYER = 14;

    static public int OBJECTLAYERMASK = 1 << STATIC_LAYER | 1 << DYNAMIC_LAYER | 1 << ATTACHED_LAYER | 1 << CHARACTER_LAYER;

    public const int MAXREINFORECEVALUE = 4;

    static public Texture2D CreateNewTextureAtlas(Texture2D[] _Source, int _SizeXY)
    {
        Texture2D texture = new Texture2D(_SizeXY, _SizeXY, TextureFormat.ARGB32, true);
        texture.PackTextures(_Source, 0, _SizeXY);
        return texture;
    }

    static public Color GetTargetTexturePointColor(Vector3 _Location, int _TargetLayerMask, Vector3 _RayDirection)
    {
        RaycastHit hit;
        if(Physics.Raycast(_Location, _RayDirection, out hit, 10.0f, _TargetLayerMask))
        {
            Renderer renderer = hit.transform.GetComponent<Renderer>();
            Texture2D texture = (Texture2D)renderer.material.mainTexture;

            Vector2 coord = hit.textureCoord;
            coord.x *= texture.width;
            coord.y *= texture.height;

            Vector2 tiling = renderer.material.mainTextureScale;

            return texture.GetPixel((int)(coord.x * tiling.x), (int)(coord.y * tiling.y));
        }

        return Color.black;
    }

    static public string FindTransformPath(Transform _Transform)
    {
        string path = "";
        CombineParentsPath(_Transform, _Transform.name, out path);
        return path;
    }

    static void CombineParentsPath(Transform _Transform, string _Path, out string _Out)
    {
        if (_Transform.parent == null || _Transform.parent.parent == null)
        {
            _Out = _Path;
            return;
        }

        CombineParentsPath(_Transform.parent, _Transform.parent.name + "/" + _Path, out _Out);
    }

    static public Object FindObject(int _ID)
    {
        if (PhotonNetwork.IsConnectedAndReady)
            return NetworkManager.Instance.RoomController.FindObjectWithPhotonViewID(_ID);
        else
        {
            //GameObject.Find();
        }
        return default;
    }
}
