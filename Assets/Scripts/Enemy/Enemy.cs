using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

/// <summary>
/// ����״̬
/// </summary>
public enum EnemyStates {
    /// <summary>
    /// վ׮
    /// </summary>
    GUARD,  
    /// <summary>
    /// Ѳ��
    /// </summary>
    PATROL,     
    /// <summary>
    /// ׷��
    /// </summary>
    CHASE,  
    /// <summary>
    /// ����
    /// </summary>
    DEAD    
}
/// <summary>
/// ���˻���
/// </summary>
/// 
//�Զ����NavMeshAgent���
[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    //���˴���
    private NavMeshAgent agent;
    //�������
    private Animator anim;
    //Ѫ�����
    private Slider healthBar;

    //������
    private PlayerDataManager playerDataManager;
    protected GameObject attackTarget;

    //����״̬
    private EnemyStates enemyStates;

    //���˲���
    public int damage;    //�˺�
    public int maxHealth = 10;
    public int nowHealth = 10; 
    public float sightRadius;   //��Ұ
    public bool isGuard;    //�Ƿ�վ׮
    private float speed;    //��¼����ԭ�е��ٶ�
    public float lookAtTime; //��ԭ�ؿ���ʱ��
    private float remainLookAtTime; //��ʱ��

    //վ׮��Ѳ�߲���
    private Vector3 guardPos;
    private Quaternion guardRotation;

    //Ѳ�����
    public float patrolRange;   //Ѳ�߷�Χ
    private Vector3 wayPoint;   //Ѳ�ߵ����һ��

    //bool��϶���
    bool isWalk;
    bool isChase;
    bool isFollow;
    bool isDead;

    //��ײ���Ͽ�Ѫ
    public float stayDamageTime = 2f;
    private float stayTimer;

    void Awake()
    {
        //��ȡ���
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        healthBar = transform.Find("Canvas/Slider").gameObject.GetComponent<Slider>();
        speed = agent.speed;

        guardPos = transform.position;
        guardRotation = transform.rotation;
        remainLookAtTime = lookAtTime;
    }

    void Start()
    {
        //��ȡ���ݹ�����������������ݳ�ʼ��
        playerDataManager = GameObject.Find("GameManager").GetComponent<PlayerDataManager>();

        //�����ѡվ׮
        if (isGuard)
        {
            enemyStates = EnemyStates.GUARD;
        }
        //����Ѳ��
        else
        {
            enemyStates = EnemyStates.PATROL;
            //��ȡ��Ѳ�߷�Χ�ڵĵ�
            GetNewWayPoint();
        }

        //Ѫ����ʼ��
        healthBar.minValue = 0;
        healthBar.maxValue = maxHealth;
    }

    void Update()
    {
        SwitchStates();
        SwitchAnimation();
    }

    //��ײ�˺�
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
    //�ҵ����
    bool FoundPlayer()
    {
        //��ȡ������Χ��������ײ��
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

    //�л�״̬
    void SwitchStates()
    {
        if (isDead)
        {
            enemyStates = EnemyStates.DEAD;
        }
        //�������player �л���CHASE
        else if (FoundPlayer())
        {
            agent.stoppingDistance = agent.radius;
            enemyStates = EnemyStates.CHASE;
        }
        else
        {
            agent.stoppingDistance = agent.radius;
        }

        //���˲�ͬ״̬����ͬ��
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

                    //�ж��Ƿ������Ѳ�ߵ�
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
                    //׷��ģʽȫ��
                    agent.speed = speed;
                    //׷��������δ�ҵ����
                    if (!FoundPlayer())
                    {
                        isFollow = false;
                        if (remainLookAtTime > 0)
                        {
                            agent.destination = transform.position;
                            remainLookAtTime -= Time.deltaTime;
                        }
                        //������һ��״̬
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
                    //��ֹ�����ƶ����ر�agent
                    agent.enabled = false;
                    agent.radius = 0;
                    anim.SetTrigger("Death");
                    Destroy(gameObject, 2f);
                }
                break;
        }

    }

    //�л�����
    void SwitchAnimation()
    {
        anim.SetBool("Walk", isWalk);
        anim.SetBool("Chase", isChase);
        anim.SetBool("Follow", isFollow);
    }

    //����ѡ�й������Ұ��Χ
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRadius);
    }

    //��ȡѲ�߷�Χ�����һ��
    void GetNewWayPoint()
    {
        remainLookAtTime = lookAtTime;

        float randomX = Random.Range(-patrolRange, patrolRange);
        float randomZ = Random.Range(-patrolRange, patrolRange);

        Vector3 randomPoint = new Vector3(guardPos.x + randomX, transform.position.y, guardPos.z + randomZ);

        NavMeshHit hit;
        wayPoint = NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1) ? hit.position : transform.position;
    }

    //������˺�
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
