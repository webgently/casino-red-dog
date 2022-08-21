using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Timers;
using Random = UnityEngine.Random;
using TMPro;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using System.Linq;
using SimpleJSON;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Start is called before the first frame update
    public static APIForm apiform;
    public static Globalinitial _global;
    private DesignManager designManager;
    private PokerControll pokerControll;
    private BetControll betControll;
    public TMP_Text totalPriceText;
    private float totalValue;
    private int loop = 0;
    private string FONflag = "NEW";
    public Button betbtn;
    public Button foldbtn;
    public bool clickflag = true;

    [DllImport("__Internal")]
    private static extern void GameReady(string msg);
    BetPlayer _player;
    public void RequestToken(string data)
    {
        JSONNode usersInfo = JSON.Parse(data);
        _player.token = usersInfo["token"];
        _player.username = usersInfo["userName"];
        float i_balance = float.Parse(usersInfo["amount"]);
        totalValue = i_balance;
        totalPriceText.text = totalValue.ToString("F2");
    }
    void Start()
    {
        _player = new BetPlayer();
        #if UNITY_WEBGL == true && UNITY_EDITOR == false
            GameReady("Ready");
        #endif
        StartCoroutine(firstServer());
        designManager = FindObjectOfType<DesignManager>();
        pokerControll = FindObjectOfType<PokerControll>();
        betControll = FindObjectOfType<BetControll>();
        betbtn.interactable = false;
    }
    // Update is called once per frame
    void Update()
    {

    }
    public void BetOrRebet()
    {
        if (pokerControll.BetValue == 0)
        {
            StartCoroutine(alert("Set balance!", "other"));
        }
        else
        {
            if (totalValue >= pokerControll.BetValue)
            {
                if (totalValue >= 5)
                {
                    switch (loop)
                    {
                        case 0:
                            betbtn.interactable = false;
                            foldbtn.interactable = false;
                            StartCoroutine(UpdateCoinsAmount(totalValue, totalValue - pokerControll.BetValue));
                            StartCoroutine(designManager.CardThrow(0, 3));
                            clickflag = false;
                            foldbtn.transform.GetChild(0).GetComponent<TMP_Text>().text = "CALL";
                            FONflag = "CALL";
                            betbtn.transform.GetChild(0).GetComponent<TMP_Text>().text = "RAISE";
                            loop = loop + 1;
                            StartCoroutine(beginServer());
                            break;
                        case 1:
                            betbtn.interactable = false;
                            foldbtn.interactable = false;
                            StartCoroutine(UpdateCoinsAmount(totalValue, totalValue - pokerControll.BetValue));
                            foldbtn.transform.GetChild(0).GetComponent<TMP_Text>().text = "NEW";
                            FONflag = "NEW";
                            betbtn.transform.GetChild(0).GetComponent<TMP_Text>().text = "REDEAL";
                            loop = loop + 1;
                            StartCoroutine(pokerControll.raiseAction());
                            break;
                        case 2:
                            betbtn.interactable = false;
                            foldbtn.interactable = false;
                            StartCoroutine(raiseformat());
                            StartCoroutine(designManager.ThrowedCardClear(true));
                            StartCoroutine(UpdateCoinsAmount(totalValue, totalValue - pokerControll.BetValue));
                            clickflag = false;
                            foldbtn.transform.GetChild(0).GetComponent<TMP_Text>().text = "CALL";
                            FONflag = "CALL";
                            betbtn.transform.GetChild(0).GetComponent<TMP_Text>().text = "RAISE";
                            loop = 1;
                            StartCoroutine(beginServer());
                            break;
                    }
                }
                else
                {
                    StartCoroutine(alert("Insufficient balance!", "other"));
                }
            }
            else
            {
                StartCoroutine(alert("Insufficient balance!", "other"));
            }
        }
    }
    public void NewOrFold()
    {
        switch (FONflag)
        {
            case "CALL":
                StartCoroutine(Server());
                StartCoroutine(designManager.CardRotate());
                foldbtn.transform.GetChild(0).GetComponent<TMP_Text>().text = "NEW";
                FONflag = "NEW";
                betbtn.transform.GetChild(0).GetComponent<TMP_Text>().text = "REDEAL";
                loop = loop + 1;
                break;
            case "NEW":
                StartCoroutine(BetClear());
                StartCoroutine(designManager.ThrowedCardClear(false));
                break;
        }
    }
    public IEnumerator firstServer()
    {
        yield return new WaitForSeconds(0.5f);
        WWWForm form = new WWWForm();
        form.AddField("userName", _player.username);
        form.AddField("token", _player.token);
        _global = new Globalinitial();
        UnityWebRequest www = UnityWebRequest.Post(_global.BaseUrl + "api/CardOder", form);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.Success)
        {
            string strdata = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
            apiform = JsonUtility.FromJson<APIForm>(strdata);
            if (apiform.serverMsg == "Success")
            {
                designManager.cardOrderArray = apiform.cardOder;
                yield return new WaitForSeconds(0.0001f);
                StartCoroutine(designManager.CardOder());
            }
            else
            {
                StartCoroutine(alert(apiform.serverMsg, "other"));
            }
        }
        else
        {
            StartCoroutine(alert("Can't find server!", "other"));
        }
    }
    IEnumerator beginServer()
    {
        yield return new WaitForSeconds(1f);
        WWWForm form = new WWWForm();
        form.AddField("userName", _player.username);
        form.AddField("token", _player.token);
        form.AddField("betAmount", pokerControll.BetValue.ToString());
        form.AddField("amount", totalValue.ToString("F2"));
        _global = new Globalinitial();
        UnityWebRequest www = UnityWebRequest.Post(_global.BaseUrl + "api/bet-redDog", form);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.Success)
        {
            string strdata = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
            apiform = JsonUtility.FromJson<APIForm>(strdata);
            if (apiform.serverMsg == "Success")
            {
                if (apiform.move == 13)
                {
                    StartCoroutine(pokerControll.casepokerOder(apiform.move - 1, false));
                    yield return new WaitForSeconds(1f);
                }
                else if (apiform.move == 12)
                {
                    StartCoroutine(pokerControll.casepokerOder(apiform.move, false));
                    StartCoroutine(UpdateCoinsAmount(totalValue, totalValue + pokerControll.BetValue));
                    string alert_text = "PUSH " + pokerControll.BetValue.ToString();
                    StartCoroutine(alert(alert_text, "win"));
                    foldbtn.transform.GetChild(0).GetComponent<TMP_Text>().text = "NEW";
                    FONflag = "NEW";
                    betbtn.transform.GetChild(0).GetComponent<TMP_Text>().text = "REDEAL";
                    loop = loop + 1;
                    yield return new WaitForSeconds(1f);
                }
                else
                {
                    StartCoroutine(pokerControll.casepokerOder(apiform.move, false));
                }
                betbtn.interactable = true;
                foldbtn.interactable = true;
            }
            else
            {
                StartCoroutine(alert(apiform.serverMsg, "other"));
                StartCoroutine(UpdateCoinsAmount(totalValue, totalValue + pokerControll.BetValue));
            }
        }
        else
        {
            StartCoroutine(alert("Can't find server!", "other"));
            StartCoroutine(UpdateCoinsAmount(totalValue, totalValue + pokerControll.BetValue));
        }
    }
    public IEnumerator Server()
    {
        yield return new WaitForSeconds(1f);
        WWWForm form = new WWWForm();
        form.AddField("userName", _player.username);
        form.AddField("token", _player.token);
        form.AddField("raiseAmount", pokerControll.RaiseValue.ToString());
        form.AddField("amount", totalValue.ToString("F2"));
        UnityWebRequest www = UnityWebRequest.Post(_global.BaseUrl + "api/result", form);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.Success)
        {
            string strdata = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
            apiform = JsonUtility.FromJson<APIForm>(strdata);
            if (apiform.serverMsg == "Success")
            {
                if (apiform.raisePrice == 0)
                {
                    StartCoroutine(alert("Better luck next time!", "lose"));
                }
                else
                {
                    if (apiform.cases == 12)
                    {
                        StartCoroutine(pokerControll.casepokerOder(13, true));
                    }
                    StartCoroutine(alert(apiform.msg, "win"));
                    StartCoroutine(UpdateCoinsAmount(totalValue, apiform.total));
                }

            }
            else
            {
                StartCoroutine(alert(apiform.serverMsg, "other"));
                StartCoroutine(UpdateCoinsAmount(totalValue, totalValue + pokerControll.BetValue + pokerControll.RaiseValue));
            }
        }
        else
        {
            StartCoroutine(alert("Can't find server!", "other"));
            StartCoroutine(UpdateCoinsAmount(totalValue, totalValue + pokerControll.BetValue + pokerControll.RaiseValue));
        }
        yield return new WaitForSeconds(0.01f);
    }
    public IEnumerator alert(string msg, string state)
    {
        if (state == "win")
        {
            AlertController.isWin = true;
        }
        else
        {
            AlertController.isLose = true;
        }
        GameObject.Find("alert").GetComponent<TMP_Text>().text = msg;
        yield return new WaitForSeconds(2.5f);
        AlertController.isWin = false;
        AlertController.isLose = false;
        yield return new WaitForSeconds(1.5f);
        betbtn.interactable = true;
        foldbtn.interactable = true;
    }
    private IEnumerator UpdateCoinsAmount(float preValue, float changeValue)
    {
        // Animation for increasing and decreasing of coins amount
        const float seconds = 0.2f;
        float elapsedTime = 0;
        while (elapsedTime < seconds)
        {
            totalPriceText.text = Mathf.Floor(Mathf.Lerp(preValue, changeValue, (elapsedTime / seconds))).ToString();
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        totalValue = changeValue;
        totalPriceText.text = totalValue.ToString();
    }
    private IEnumerator BetClear()
    {
        clickflag = true;
        pokerControll.BetValue = 0;
        pokerControll.RaiseValue = 0;
        pokerControll.BetValue = 0;
        pokerControll.BetValueText.text = pokerControll.BetValue.ToString();
        pokerControll.RaiseValueText.text = pokerControll.RaiseValue.ToString();
        pokerControll.BetValueText.text = pokerControll.BetValue.ToString();
        betbtn.transform.GetChild(0).GetComponent<TMP_Text>().text = "DEAL";
        loop = 0;
        Destroy(GameObject.Find("casePoker"));
        for (int i = 0; i < pokerControll.raiseloop; i++)
        {
            string name = "raisePoker" + (i + 1);
            Destroy(GameObject.Find(name));
        }
        pokerControll.raiseloop = 0;
        for (int i = 0; i < betControll.loop; i++)
        {
            string name = "betPoker" + (i + 1);
            Destroy(GameObject.Find(name));
        }
        betControll.loop = 0;
        yield return new WaitForSeconds(0.1f);
    }
    private IEnumerator raiseformat()
    {
        Destroy(GameObject.Find("casePoker"));
        for (int i = 0; i < pokerControll.raiseloop; i++)
        {
            string name = "raisePoker" + (i + 1);
            Destroy(GameObject.Find(name));
        }
        pokerControll.raiseloop = 0;
        pokerControll.RaiseValue = 0;
        pokerControll.RaiseValueText.text = pokerControll.RaiseValue.ToString();
        yield return new WaitForSeconds(0.01f);
    }
}
public class BetPlayer
{
    public string username;
    public string token;
}
