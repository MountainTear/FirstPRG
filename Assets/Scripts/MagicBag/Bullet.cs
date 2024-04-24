using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ͷ����
/// </summary>
public class Bullet : MonoBehaviour
{
    //��������
    public float launchForce = 3f;
    //����ʱ���ϵ���
    public float launchUp = 3f;
    //���к��Զ�����ʱ��
    public float flyTime = 2f;
    //���Ƿ���
    private Vector3 loolDirection;
    //�˺�
    public int damage = 2;
    private Rigidbody uRigidbody;

    private void Awake()
    {
        uRigidbody = GetComponent<Rigidbody>();
        //��ȡ��ǰ��ɫ�ĳ���
        loolDirection = GameObject.Find("Lin").transform.forward;
        //�����ʼ�ٶ�
        uRigidbody.velocity =  launchForce*loolDirection + Vector3.up * launchUp;
    }

    private void Update()
    {
        if (flyTime > 0)
        {
            flyTime = flyTime - Time.deltaTime;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        //�����ײ��Enemy
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<Enemy>().TakeDamage(damage);
        }
        Destroy(this.gameObject);
    }

}
