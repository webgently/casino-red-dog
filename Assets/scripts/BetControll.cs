using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetControll : MonoBehaviour
{
    private PokerControll pokerControll;
    private GameManager gameManager;
    public int loop = 0;
    // Start is called before the first frame update
    void Start()
    {
        pokerControll = FindObjectOfType<PokerControll>();
        gameManager = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnMouseDown()
    {
        if (gameManager.clickflag) { 
            if (pokerControll.clickAble)
            {
                if (pokerControll.BetValue + pokerControll.everyBetAmount <= 1000)
                {
                    loop = loop + 1;
                    pokerControll.clickAble = false;
                    StartCoroutine(pokerControll.pokerOder(1676.573f, -305.9635f, -883.091f, "betPoker", loop));
                }
                else
                {
                    //alert
                }
            }
        }
    }
}
