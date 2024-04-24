using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// С��ͼ�����
/// </summary>
public class MapCameraController : MonoBehaviour
{
    //���������
    private Transform followObject;
    //����λ�ò�
    private Vector3 vector;

    void Start()
    {
        followObject = GameObject.Find("Lin").GetComponent<Transform>();
        vector = this.transform.position - followObject.position;
    }

    private void LateUpdate()
    {
        ToFollow();
    }


    void ToFollow()
    {
        this.transform.position = followObject.position + vector;
    }

}
