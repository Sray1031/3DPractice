using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : Enemy
{
    public GameObject objectBossMissile;
    public Transform bossMissilePortA;
    public Transform bossMissilePortB;

    Vector3 vecLookVec;
    Vector3 vecTauntVec;
    public bool bIsLook;


    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        nav.isStopped = true;
        StartCoroutine(Think());
    }

    void Update()
    {
        if (bIsDead)
        {
            StopAllCoroutines();
            return;
        }
        if (bIsLook)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            vecLookVec = new Vector3(h, 0, v) * 5f;
            transform.LookAt(target.position + vecLookVec);
        }
        else
            nav.SetDestination(vecTauntVec);
    }

    IEnumerator Think()
    {
        yield return new WaitForSeconds(0.1f);

        int nRandAction = Random.Range(0, 5);
        switch(nRandAction)
        {
            case 0:
            case 1:
                //미사일 발사
                StartCoroutine(MissileShot());
                break;
            case 2:                
            case 3:
                //돌
                StartCoroutine(RockShot());
                break;
            case 4:
                //점프
                StartCoroutine(Tanut());
                break;
        }
    }
    
    IEnumerator MissileShot()
    {
        anim.SetTrigger("doShot");
        yield return new WaitForSeconds(0.2f);
        GameObject instantMissileA = Instantiate(objectBossMissile, bossMissilePortA.position, bossMissilePortA.rotation);
        BossMissile bossMissileA = instantMissileA.GetComponent<BossMissile>();
        bossMissileA.target = target;

        yield return new WaitForSeconds(0.3f);
        GameObject instantMissileB = Instantiate(objectBossMissile, bossMissilePortB.position, bossMissilePortB.rotation);
        BossMissile bossMissileB = instantMissileB.GetComponent<BossMissile>();
        bossMissileB.target = target;

        yield return new WaitForSeconds(2f);
        
        StartCoroutine(Think());
    }

    IEnumerator RockShot()
    {
        bIsLook = false;
        anim.SetTrigger("doBigShot");
        Instantiate(objectBullet, transform.position, transform.rotation);
        yield return new WaitForSeconds(3f);

        bIsLook = true;
        StartCoroutine(Think());        
    }

    IEnumerator Tanut()
    {
        vecTauntVec = target.position + vecLookVec;

        bIsLook = false;
        boxCollider.enabled = false;
        nav.isStopped = false;

        anim.SetTrigger("doTaunt");

        yield return new WaitForSeconds(1.5f);
        boxMeleeArea.enabled = true;

        yield return new WaitForSeconds(0.5f);
        boxMeleeArea.enabled = false;

        yield return new WaitForSeconds(1f);
        bIsLook = true;
        boxCollider.enabled = true;
        nav.isStopped = true;

        StartCoroutine(Think());
    }
}
