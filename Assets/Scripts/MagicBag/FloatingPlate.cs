using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 悬浮板
/// </summary>
public class FloatingPlate : MonoBehaviour
{
    //组件及移动位置
    private Rigidbody rigibdody;
    private Transform upPositon;
    private Transform downPosition;

    //移动速度及相关参数
    [SerializeField]private float moveSpeed = 1f;
    private float deltaX, deltaY, deltaZ;
    private Vector3 oldPosition;

    //自动销毁时间
    public float existTime = 20f;

    private void Awake()
    {
        rigibdody = transform.Find("Cookie").gameObject.GetComponent<Rigidbody>();
        upPositon = transform.Find("UpPosition").gameObject.GetComponent<Transform>();
        downPosition = transform.Find("DownPosition").gameObject.GetComponent<Transform>();
        //计算偏移距离
        deltaX = upPositon.position.x - downPosition.position.x;
        deltaY = upPositon.position.y - downPosition.position.y;
        deltaZ = upPositon.position.z - downPosition.position.z;
        oldPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }

    private void FixedUpdate()
    {
        //过期销毁
        existTime = existTime - Time.deltaTime;
        if (existTime < 0)
        {
            Destroy(this.gameObject);
        }
        //上下移动
        float temp = Mathf.Sin(Time.time * moveSpeed);
        rigibdody.position = oldPosition+  new Vector3(temp * deltaX,  temp * deltaY, temp * deltaZ);
    }
}
