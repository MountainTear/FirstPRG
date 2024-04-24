using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 介绍弹窗类
/// </summary>
public class IntroducePopView : MonoBehaviour
{
    //弹窗内容
    private Button closeButton;
    private Text content;

    private void Awake()
    {
        //获取关闭按钮
        closeButton = GameObject.Find("IntroducePopView(Clone)/CloseButton").GetComponent<Button>();
        closeButton.onClick.AddListener(() => { CloseButtonOnClick(); });
        content = GameObject.Find("IntroducePopView(Clone)/Content").GetComponent<Text>();
    }

    //关闭弹窗
    private void CloseButtonOnClick()
    {
        //关闭窗口，销毁与当前脚本挂靠的游戏体
        Destroy(gameObject);
    }

    //设置内容
    public void SetContent(string s)
    {
        content.text = s;
    }
}


