using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

/// <summary>
/// 敌人状态
/// </summary>
public enum EnemyStates {
    /// <summary>
    /// 站桩
    /// </summary>
    GUARD,  
    /// <summary>
    /// 巡逻
    /// </summary>
    PATROL,     
    /// <summary>
    /// 追击
    /// </summary>
    CHASE,  
    /// <summary>
    /// 死亡
    /// </summary>
    DEAD    
}
/// <summary>
/// 敌人基类
/// </summary>
/// 
//自动添加NavMeshAgent组件
[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    //敌人代理
    private NavMeshAgent agent;
    //动画组件
    public Animator anim;
    //血条组件
    private Slider healthBar;

    //玩家相关
    private PlayerDataManager playerDataManager;
    protected GameObject attackTarget;

    //敌人状态
    private EnemyStates enemyStates;

    //敌人参数
    public int damage;    //伤害
    public int maxHealth = 10;
    public int nowHealth = 10; 
    public float sightRadius;   //视野
    public bool isGuard;    //是否站桩
    private float speed;    //记录敌人原有的速度
    public float lookAtTime; //在原地看的时间
    private float remainLookAtTime; //计时器

    //站桩及巡逻参数
    private Vector3 guardPos;
    private Quaternion guardRotation;

    //巡逻相关
    public float patrolRange;   //巡逻范围
    private Vector3 wayPoint;   //巡逻的随机一点

    //bool配合动画
    bool isWalk;
    bool isChase;
    bool isFollow;
    bool isDead;

    //碰撞贴合扣血
    public float stayDamageTime = 2f;
    private float stayTimer;

    void Awake()
    {
        //获取组件
        agent = GetComponent<NavMeshAgent>();
        healthBar = transform.Find("Canvas/Slider").gameObject.GetComponent<Slider>();
        speed = agent.speed;

        guardPos = transform.position;
        guardRotation = transform.rotation;
        remainLookAtTime = lookAtTime;
    }

    void Start()
    {
        //获取数据管理器，晚于玩家数据初始化
        playerDataManager = GameObject.Find("GameManager").GetComponent<PlayerDataManager>();

        //如果勾选站桩
        if (isGuard)
        {
            enemyStates = EnemyStates.GUARD;
        }
        //否则巡逻
        else
        {
            enemyStates = EnemyStates.PATROL;
            //获取可巡逻范围内的点
            GetNewWayPoint();
        }

        //血条初始化
        healthBar.minValue = 0;
        healthBar.maxValue = maxHealth;
    }

    void Update()
    {
        SwitchStates();
        SwitchAnimation();
    }

    //碰撞伤害
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<CharacterController>())
        {
            playerDataManager.Injured(damage);
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.GetComponent<CharacterController>())
        {
            stayTimer = stayTimer + Time.deltaTime;
            if (stayTimer > stayDamageTime)
            {
                playerDataManager.Injured(damage);
                stayTimer = 0;
            }
        }
    }
    //找到玩家
    bool FoundPlayer()
    {
        //获取球体周围的所有碰撞体
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);

        foreach (var target in colliders)
        {
            if (target.CompareTag("Player"))
            {
                attackTarget = target.gameObject;
                return true;
            }
        }
        attackTarget = null;
        return false;
    }

    //切换状态
    void SwitchStates()
    {
        if (isDead)
        {
            enemyStates = EnemyStates.DEAD;
        }
        //如果发现player 切换到CHASE
        else if (FoundPlayer())
        {
            agent.stoppingDistance = agent.radius;
            enemyStates = EnemyStates.CHASE;
        }
        else
        {
            agent.stoppingDistance = agent.radius;
        }

        //敌人不同状态做不同事
        switch (enemyStates)
        {
            case EnemyStates.GUARD:
                {
                    isChase = false;

                    if (transform.position != guardPos)
                    {
                        isWalk = true;
                        agent.isStopped = false;
                        agent.destination = guardPos;

                        if (Vector3.SqrMagnitude(guardPos - transform.position) <= agent.stoppingDistance)
                        {
                            isWalk = false;
                            transform.rotation = Quaternion.Lerp(transform.rotation, guardRotation, 0.01f);
                        }
                    }
                }
                break;
            case EnemyStates.PATROL:
                {
                    isChase = false;
                    agent.speed = speed * 0.5f;

                    //判断是否到了随机巡逻点
                    if (Vector3.Distance(wayPoint, transform.position) <= agent.stoppingDistance)
                    {
                        isWalk = false;
                        if (remainLookAtTime > 0)
                            remainLookAtTime -= Time.deltaTime;
                        else
                            GetNewWayPoint();
                    }
                    else
                    {
                        isWalk = true;
                        agent.destination = wayPoint;
                    }
                }
                break;
            case EnemyStates.CHASE:
                {
                    isWalk = false;
                    isChase = true;
                    //追击模式全速
                    agent.speed = speed;
                    //追击过程中未找到玩家
                    if (!FoundPlayer())
                    {
                        isFollow = false;
                        if (remainLookAtTime > 0)
                        {
                            agent.destination = transform.position;
                            remainLookAtTime -= Time.deltaTime;
                        }
                        //返回上一个状态
                        else if (isGuard)
                            enemyStates = EnemyStates.GUARD;
                        else
                            enemyStates = EnemyStates.PATROL;
                    }
                    else
                    {
                        isFollow = true;
                        agent.destination = attackTarget.transform.transform.position;
                    }
                }            
                break;
            case EnemyStates.DEAD:
                {
                    //防止死亡移动，关闭agent
                    agent.enabled = false;
                    agent.radius = 0;
                    anim.SetTrigger("Death");
                    Destroy(gameObject, 2f);
                }
                break;
        }

    }

    //切换动画
    void SwitchAnimation()
    {
        anim.SetBool("Walk", isWalk);
        anim.SetBool("Chase", isChase);
        anim.SetBool("Follow", isFollow);
    }

    //画出选中怪物的视野范围
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRadius);
    }

    //获取巡逻范围内随机一点
    void GetNewWayPoint()
    {
        remainLookAtTime = lookAtTime;

        float randomX = Random.Range(-patrolRange, patrolRange);
        float randomZ = Random.Range(-patrolRange, patrolRange);

        Vector3 randomPoint = new Vector3(guardPos.x + randomX, transform.position.y, guardPos.z + randomZ);

        NavMeshHit hit;
        wayPoint = NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1) ? hit.position : transform.position;
    }

    //被造成伤害
    public void TakeDamage(int damage)
    {
        nowHealth = nowHealth - damage;
        if (nowHealth <= 0)
        {
            healthBar.value = 0;
            isDead = true;
        }
        else
        {
            healthBar.value = nowHealth;
            anim.SetTrigger("Hit");
        }
    }
}
