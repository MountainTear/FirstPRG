using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ���ܵ�����
/// </summary>
public class IntroducePopView : MonoBehaviour
{
    //��������
    private Button closeButton;
    private Text content;

    private void Awake()
    {
        //��ȡ�رհ�ť
        closeButton = GameObject.Find("IntroducePopView(Clone)/CloseButton").GetComponent<Button>();
        closeButton.onClick.AddListener(() => { CloseButtonOnClick(); });
        content = GameObject.Find("IntroducePopView(Clone)/Content").GetComponent<Text>();
    }

    //�رյ���
    private void CloseButtonOnClick()
    {
        //�رմ��ڣ������뵱ǰ�ű��ҿ�����Ϸ��
        Destroy(gameObject);
    }

    //��������
    public void SetContent(string s)
    {
        content.text = s;
    }
}


