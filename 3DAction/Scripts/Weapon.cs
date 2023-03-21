using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type { Melee, Range };
    public Type eType;
    public int nDamege;
    public float fRate;

    public int nMaxAmmo;
    public int nCurAmmo;


    public BoxCollider boxMeleeArea;
    public TrailRenderer trailEffect;

    public Transform bulletPos;
    public GameObject objectBullet;
    public Transform bulletCasePos;
    public GameObject objectBulletCase;

    public void Use()
    {
        if(eType == Type.Melee)
        {
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }
        else if (eType == Type.Range && nCurAmmo > 0)
        {
            nCurAmmo--;
            StartCoroutine("Shot");

        }
    }

    IEnumerator Swing()
    {
        yield return new WaitForSeconds(0.1f);
        boxMeleeArea.enabled = true;
        trailEffect.enabled = true;

        yield return new WaitForSeconds(0.3f);
        boxMeleeArea.enabled = false;

        yield return new WaitForSeconds(0.3f);
        trailEffect.enabled = false;
    }

    IEnumerator Shot()
    {
        //Åº
        GameObject instantBullet = Instantiate(objectBullet, bulletPos.position, bulletPos.rotation);
        Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletPos.forward * 50;

        yield return null;

        //ÅºÇÇ
        GameObject instantBulletCase = Instantiate(objectBulletCase, bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody bulletCaseRigid = instantBulletCase.GetComponent<Rigidbody>();
        Vector3 caseVec = bulletCasePos.forward * Random.Range(-5, -3) + Vector3.up * Random.Range(3, 5);
        bulletCaseRigid.AddForce(caseVec, ForceMode.Impulse);
        bulletCaseRigid.AddTorque(Vector3.up * 30);

    }
}
