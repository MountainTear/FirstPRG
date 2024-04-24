using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���������������𵯴�������
/// </summary>
public class PopViewManager : MonoBehaviour
{
    //���ܵ���Ԥ����
    private GameObject introPopView;

    private void Awake()
    {
        //��ȡ���ܵ���Ԥ����
        introPopView = (GameObject)Resources.Load("Prefabs/UI/IntroducePopView");
    }

    //���ɽ��ܵ���
    public void IntroducePopView(string content)
    {
        //��ȡ���ܵ����ű�
        GameObject introPW =  Instantiate(introPopView);
        IntroducePopView introScript = introPW.GetComponent<IntroducePopView>();
        introScript.SetContent(content);
    }
}