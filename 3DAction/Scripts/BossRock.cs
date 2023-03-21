using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRock : Bullet
{
    Rigidbody rigid;
    float fAngularPower = 2f;
    float fScaleValue = 0.1f;
    bool bIsShoot;
    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        StartCoroutine(GainPowerTimer());
        StartCoroutine(GainPower());

    }

    IEnumerator GainPowerTimer()
    {
        yield return new WaitForSeconds(2.2f);
        bIsShoot = true;
    }
    IEnumerator GainPower()
    {
        while(!bIsShoot)
        {
            fAngularPower += 0.08f;
            fScaleValue += 0.01f;
            transform.localScale = Vector3.one * fScaleValue;
            rigid.AddTorque(transform.right * fAngularPower, ForceMode.Acceleration);
            yield return null;

        }
    }

}
