using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NpcManager : MonoBehaviour
{
    public float displayTime = 5.0f;    //չʾʱ��
    private Transform dialogText;    //�Ի���
    public string[] talkList;
    public string defaultTalk = null;
    private float timerDisplay;
    private int talkTimes = 0;      //�Ի�����

    private void Awake()
    {
        //��ȡ�ı��Ի���
        dialogText = transform.Find("Canvas/Dialogue");
        timerDisplay = -1.0f;

        dialogText.GetComponent<Text>().text = defaultTalk == null ? "δ����" : defaultTalk;    
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
