using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public RectTransform uiGroup;
    public Animator anim;

    public GameObject[] objectItemObj;
    public int[] nItemPrice;
    public Transform[] itemPos;
    public Text textTalk;

    public string[] strTalkData;

    Player enterPlayer;

    public void Enter(Player player)
    {
        enterPlayer = player;
        uiGroup.anchoredPosition = Vector3.zero;
    }

    
    public void Exit()
    {
        anim.SetTrigger("doHello");
        uiGroup.anchoredPosition = Vector3.down * 1000;
    }

    public void Buy(int index)
    {
        int nPrice = nItemPrice[index];
        if (nPrice > enterPlayer.nCoin)
        {
            StopCoroutine(Talk());
            StartCoroutine(Talk());
            return;
        }

        enterPlayer.nCoin -= nPrice;
        Vector3 ranVec = Vector3.right * Random.Range(-3, 3) + Vector3.forward * Random.Range(-3, 3);
        Instantiate(objectItemObj[index], itemPos[index].position + ranVec, itemPos[index].rotation);

    }

    IEnumerator Talk()
    {
        textTalk.text = strTalkData[1];
        yield return new WaitForSeconds(1f);
        textTalk.text = strTalkData[0];
    }
}
