using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 角色控制器
/// </summary>
public class CharacterController : MonoBehaviour
{
    //移动速度，转向速度及跳跃高度
    [SerializeField] private float m_moveSpeed = 2;
    [SerializeField] private float m_turnSpeed = 200;
    [SerializeField] private float m_jumpForce = 4;

    //动画及刚体组件
    private Animator m_animator = null;
    private Rigidbody m_rigidBody = null;
    //音效管理
    private AudioManager audioManager;

    private float m_currentV = 0;
    private float m_currentH = 0;

    private readonly float m_interpolation = 10;

    //向后移动速度
    private readonly float m_backwardRunScale = 0f;
    private bool m_wasGrounded;
    private float m_jumpTimeStamp = 0;
    private float m_minJumpInterval = 0.25f;
    private bool m_jumpInput = false;
    private bool m_isGrounded;

    //记录初始位置
    private Vector3 initialPosition;
    //飘浮计时器
    private float floatTimer;
    //弹窗
    private PopViewManager popViewM;
    //碰撞列表
    private List<Collider> m_collisions = new List<Collider>();

    public GameObject man;
    public GameObject women;
    private PlayerDataManager playerDataManager;

    private void Awake()
    {
        //获取组件
        m_rigidBody = gameObject.GetComponent<Rigidbody>();
        popViewM = GameObject.Find("GameManager").GetComponent<PopViewManager>();
        audioManager = GameObject.Find("GameManager").GetComponent<AudioManager>();

        //记录初始位置
        initialPosition = transform.position;
    }

    private void Start()
    {
        playerDataManager = GameObject.Find("GameManager").GetComponent<PlayerDataManager>();
        if (playerDataManager.playerData.sex == 0)
        {
            man.SetActive(true);
            women.SetActive(false);
            m_animator = man.GetComponent<Animator>();
        }
        else
        {
            man.SetActive(false);
            women.SetActive(true);
            m_animator = women.GetComponent<Animator>();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                if (!m_collisions.Contains(collision.collider))
                {
                    m_collisions.Add(collision.collider);
                }
                m_isGrounded = true;
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        bool validSurfaceNormal = false;
        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                validSurfaceNormal = true; break;
            }
        }

        if (validSurfaceNormal)
        {
            m_isGrounded = true;
            if (!m_collisions.Contains(collision.collider))
            {
                m_collisions.Add(collision.collider);
            }
        }
        else
        {
            if (m_collisions.Contains(collision.collider))
            {
                m_collisions.Remove(collision.collider);
            }
            if (m_collisions.Count == 0) { m_isGrounded = false; }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (m_collisions.Contains(collision.collider))
        {
            m_collisions.Remove(collision.collider);
        }
        if (m_collisions.Count == 0) { m_isGrounded = false; }
    }

    private void Update()
    {
        //获取跳跃输入
        if (!m_jumpInput && Input.GetKey(KeyCode.Space))
        {
            m_jumpInput = true;
        }

        //记录飘浮时间
        if (!m_isGrounded)
        {
            floatTimer = floatTimer + Time.deltaTime;
            //飘浮时间过长回归原位
            if(floatTimer > 6)
            {
                popViewM.IntroducePopView("坠落时间过长，返回起点");
                transform.position = initialPosition;
                //制止下落
                m_rigidBody.isKinematic = true;
                m_rigidBody.isKinematic = false;
                floatTimer = 0;
            }
        }
        else
        {
            floatTimer = 0;
        }
    }

    private void FixedUpdate()
    {
        m_animator.SetBool("Grounded", m_isGrounded);
        TankUpdate();

        m_wasGrounded = m_isGrounded;
        m_jumpInput = false;
    }

    /// <summary>
    /// 移动和调用函数
    /// </summary>
    private void TankUpdate()
    {
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");

        //向后移动
        if (v < 0)
        {
            v *= m_backwardRunScale; 
        }

        //插值计算
        m_currentV = Mathf.Lerp(m_currentV, v, Time.deltaTime * m_interpolation);
        m_currentH = Mathf.Lerp(m_currentH, h, Time.deltaTime * m_interpolation);

        transform.position += transform.forward * m_currentV * m_moveSpeed * Time.deltaTime;
        transform.Rotate(0, m_currentH * m_turnSpeed * Time.deltaTime, 0);

        m_animator.SetFloat("MoveSpeed", m_currentV);

        JumpingAndLanding();
    }

    /// <summary>
    /// 跳跃
    /// </summary>
    private void JumpingAndLanding()
    {
        bool jumpCooldownOver = (Time.time - m_jumpTimeStamp) >= m_minJumpInterval;

        if (jumpCooldownOver && m_isGrounded && m_jumpInput)
        {
            m_jumpTimeStamp = Time.time;
            m_rigidBody.AddForce(Vector3.up * m_jumpForce, ForceMode.Impulse);
        }

        if (!m_wasGrounded && m_isGrounded)
        {
            m_animator.SetTrigger("Land");
        }

        if (!m_isGrounded && m_wasGrounded)
        {
            m_animator.SetTrigger("Jump");
            audioManager.PlayJump();
        }
    }   
}

