using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 投掷物
/// </summary>
public class Bullet : MonoBehaviour
{
    //发射力度
    public float launchForce = 3f;
    //发射时向上的力
    public float launchUp = 3f;
    //飞行后自动销毁时间
    public float flyTime = 2f;
    //主角方向
    private Vector3 loolDirection;
    //伤害
    public int damage = 2;
    private Rigidbody uRigidbody;

    private void Awake()
    {
        uRigidbody = GetComponent<Rigidbody>();
        //获取当前角色的朝向
        loolDirection = GameObject.Find("Lin").transform.forward;
        //赋予初始速度
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
        //如果碰撞到Enemy
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<Enemy>().TakeDamage(damage);
        }
        Destroy(this.gameObject);
    }

}
