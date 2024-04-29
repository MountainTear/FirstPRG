using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//玩家数据类
public class PlayerData
{
    public int healthMax = 10;
    public int fargmentMax = 3;
    public int health = 0;
    public int fragment = 0;
    public int sex = 0;
}

/// <summary>
/// 管理玩家游戏数据
/// </summary>
public class PlayerDataManager : MonoBehaviour
{
    public PlayerData playerData = new PlayerData();
    private UIGameInManager uIGameInManager;
    private PopViewManager popViewM;

    //受伤闪烁
    private bool isInjured;
    private float injuredTimer;
    private SkinnedMeshRenderer m_renderer;

    private void Awake()
    {
        //获取存储的主角数据
        playerData.health  = PlayerPrefs.GetInt("Health", playerData.healthMax) % (playerData.healthMax + 1);
        playerData.sex = PlayerPrefs.GetInt("Sex", 0);
        //playerData.fargment = PlayerPrefs.GetInt("Energy", playerData.fargmentMax) % (playerData.fargmentMax + 1);
        //获取组件
        uIGameInManager = GetComponent<UIGameInManager>();
        popViewM = GetComponent<PopViewManager>();
        m_renderer = GameObject.Find("Lin/skin").GetComponent<SkinnedMeshRenderer>();
    }

    //保存数据
    public  void SavePlayerData()
    {
        PlayerPrefs.SetInt("Health", playerData.health);
        PlayerPrefs.SetInt("Sex", playerData.sex);
        //PlayerPrefs.SetInt("Energy", playerData.fargment);
    }

    void Update()
    {
        //判定受伤闪烁
        if (isInjured)
        {
            injuredTimer += Time.deltaTime;
            if (injuredTimer < 3f)
            {
                float remainder = injuredTimer % 0.3f;
                m_renderer.enabled = remainder > 0.15f;
            }
            else
            {
                injuredTimer = 0;
                m_renderer.enabled = true;
                isInjured = false;
            }
        }
    }

    //受伤
    public void Injured(int damage)
    {
        isInjured = true;
        if (playerData.health > damage)
        {
            playerData.health = playerData.health - damage;
            //更新UI
            uIGameInManager.UpdateUI();
        }
        else
        {
            //此处可播放死亡动画
            SceneManager.LoadScene("EndInterface");
        }
    }

    //获得梦境碎片
    public void getFragment()
    {
        playerData.fragment = playerData.fragment + 1;
        uIGameInManager.UpdateUI();
        popViewM.IntroducePopView("恭喜获得一个梦境碎片");
    }

    //获得血瓶
    public void getHealthBottle(int addHealth)
    {
        playerData.health = playerData.health + addHealth;
        if (playerData.health > playerData.healthMax)
        {
            playerData.health = playerData.healthMax;
        }
        uIGameInManager.UpdateUI();
        popViewM.IntroducePopView("恭喜加血" + addHealth + "滴");
    }
}
