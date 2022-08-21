using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Timers;
using Random = UnityEngine.Random;
using TMPro;
using UnityEngine.Networking;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;
using System.Linq;
using UnityEngine.SceneManagement;
using SimpleJSON;
public class PokerControll : MonoBehaviour
{
    private BetControll betControll;
    private DesignManager designManager;
    private GameManager gameManager;
    public int loop = 0;
    public int raiseloop = 0;
    public bool clickAble = true;
    public Transform Poker;
    public Transform prefab;
    public Transform prefab1;
    private Transform casePoker;
    private Transform PokerPieces;
    public TMP_Text everyBetAmountText;
    public int everyBetAmount = 5;
    public int BetValue = 0;
    public TMP_Text BetValueText;
    public int RaiseValue = 0;
    public TMP_Text RaiseValueText;
    public float[] moveX = new float[14] { 1676.204f, 1676.368f, 1676.457f, 1676.543f, 1676.631f, 1676.719f, 1676.807f, 1676.893f, 1676.98f, 1677.065f, 1677.153f, 1677.24f, 1677.385f, 1677.58f };
    public float moveY = -305.977f;
    public float moveZ = -882.782f;
    public Color betColor = Color.black;
    // Start is called before the first frame update
    void Start()
    {
        betControll = FindObjectOfType<BetControll>();
        designManager = FindObjectOfType<DesignManager>();
        gameManager = FindObjectOfType<GameManager>();
    }
    // Update is called once per frame
    void Update()
    {

    }
    public void OnMouseDown()
    {
        switch (loop)
        {
            case 0:
                Poker.GetComponent<MeshRenderer>().materials[0].color = Color.white;
                Poker.GetComponent<MeshRenderer>().materials[1].color = Color.cyan;
                betColor = Color.cyan;
                everyBetAmount = 10;
                loop = loop + 1;
                break;
            case 1:
                Poker.GetComponent<MeshRenderer>().materials[0].color = Color.white;
                Poker.GetComponent<MeshRenderer>().materials[1].color = Color.magenta;
                betColor = Color.magenta;
                everyBetAmount = 15;
                loop = loop + 1;
                break;
            case 2:
                Poker.GetComponent<MeshRenderer>().materials[0].color = Color.white;
                Poker.GetComponent<MeshRenderer>().materials[1].color = Color.yellow;
                betColor = Color.yellow;
                everyBetAmount = 20;
                loop = loop + 1;
                break;
            case 3:
                Poker.GetComponent<MeshRenderer>().materials[0].color = Color.white;
                Poker.GetComponent<MeshRenderer>().materials[1].color = Color.blue;
                betColor = Color.blue;
                everyBetAmount = 25;
                loop = loop + 1;
                break;
            case 4:
                Poker.GetComponent<MeshRenderer>().materials[0].color = Color.white;
                Poker.GetComponent<MeshRenderer>().materials[1].color = Color.green;
                betColor = Color.green;
                everyBetAmount = 50;
                loop = loop + 1;
                break;
            case 5:
                Poker.GetComponent<MeshRenderer>().materials[0].color = Color.white;
                Poker.GetComponent<MeshRenderer>().materials[1].color = Color.red;
                betColor = Color.red;
                everyBetAmount = 100;
                loop = loop + 1;
                break;
            case 6:
                Poker.GetComponent<MeshRenderer>().materials[0].color = Color.white;
                Poker.GetComponent<MeshRenderer>().materials[1].color = Color.black;
                betColor = Color.black;
                everyBetAmount = 5;
                loop = 0;
                break;
        }
        everyBetAmountText.text = everyBetAmount.ToString();
    }
    public IEnumerator pokerOder(float x, float y, float z, string name, int n)
    {
        BetValue = BetValue + everyBetAmount;
        BetValueText.text = BetValue.ToString();
        PokerPieces = Instantiate(prefab, new Vector3(1676.992f, -305.9f, -883.3063f), Quaternion.identity);
        PokerPieces.name = name + n;
        PokerPieces.GetComponent<MeshRenderer>().materials[0].color = Color.white;
        PokerPieces.GetComponent<MeshRenderer>().materials[1].color = Poker.GetComponent<MeshRenderer>().materials[1].color;
        PokerPieces.transform.GetChild(0).GetComponent<TMP_Text>().text = everyBetAmount.ToString();
        PokerPieces.transform.localScale = new Vector3(5f, 5f, 1f);
        const float seconds = 0.3f;
        float time = 0;
        float yy = y + (0.0083f * (n - 1));
        while (time < seconds)
        {
            PokerPieces.transform.position = Vector3.Lerp(new Vector3(1676.992f, -305.9f, -883.3063f),
                new Vector3(x, yy, z), time / seconds);
            PokerPieces.transform.localScale = new Vector3(5f, 5f, 1f);
            PokerPieces.transform.rotation = Quaternion.Lerp(Quaternion.Euler(new Vector3(-128.999f, 0, 0)), Quaternion.Euler(new Vector3(-90, 0, 0)), time / seconds);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        PokerPieces.transform.position = new Vector3(x, yy, z);
        yield return new WaitForSeconds(0.0001f);
        clickAble = true;
    }
    public IEnumerator casepokerOder(int n,bool flag)
    {
        if (flag)
        {
            const float seconds = 0.3f;
            float time = 0;
            while (time < seconds)
            {
                GameObject.Find("casePoker").transform.position = Vector3.Lerp(new Vector3(1677.385f, moveY, moveZ),
                    new Vector3(moveX[n], moveY, moveZ), time / seconds);
                GameObject.Find("casePoker").transform.localScale = new Vector3(3f, 3f, 1f);
                GameObject.Find("casePoker").transform.rotation = Quaternion.Lerp(Quaternion.Euler(new Vector3(-90, 0, 0)), Quaternion.Euler(new Vector3(-90, 0, 0)), time / seconds);
                time += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            casePoker.transform.position = new Vector3(moveX[n], moveY, moveZ);
            yield return new WaitForSeconds(0.0001f);
        }
        else { 
            casePoker = Instantiate(prefab1, new Vector3(1676.992f, -305.9f, -883.3063f), Quaternion.identity);
            casePoker.name = "casePoker";
            casePoker.transform.localScale = new Vector3(3f, 3f, 1f);
            const float seconds = 0.3f;
            float time = 0;
            while (time < seconds)
            {
                casePoker.transform.position = Vector3.Lerp(new Vector3(moveX[0], moveY, moveZ),
                    new Vector3(moveX[n], moveY, moveZ), time / seconds);
                casePoker.transform.localScale = new Vector3(3f, 3f, 1f);
                casePoker.transform.rotation = Quaternion.Lerp(Quaternion.Euler(new Vector3(-128.999f, 0, 0)), Quaternion.Euler(new Vector3(-90, 0, 0)), time / seconds);
                time += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            casePoker.transform.position = new Vector3(moveX[n], moveY, moveZ);
            yield return new WaitForSeconds(0.0001f);
        }
    }
    public IEnumerator raiseAction()
    {
        for (int i = 0; i < betControll.loop; i++)
        {
            float yz = -305.9635f + (0.0083f * (betControll.loop - 1));
            PokerPieces = Instantiate(prefab, new Vector3(1676.581f, yz, -883.091f), Quaternion.identity);
            PokerPieces.name = "raisePoker" + (i + 1);
            string name = "betPoker" + (i + 1);
            PokerPieces.GetComponent<MeshRenderer>().materials[0].color = Color.white;
            PokerPieces.GetComponent<MeshRenderer>().materials[1].color = GameObject.Find(name).GetComponent<MeshRenderer>().materials[1].color;
            PokerPieces.transform.GetChild(0).GetComponent<TMP_Text>().text = GameObject.Find(name).transform.GetChild(0).GetComponent<TMP_Text>().text;
            PokerPieces.transform.localScale = new Vector3(5f, 5f, 1f);
            const float seconds = 0.3f;
            float time = 0;
            float yy = -305.9635f + (0.0083f * (i - 1));
            while (time < seconds)
            {
                PokerPieces.transform.position = Vector3.Lerp(new Vector3(1676.581f, -305.9635f, -883.091f),
                    new Vector3(1677.41f, yy, -883.091f), time / seconds);

                PokerPieces.transform.localScale = new Vector3(5f, 5f, 1f);
                PokerPieces.transform.rotation = Quaternion.Lerp(Quaternion.Euler(new Vector3(-128.999f, 0, 0)), Quaternion.Euler(new Vector3(-90, 0, 0)), time / seconds);
                time += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            PokerPieces.transform.position = new Vector3(1677.41f, yy, -883.091f);
            yield return new WaitForSeconds(0.0001f);
        }
        raiseloop = betControll.loop;
        RaiseValue = BetValue;
        RaiseValueText.text = RaiseValue.ToString();
        yield return new WaitForSeconds(0.0001f);
        StartCoroutine(designManager.CardRotate());
        StartCoroutine(gameManager.Server());
    }
}