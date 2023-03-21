using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    public Transform target;

    public Vector3 vecOffset;

    // Update is called once per frame
    void Update()
    {
        transform.position = target.position + vecOffset;
    }
}