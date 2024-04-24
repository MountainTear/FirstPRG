using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 弹窗管理器，负责弹窗的生成
/// </summary>
public class PopViewManager : MonoBehaviour
{
    //介绍弹窗预制体
    private GameObject introPopView;

    private void Awake()
    {
        //获取介绍弹窗预制体
        introPopView = (GameObject)Resources.Load("Prefabs/UI/IntroducePopView");
    }

    //生成介绍弹窗
    public void IntroducePopView(string content)
    {
        //获取介绍弹窗脚本
        GameObject introPW =  Instantiate(introPopView);
        IntroducePopView introScript = introPW.GetComponent<IntroducePopView>();
        introScript.SetContent(content);
    }
}