using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    public float moveSpeed = 5.0f;

    void Update()
    {
        // 获得鼠标当前位置的X和Y
        float mouseX = Input.GetAxis("Mouse X") * moveSpeed;
        // 鼠标在X轴上的移动转为主角左右的移动，同时带动其子物体摄像机的左右移动
        transform.localRotation = transform.localRotation * Quaternion.Euler(0, mouseX, 0);

    }
}
