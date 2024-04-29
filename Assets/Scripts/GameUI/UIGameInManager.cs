using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 游戏UI管理类
/// </summary>
public class UIGameInManager: MonoBehaviour
{
    //血量条及梦境碎片条
    private Text healthT;
    private Text fragmentT;
    private PlayerDataManager playerDataManager;

    //背包食物及功能介绍
    private Text foodT;
    private Text powerT;

    //小地图
    private GameObject map;
    private Button mapButton;
    private void Start()
    {
        //获取组件     
        healthT = GameObject.Find("UIGameIn/Canvas/Content/Health").GetComponent<Text>();
        fragmentT = GameObject.Find("UIGameIn/Canvas/Content/Fragment").GetComponent<Text>();
        foodT = GameObject.Find("UIGameIn/Canvas/MagicFoodContent/FoodName").GetComponent<Text>();
        powerT = GameObject.Find("UIGameIn/Canvas/MagicFoodContent/PowerName").GetComponent<Text>();
        mapButton = GameObject.Find("UIGameIn/Canvas/MapButton").GetComponent<Button>();
        map = GameObject.Find("UIGameIn/Canvas/Map");
        playerDataManager = GetComponent<PlayerDataManager>();
        //UI初始化
        healthT.text = playerDataManager.playerData.health + "/" + playerDataManager.playerData.healthMax;
        fragmentT.text = playerDataManager.playerData.fragment + "/" + playerDataManager.playerData.fargmentMax;

        //按钮响应
        mapButton.onClick.AddListener( () => { map.SetActive(!map.activeSelf);  });
    }

    /// <summary>
    /// UI更新函数
    /// </summary>
    public void UpdateUI()
    {
        healthT.text = playerDataManager.playerData.health + "/" + playerDataManager.playerData.healthMax;
        fragmentT.text = playerDataManager.playerData.fragment + "/" + playerDataManager.playerData.fargmentMax;
    }

    /// <summary>
    /// 更新食物
    /// </summary>
    public void UpdateFood(FoodType foodStates)
    {
        switch (foodStates)
        {
            case FoodType.APPLE:
                {
                    foodT.text = "战斗";
                    powerT.text = "攻击";
                }
                break;
            case FoodType.COOKIE:
                {
                    foodT.text = "地毯";
                    powerT.text = "地形";
                }
                break;
        }
    }

    private void Update()
    {
        if(Input.GetButtonDown("Map"))
        {
            map.SetActive(!map.activeSelf);
        }
    }
}
