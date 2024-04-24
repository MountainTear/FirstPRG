using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 小地图摄像机
/// </summary>
public class MapCameraController : MonoBehaviour
{
    //跟随的物体
    private Transform followObject;
    //跟随位置差
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
