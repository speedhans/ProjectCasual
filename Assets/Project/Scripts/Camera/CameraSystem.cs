using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSystem : MonoBehaviour
{
    protected Camera m_Camera;

    protected virtual void Awake()
    {
        m_Camera = GetComponentInChildren<Camera>();
    }

    protected virtual void Start()
    {

    }
}
