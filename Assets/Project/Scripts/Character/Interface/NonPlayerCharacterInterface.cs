using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface NonPlayerCharacterInterface
{
    void SetPatrolPath(List<Transform> _Path);
    void RemovePatrolPath();
    bool PathPointReachCheck();
}
