using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    public Transform target;
    public float fOrbitSpeed;
    Vector3 vecOffset;

    void Start()
    {
        vecOffset = transform.position - target.position;
    }
    // Update is called once per frame
    void Update()
    {
        transform.position = target.position + vecOffset;
        transform.RotateAround(target.position, Vector3.up, fOrbitSpeed * Time.deltaTime);
        vecOffset = transform.position - target.position;
    }
}
