using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������
/// </summary>
public class FloatingPlate : MonoBehaviour
{
    //������ƶ�λ��
    private Rigidbody rigibdody;
    private Transform upPositon;
    private Transform downPosition;

    //�ƶ��ٶȼ���ز���
    [SerializeField]private float moveSpeed = 1f;
    private float deltaX, deltaY, deltaZ;
    private Vector3 oldPosition;

    //�Զ�����ʱ��
    public float existTime = 20f;

    private void Awake()
    {
        rigibdody = transform.Find("Cookie").gameObject.GetComponent<Rigidbody>();
        upPositon = transform.Find("UpPosition").gameObject.GetComponent<Transform>();
        downPosition = transform.Find("DownPosition").gameObject.GetComponent<Transform>();
        //����ƫ�ƾ���
        deltaX = upPositon.position.x - downPosition.position.x;
        deltaY = upPositon.position.y - downPosition.position.y;
        deltaZ = upPositon.position.z - downPosition.position.z;
        oldPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }

    private void FixedUpdate()
    {
        //��������
        existTime = existTime - Time.deltaTime;
        if (existTime < 0)
        {
            Destroy(this.gameObject);
        }
        //�����ƶ�
        float temp = Mathf.Sin(Time.time * moveSpeed);
        rigibdody.position = oldPosition+  new Vector3(temp * deltaX,  temp * deltaY, temp * deltaZ);
    }
}
