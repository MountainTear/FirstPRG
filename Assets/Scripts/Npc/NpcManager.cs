using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NpcManager : MonoBehaviour
{
    public float displayTime = 5.0f;    //展示时间
    private Transform dialogText;    //对话框
    public string[] talkList;
    public string defaultTalk = null;
    private float timerDisplay;
    private int talkTimes = 0;      //对话次数

    private void Awake()
    {
        //获取文本对话框
        dialogText = transform.Find("Canvas/Dialogue");
        timerDisplay = -1.0f;

        dialogText.GetComponent<Text>().text = defaultTalk == null ? "未设置" : defaultTalk;    
    }

    private void Update()
    {
        if (timerDisplay >= 0)
        {
            timerDisplay -= Time.deltaTime;
            if (timerDisplay < 0)
            {
                dialogText.GetComponent<Text>().text = defaultTalk;
            }
        }
    }

    public void DisplayDialog()
    {
        if (talkList.Length == 0)
        {
            return;
        }
        talkTimes =talkTimes % talkList.Length;
        timerDisplay = displayTime;
        dialogText.gameObject.SetActive(true);
        dialogText.GetComponent<Text>().text = talkList[talkTimes];
        talkTimes = talkTimes + 1;
    }

}
