using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int nDamage;
    public bool bIsMelee;
    public bool bIsRock;
    private void OnCollisionEnter(Collision collision)
    {
        if(!bIsRock && collision.gameObject.tag == "Floor")
        {
            Destroy(gameObject, 1f);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!bIsMelee && other.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }
}
