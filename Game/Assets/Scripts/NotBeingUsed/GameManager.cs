using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

public class GameManager : MonoBehaviour
{
    [Header("Start Game Info:")]
    public bool gameStarted;
    public bool roundOver;
    public int startTimer;
    public int playersAlive;
    public Text startTimerText;

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(StartTimer());
    }

    // Update is called once per frame
    void Update()
    {
    }

    //private IEnumerator StartTimer()
    //{
    //    int time = startTimer;

    //    while(time > 0)
    //    {
    //        startTimerText.text = time.ToString();
    //        yield return new WaitForSeconds(1);
    //        time--;

    //        if(time == 0)
    //        {
    //            startTimerText.text = "Start!";
    //            yield return new WaitForSeconds(1);
    //            startTimerText.text = "";
    //        }
    //        //scoreBoard.SetActive(true);
    //        gameStarted = true;
    //        roundOver = false;
    //    }
    //}

    
}
