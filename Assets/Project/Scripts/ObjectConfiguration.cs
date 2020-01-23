using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectConfiguration : MonoBehaviour
{
    public void TranslationObjectPosition(Transform _TargetObject, Vector3 _Position)
    {
        Vector3 fixedposition = Vector3.zero;

        float x = _Position.x % Common.BLOCK_SIZE;
        float y = _Position.y % Common.BLOCK_SIZE;
        float z = _Position.z % Common.BLOCK_SIZE;

        float halfsize = Common.BLOCK_SIZE * 0.5f;

        fixedposition.x = PositionValueRound(x, halfsize, _Position.x);
        fixedposition.y = PositionValueRound(y, halfsize, _Position.y);
        fixedposition.z = PositionValueRound(z, halfsize, _Position.z);

        _TargetObject.position = fixedposition;
    }

    float PositionValueRound(float _Value, float _Blockhalfsize, float _OriginalPosition)
    {
        if (_Value >= 0.0f)
        {
            if (_Value > _Blockhalfsize)
                return _OriginalPosition + Common.BLOCK_SIZE - _Value;
            else
                return _OriginalPosition - _Value;
        }
        else
        {
            if (_Value < -_Blockhalfsize)
                return _OriginalPosition + -Common.BLOCK_SIZE - _Value;
            else
                return _OriginalPosition - _Value;
        }
    }
}
