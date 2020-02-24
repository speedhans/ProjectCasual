using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript2 : MonoBehaviour
{
    private void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.forward, out hit, 10.0f, -1))
        {
            Debug.Log(hit.transform.gameObject.name);
        }
        else
        {
            Debug.Log("no hit");
        }
    }
}
