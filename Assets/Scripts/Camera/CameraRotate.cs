using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    public float moveSpeed = 5.0f;

    void Update()
    {
        // �����굱ǰλ�õ�X��Y
        float mouseX = Input.GetAxis("Mouse X") * moveSpeed;
        // �����X���ϵ��ƶ�תΪ�������ҵ��ƶ���ͬʱ������������������������ƶ�
        transform.localRotation = transform.localRotation * Quaternion.Euler(0, mouseX, 0);

    }
}
