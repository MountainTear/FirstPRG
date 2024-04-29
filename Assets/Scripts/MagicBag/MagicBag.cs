using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 魔法背包里的食物类型
/// </summary>
public enum FoodType
{ 
    /// <summary>
    /// 曲奇，对应悬浮板
    /// </summary>
    COOKIE,
    /// <summary>
    /// 苹果，对应子弹
    /// </summary>
    APPLE
}

/// <summary>
/// 魔力书包
/// </summary>
public class MagicBag : MonoBehaviour
{
    //魔法食物预加载
    private GameObject cookie;
    private GameObject apple;

    //食物类别
    private FoodType foodStates;

    //使用冷却
    public float coolTime = 1f;
    private float coolTimer = 0;
    private bool canUse = true;

    //音效管理
    private AudioManager audioManager;

    //UI
    private UIGameInManager uIGameInManager;
    public CharacterController characterController;

    void Awake()
    {
        //预加载
        apple = (GameObject)Resources.Load("Prefabs/MagicFood/Bullet");
        cookie = (GameObject)Resources.Load("Prefabs/MagicFood/FloatingPlate");
        uIGameInManager = GameObject.Find("GameManager").GetComponent<UIGameInManager>();
        audioManager = GameObject.Find("GameManager").GetComponent<AudioManager>();

        //默认魔法食物为苹果
        foodStates = FoodType.COOKIE;
    }

    void Update()
    {
        //冷却
        if(coolTimer > coolTime)
        {
            canUse = true;
        }
        else
        {
            coolTimer = coolTimer + Time.deltaTime;
        }

        //切换魔法食物
        if (Input.GetButtonDown("Switch"))
        {
            audioManager.PlaySwitchFood();
            //此处暂时随enum而更改
            if (foodStates == FoodType.APPLE)
            {
                foodStates = FoodType.COOKIE;
            }
            else
            {
                foodStates = foodStates + 1;
            }
            //更改UI描述
            uIGameInManager.UpdateFood(foodStates);
        }
        //使用魔法
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
        //发动魔法
        switch (foodStates)
        {

            case FoodType.APPLE:
                {
                    Instantiate(apple, transform.position + Vector3.up * 0.5f, Quaternion.identity);
                    characterController.Attack();
                }
                break;
            case FoodType.COOKIE:
                {
                    Instantiate(cookie, transform.position + transform.forward * -3f , Quaternion.identity);
                }
                break;
        }
    }
}
