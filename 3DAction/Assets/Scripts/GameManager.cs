using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject camMenuCamera;
    public GameObject camGameCamera;

    public Player player;
    public Boss boss;     

    public int nStage;
    public float fPlayTime;
    public bool bIsBattle;
    public int nEnemyCntA;
    public int nEnemyCntB;
    public int nEnemyCntC;
    public int nEnemyCntD;

    public Transform[] enemyZones;
    public GameObject[] objectEnemies;
    public List<int> listEnemyList;

    public GameObject objectMenuPanel;
    public GameObject objectGamePanel;
    public GameObject objectOverPanel;

    public GameObject objectItemShop;
    public GameObject objectWeaponShop;
    public GameObject objectStartZone;

    public Text textMaxScore;
    public Text textScore;
    public Text textStage;
    public Text textPlayTime;
    public Text textPlayerHealth;
    public Text textPlayerAmmo;
    public Text textPlayerCoin;
    public Image imageWeapon1Img;
    public Image imageWeapon2Img;
    public Image imageWeapon3Img;
    public Image imageWeaponRImg;
    public Text textEnemyA;
    public Text textEnemyB;
    public Text textEnemyC;
    public RectTransform bossHealthGroup;
    public RectTransform bossHealthBar;
    public Text textCurScore;
    public Text textBest;

    private void Awake()
    {
        textMaxScore.text = string.Format("{0:n0}" ,PlayerPrefs.GetInt("MaxScore"));

        if (PlayerPrefs.HasKey("MaxScore"))
            PlayerPrefs.SetInt("MaxScore", 0);
    }

    public void GameStart()
    {
        camMenuCamera.SetActive(false);
        camGameCamera.SetActive(true);

        objectMenuPanel.SetActive(false);
        objectGamePanel.SetActive(true);

        player.gameObject.SetActive(true);
    }

    public void GameOver()
    {
        objectGamePanel.SetActive(false);
        objectOverPanel.SetActive(true);
        textCurScore.text = textScore.text;

        int nMaxScore = PlayerPrefs.GetInt("MaxScore");
        if(player.nScore > nMaxScore)
        {
            textBest.gameObject.SetActive(true);
            PlayerPrefs.SetInt("MaxScore", player.nScore);
        }
    }

    public void ReStart()
    {
        SceneManager.LoadScene(0);
    }

    public void StageStart()
    {
        objectItemShop.SetActive(false);
        objectWeaponShop.SetActive(false);
        objectStartZone.SetActive(false);

        foreach (Transform zone in enemyZones)
            zone.gameObject.SetActive(true);

        nStage++;
        bIsBattle = true;
        StartCoroutine(InBattle());
    }

    public void StageEnd()
    {
        player.transform.position = Vector3.up * 0.8f;

        objectItemShop.SetActive(true);
        objectWeaponShop.SetActive(true);
        objectStartZone.SetActive(true);

        foreach (Transform zone in enemyZones)
            zone.gameObject.SetActive(false);


        bIsBattle = false;

    }

    IEnumerator InBattle()
    {
        if (nStage % 5 == 0)
        {
            nEnemyCntD++;
            GameObject instantEnemy = Instantiate(objectEnemies[3], enemyZones[3].position, enemyZones[3].rotation);
            Enemy enemy = instantEnemy.GetComponent<Enemy>();
            enemy.target = player.transform;
            enemy.manager = this;
            boss = instantEnemy.GetComponent<Boss>();
        }
        else
        {
            for (int index = 0; index < nStage; index++)
            {
                int ran = Random.Range(0, 3);
                listEnemyList.Add(ran);

                switch (ran)
                {
                    case 0:
                        nEnemyCntA++;
                        break;
                    case 1:
                        nEnemyCntB++;
                        break;
                    case 2:
                        nEnemyCntC++;
                        break;
                }
            }
            while (listEnemyList.Count > 0)
            {
                int ranZone = Random.Range(0, 4);
                GameObject instantEnemy = Instantiate(objectEnemies[listEnemyList[0]], enemyZones[ranZone].position, enemyZones[ranZone].rotation);
                Enemy enemy = instantEnemy.GetComponent<Enemy>();
                enemy.target = player.transform;
                enemy.manager = this;
                listEnemyList.RemoveAt(0);
                yield return new WaitForSeconds(4f);
            }
        }         
        while(nEnemyCntA + nEnemyCntB + nEnemyCntC + nEnemyCntD > 0)
        {
            yield return null;
        }

        yield return new WaitForSeconds(4f);

        boss = null;
        StageEnd();
    }

    private void Update()
    {
        if (bIsBattle)
            fPlayTime += Time.deltaTime;
    }

    private void LateUpdate()
    {
        //상단 UI
        textScore.text = string.Format("{0:n0}", player.nScore);
        textStage.text = "STAGE " + nStage;

        int nHour = (int)(fPlayTime / 3600);
        int nMin = (int)((fPlayTime - nHour *3600) / 60);
        int nSecond = (int)fPlayTime % 60;
        textPlayTime.text = string.Format("{0:00}",nHour) + ":" + 
                            string.Format("{0:00}", nMin) + ":" +
                            string.Format("{0:00}", nSecond);

        textPlayerHealth.text = player.nHealth + " / " + player.nMaxHealth;
        textPlayerCoin.text = string.Format("{0:n0}", player.nCoin);

        //플레이어 UI
        if (player.objectEquipWeapon == null)
            textPlayerAmmo.text = "- / " + player.nAmmo;
        else if (player.objectEquipWeapon.eType == Weapon.Type.Melee)
            textPlayerAmmo.text = "- / " + player.nAmmo;
        else
            textPlayerAmmo.text = player.objectEquipWeapon.nCurAmmo + " / " + player.nAmmo;

        //무기 UI
        imageWeapon1Img.color = new Color(1, 1, 1, player.bHasWeapon[0] ? 1 : 0);
        imageWeapon2Img.color = new Color(1, 1, 1, player.bHasWeapon[1] ? 1 : 0);
        imageWeapon3Img.color = new Color(1, 1, 1, player.bHasWeapon[2] ? 1 : 0);
        imageWeaponRImg.color = new Color(1, 1, 1, player.nHasGrenades > 0 ? 1 : 0);


        //몬스터 UI
        textEnemyA.text = "x " + nEnemyCntA.ToString();
        textEnemyB.text = "x " + nEnemyCntB.ToString();
        textEnemyC.text = "x " + nEnemyCntC.ToString();

        if (boss != null)
        {
            bossHealthGroup.anchoredPosition = Vector3.down * 30;
            bossHealthBar.localScale = new Vector3((float)boss.nCurHealth / boss.nMaxHealth, 1, 1);
        }
        else
        {
            bossHealthGroup.anchoredPosition = Vector3.up * 200;
        }
            
    }
}
