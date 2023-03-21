using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    float fHorizonAxis;
    float fVerticalAxis;
    public float fSpeed;
    public float fJumpPower;
    public GameObject[] objectWeapon;
    public bool[] bHasWeapon;
    public GameObject[] objectGrenades;
    public int nHasGrenades;
    public GameObject objectGrenade;
    public GameManager manager;

    public Camera camFollowCamera;

    public int nAmmo;
    public int nCoin;
    public int nHealth;
    public int nScore;

    public int nMaxAmmo;
    public int nMaxCoin;
    public int nMaxHealth;
    public int nMaxHasGrenades;

    bool bWalkDown;
    bool bJumpDown;
    bool bIsJump;
    bool bIsDodge;
    bool bFireDown;
    bool bIsFireReady;
    bool bReloadDown;
    bool bGrenadeDown;

    bool bSwapFirst;
    bool bSwapSecond;
    bool bSwapThird;
    bool bIsSwap;
    bool bIsReload;
    bool bIsBorder;
    bool bIsDamage;
    bool bIsShopping;
    bool bIsDead;

    int nEquipWeaponIdx = -1;

    bool bInteraction;

    public float fFireDelay;

    Vector3 vecMoveVec;
    Vector3 vecDodgeVec;

    Animator anim;

    Rigidbody rigidPlayer;

    GameObject objectNearObject;
    public Weapon objectEquipWeapon;

    MeshRenderer[] meshs;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rigidPlayer = GetComponent<Rigidbody>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        bIsJump = false;
        bIsDodge = false;
        bIsSwap = false;
        bIsFireReady = true;
        bIsReload = false;
        bIsDamage = false;
        bIsShopping = false;
        bIsDead = false;
        //PlayerPrefs.SetInt("MaxScore", 80000);
    }

    void Update()
    {
        GetInput();
        MovePlayer();
        TrunPlayer();
        JumpPlayer();
        DodgePlayer();
        Interaction();
        SwapWeapon();
        AttackPlayer();
        ReloadPlayer();
        ThrowGrenade();
    }

    void GetInput()
    {
        fHorizonAxis = Input.GetAxisRaw("Horizontal");
        fVerticalAxis = Input.GetAxisRaw("Vertical");
        bWalkDown = Input.GetButton("Walk");
        bJumpDown = Input.GetButtonDown("Jump");
        bInteraction = Input.GetButtonDown("Interaction");
        bSwapFirst = Input.GetButtonDown("Swap1");
        bSwapSecond = Input.GetButtonDown("Swap2");
        bSwapThird = Input.GetButtonDown("Swap3");
        bFireDown = Input.GetButton("Fire1");
        bReloadDown = Input.GetButtonDown("Reload");
        bGrenadeDown = Input.GetButtonDown("Fire2");
    }
    void TrunPlayer()
    {
        if (bIsDead)
            return;
        //키보드 회전
        transform.LookAt(transform.position + vecMoveVec);

        //마우스 커서 방향 회전
        if (bFireDown)
        {
            Ray ray = camFollowCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 vecNextVector = rayHit.point - transform.position;
                vecNextVector.y = 0;
                transform.LookAt(transform.position + vecNextVector);
            }
        }
    }
    void MovePlayer()
    {
        if (bIsSwap || !bIsFireReady || bIsReload || bIsDead)
            return;

        vecMoveVec = new Vector3(fHorizonAxis, 0, fVerticalAxis).normalized;

        if (bIsDodge)
            vecMoveVec = vecDodgeVec;

        if (!bIsBorder)
            transform.position += vecMoveVec * fSpeed * (bWalkDown ? 0.3f : 1f) * Time.deltaTime;

        anim.SetBool("isRun", vecMoveVec != Vector3.zero);
        anim.SetBool("isWalk", bWalkDown);
    }

    void AttackPlayer()
    {
        if (objectEquipWeapon == null)
            return;

        fFireDelay += Time.deltaTime;
        bIsFireReady = objectEquipWeapon.fRate < fFireDelay;

        if(bFireDown && bIsFireReady && !bIsDodge && !bIsSwap && !bIsShopping && !bIsDead && !bIsJump)
        {
            objectEquipWeapon.Use();
            anim.SetTrigger(objectEquipWeapon.eType == Weapon.Type.Melee ? "doSwing" : "doShot");
            fFireDelay = 0.0f;
        }
    }
    void ReloadPlayer()
    {
        if (objectEquipWeapon == null || objectEquipWeapon.eType == Weapon.Type.Melee || bIsReload)
            return;

        //탄 개수 확인
        if (nAmmo == 0)
            return;

        if(bReloadDown && !bIsJump && !bIsDodge && !bIsSwap && bIsFireReady && !bIsDead)
        {
            anim.SetTrigger("doReload");
            bIsReload = true;

            Invoke("ReloadEnd", 3f);
        }
    }
    void ThrowGrenade()
    {
        if (nHasGrenades == 0)
            return;
        if(bGrenadeDown && !bIsReload && !bIsDodge && !bIsSwap && !bIsDead)
        {
            Ray ray = camFollowCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if(Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 vecNextVec = rayHit.point - transform.position;
                vecNextVec.y = 10;

                GameObject instantGrenade = Instantiate(objectGrenade, transform.position, transform.rotation);
                Rigidbody rigidGrenade = instantGrenade.GetComponent<Rigidbody>();
                rigidGrenade.AddForce(vecNextVec, ForceMode.Impulse);
                rigidGrenade.AddTorque(Vector3.back * 10, ForceMode.Impulse);

                nHasGrenades--;
                objectGrenades[nHasGrenades].SetActive(false);
            }
        }
    }

    void ReloadEnd()
    {
        int nReloadAmmo = nAmmo < objectEquipWeapon.nMaxAmmo ? nAmmo : objectEquipWeapon.nMaxAmmo;
        if (nReloadAmmo <= 0)
        {
            bIsReload = false;
            return;
        }
        objectEquipWeapon.nCurAmmo = nReloadAmmo;
        nAmmo -= nReloadAmmo;
        bIsReload = false;
    }
    void JumpPlayer()
    {
        if(bJumpDown && !bIsSwap && !bIsJump && vecMoveVec == Vector3.zero && !bIsDodge && !bIsDead && !bIsReload)
        {
            rigidPlayer.AddForce(Vector3.up * fJumpPower, ForceMode.Impulse);
            bIsJump = true;
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
        }
    }
    void DodgePlayer()
    {
        if (bJumpDown && bIsFireReady && !bIsSwap && !bIsJump && !bIsDodge && vecMoveVec != Vector3.zero && !bIsDead)
        {
            vecDodgeVec = vecMoveVec;
            fSpeed *= 2f;
            bIsDodge = true;            
            anim.SetTrigger("doDodge");
            Invoke("DodgeEnd", 0.5f);
        }
    }

    void DodgeEnd()
    {
        bIsDodge = false;
        fSpeed *= 0.5f;
    }

    void SwapWeapon()
    {
        if ((bSwapFirst && (!bHasWeapon[0] || nEquipWeaponIdx == 0)) ||
            (bSwapSecond && (!bHasWeapon[1] || nEquipWeaponIdx == 1)) ||
            (bSwapThird && (!bHasWeapon[2] || nEquipWeaponIdx == 2)) )
            return;

        int nWeaponIdx = -1;

        if (bSwapFirst) nWeaponIdx = 0;
        if (bSwapSecond) nWeaponIdx = 1;
        if (bSwapThird) nWeaponIdx = 2;

        if (nWeaponIdx == -1)
            return;

        if ((bSwapFirst || bSwapSecond || bSwapThird) && !bIsJump && !bIsDodge && !bIsDead)
        {
            if(objectEquipWeapon != null)
                objectEquipWeapon.gameObject.SetActive(false);

            nEquipWeaponIdx = nWeaponIdx;
            objectEquipWeapon = objectWeapon[nWeaponIdx].GetComponent<Weapon>();
            objectWeapon[nWeaponIdx].gameObject.SetActive(true);

            anim.SetTrigger("doSwap");
            bIsSwap = true;
            Invoke("SwapEnd", 0.6f);
        }
    }
    void SwapEnd()
    {
        bIsSwap = false;
    }

    void Interaction()
    {
        if (bInteraction && objectNearObject != null && !bIsDodge && !bIsJump && !bIsDead)
        {
            if(objectNearObject.tag == "Weapon")
            {
                Item item = objectNearObject.GetComponent<Item>();
                int nWeaponIdx = item.nValue;
                bHasWeapon[nWeaponIdx] = true;
                Destroy(objectNearObject);
            }
            else if(objectNearObject.tag == "Shop")
            {
                Shop shop = objectNearObject.GetComponent<Shop>();
                shop.Enter(this);
                bIsShopping = true;
            }
        }
    }

    void FreezeRotation()
    {
        rigidPlayer.angularVelocity = Vector3.zero;

    }
    void StopToWall()
    {
        Debug.DrawRay(transform.position, transform.forward * 5, Color.cyan);
        bIsBorder = Physics.Raycast(transform.position, transform.forward, 5, LayerMask.GetMask("Wall"));
    }
    private void FixedUpdate()
    {
        FreezeRotation();
        StopToWall();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            anim.SetBool("isJump", false);
            bIsJump = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Item")
        {
            Item item = other.GetComponent<Item>();
            switch (item.eType)
            {
                case Item.Type.Ammo:
                    nAmmo += item.nValue;
                    if (nAmmo > nMaxAmmo)
                        nAmmo = nMaxAmmo;
                    break;
                case Item.Type.Coin:
                    nCoin += item.nValue;
                    if (nCoin > nMaxCoin)
                        nCoin = nMaxCoin;
                    break;
                case Item.Type.Grenade:
                    objectGrenades[nHasGrenades].SetActive(true);
                    nHasGrenades += item.nValue;
                    if (nHasGrenades > nMaxHasGrenades)
                        nHasGrenades = nMaxHasGrenades;
                    break;
                case Item.Type.Heart:
                    nHealth += item.nValue;
                    if (nHealth > nMaxHealth)
                        nHealth = nMaxHealth;
                    break;
            }
            Destroy(other.gameObject);

        }
        else if(other.tag == "EnemyBullet")
        {
            if (!bIsDamage)
            {
                Bullet enemyBullet = other.GetComponent<Bullet>();
                nHealth -= enemyBullet.nDamage;

                bool bIsBossAttack = other.name == "Boss Melee Area";
                StartCoroutine(OnDamage(bIsBossAttack));
            }
            //적이 쏜 총알일 경우. rigid가 있으므로 그것으로 확인.
            if (other.GetComponent<Rigidbody>() != null)
            {
                Destroy(other.gameObject);
            }
        }
    }

    IEnumerator OnDamage(bool bIsBossAttack)
    {
        bIsDamage = true;
        foreach(MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.yellow;
        }

        if(bIsBossAttack)
        {
            rigidPlayer.AddForce(transform.forward * -25f, ForceMode.Impulse);
        }

        if (nHealth <= 0 && !bIsDead)
        {
            OnDie();
        }
        yield return new WaitForSeconds(1f);

        bIsDamage = false;
        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.white;
        }

        if (bIsBossAttack)
        {
            rigidPlayer.velocity = Vector3.zero;
        }

    }

    void OnDie()
    {
        anim.SetTrigger("doDie");
        bIsDead = true;
        manager.GameOver();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon" || other.tag == "Shop")
            objectNearObject = other.gameObject;

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
            objectNearObject = null;
        else if(other.tag == "Shop")
        {
            Shop shop = objectNearObject.GetComponent<Shop>();
            shop.Exit();
            objectNearObject = null;
            bIsShopping = false;
        }    
    }
}
