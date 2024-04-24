using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ħ���������ʳ������
/// </summary>
public enum FoodType
{ 
    /// <summary>
    /// ���棬��Ӧ������
    /// </summary>
    COOKIE,
    /// <summary>
    /// ƻ������Ӧ�ӵ�
    /// </summary>
    APPLE
}

/// <summary>
/// ħ�����
/// </summary>
public class MagicBag : MonoBehaviour
{
    //ħ��ʳ��Ԥ����
    private GameObject cookie;
    private GameObject apple;

    //ʳ�����
    private FoodType foodStates;

    //ʹ����ȴ
    public float coolTime = 1f;
    private float coolTimer = 0;
    private bool canUse = true;

    //��Ч����
    private AudioManager audioManager;

    //UI
    private UIGameInManager uIGameInManager;
    void Awake()
    {
        //Ԥ����
        apple = (GameObject)Resources.Load("Prefabs/MagicFood/Bullet");
        cookie = (GameObject)Resources.Load("Prefabs/MagicFood/FloatingPlate");
        uIGameInManager = GameObject.Find("GameManager").GetComponent<UIGameInManager>();
        audioManager = GameObject.Find("GameManager").GetComponent<AudioManager>();

        //Ĭ��ħ��ʳ��Ϊƻ��
        foodStates = FoodType.COOKIE;
    }

    void Update()
    {
        //��ȴ
        if(coolTimer > coolTime)
        {
            canUse = true;
        }
        else
        {
            coolTimer = coolTimer + Time.deltaTime;
        }

        //�л�ħ��ʳ��
        if (Input.GetButtonDown("Switch"))
        {
            audioManager.PlaySwitchFood();
            //�˴���ʱ��enum������
            if (foodStates == FoodType.APPLE)
            {
                foodStates = FoodType.COOKIE;
            }
            else
            {
                foodStates = foodStates + 1;
            }
            //����UI����
            uIGameInManager.UpdateFood(foodStates);
        }
        //ʹ��ħ��
        if(Input.GetButtonDown("Magic")  && canUse)
        {
            audioManager.PlayMagic();
            SwitchStates();
            coolTimer = 0;
            canUse = false;
        }
    }
    
    void SwitchStates()
    {
        //����ħ��
        switch (foodStates)
        {

            case FoodType.APPLE:
                {
                    Instantiate(apple, transform.position + Vector3.up * 0.5f, Quaternion.identity);
                }
                break;
            case FoodType.COOKIE:
                {
                    Instantiate(cookie, transform.position + transform.forward * -1.5f , Quaternion.identity);
                }
                break;
        }
    }
}
