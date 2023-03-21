using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum Type { enemyA, enemyB, enemyC, enemyD };
    public Type eEnemyType;
    public int nMaxHealth;
    public int nCurHealth;
    public int nScore;
    
    public Transform target;
    public bool bIsChase;
    public BoxCollider boxMeleeArea;
    public bool bIsAttack;
    public GameObject objectBullet;

    public GameManager manager;

    public GameObject[] objectCoins;

    public bool bIsDead;

    public Rigidbody rigid;
    public BoxCollider boxCollider;
    public MeshRenderer[] meshs;
    public NavMeshAgent nav;
    public Animator anim;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        if(eEnemyType != Type.enemyD)
            Invoke("ChaseStart", 2f);
    }

    void ChaseStart()
    {
        bIsChase = true;
        anim.SetBool("isWalk", true);
    }

    private void Update()
    {
        if(bIsDead)
        {
            StopAllCoroutines();
            return;
        }

        if (nav.enabled && eEnemyType != Type.enemyD)
        {
            nav.SetDestination(target.position);
            nav.isStopped = !bIsChase;
        }
    }

    void FreezeVelocity()
    {
        if (bIsChase)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }
    
    void Targeting()
    {
        if (eEnemyType != Type.enemyD && !bIsDead)
        {
            float targetRadius = 0;
            float targetRange = 0;

            switch (eEnemyType)
            {
                case Type.enemyA:
                    targetRadius = 1.5f;
                    targetRange = 3f;
                    break;
                case Type.enemyB:
                    targetRadius = 1f;
                    targetRange = 15f;
                    break;
                case Type.enemyC:
                    targetRadius = 0.5f;
                    targetRange = 25f;
                    break;
            }

            RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player"));

            if (rayHits.Length > 0 && !bIsAttack)
            {
                StartCoroutine(Attack());
            }
        }
    }
    IEnumerator Attack()
    {
        bIsChase = false;
        bIsAttack = true;

        anim.SetBool("isAttack", true);        

        switch(eEnemyType)
        {
            case Type.enemyA:
                yield return new WaitForSeconds(0.2f);
                boxMeleeArea.enabled = true;

                yield return new WaitForSeconds(1f);
                boxMeleeArea.enabled = false;

                yield return new WaitForSeconds(1f);
                break;

            case Type.enemyB:
                yield return new WaitForSeconds(0.1f);
                rigid.AddForce(transform.forward * 20, ForceMode.Impulse);
                boxMeleeArea.enabled = true;

                yield return new WaitForSeconds(0.5f);
                rigid.velocity = Vector3.zero;
                boxMeleeArea.enabled = false;

                yield return new WaitForSeconds(2f);
                break;

            case Type.enemyC:
                yield return new WaitForSeconds(0.5f);
                GameObject instantBullet = Instantiate(objectBullet, transform.position, transform.rotation);
                Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
                rigidBullet.velocity = transform.forward * 20;

                yield return new WaitForSeconds(2f);
                break;

        }
        bIsChase = true;
        bIsAttack = false;

        anim.SetBool("isAttack", false);
    }

    private void FixedUpdate()
    {
        Targeting();
        FreezeVelocity();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            nCurHealth -= weapon.nDamege;
            Vector3 vecReactVec = transform.position - other.transform.position;
            StartCoroutine(OnDamage(vecReactVec, false));
        }
        else if (other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            nCurHealth -= bullet.nDamage;
            Vector3 vecReactVec = transform.position - other.transform.position;
            StartCoroutine(OnDamage(vecReactVec, false));
            Destroy(other.gameObject);
        }
    }

    IEnumerator OnDamage(Vector3 vecReactVec, bool bIsGrenade)
    {
        foreach(MeshRenderer mesh in meshs)
            mesh.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        if(nCurHealth > 0)
        {
            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = Color.white;
        }
        else
        {
            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = Color.gray;
            gameObject.layer = 12;

            bIsDead = true;
            anim.SetTrigger("doDie");
            bIsChase = false;
            nav.enabled = false;

            Player player = target.GetComponent<Player>();
            player.nScore += nScore;
            int ranCoin = Random.Range(0, 3);
            Instantiate(objectCoins[ranCoin], transform.position, Quaternion.identity);


            switch(eEnemyType)
            {
                case Type.enemyA:
                    manager.nEnemyCntA--;
                    break;
                case Type.enemyB:
                    manager.nEnemyCntB--;
                    break;
                case Type.enemyC:
                    manager.nEnemyCntC--;
                    break;
                case Type.enemyD:
                    manager.nEnemyCntD--;
                    break;
            }

            if(bIsGrenade)
            {
                vecReactVec = vecReactVec.normalized;
                vecReactVec += Vector3.up * 3;

                rigid.freezeRotation = false;
                rigid.AddForce(vecReactVec * 5, ForceMode.Impulse);
                rigid.AddTorque(vecReactVec * 15, ForceMode.Impulse);
            }
            else
            {
                vecReactVec = vecReactVec.normalized;
                vecReactVec += Vector3.up;
                rigid.AddForce(vecReactVec * 5, ForceMode.Impulse);
            }
            Destroy(gameObject, 3f);
        }
    }

    public void HitByGrenade(Vector3 explosionPos)
    {
        nCurHealth -= 100;
        Vector3 vecReactVec = transform.position - explosionPos;
        StartCoroutine(OnDamage(vecReactVec, true));
    }
}
