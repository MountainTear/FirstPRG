using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ��ϷUI������
/// </summary>
public class UIGameInManager: MonoBehaviour
{
    //Ѫ�������ξ���Ƭ��
    private Text healthT;
    private Text fragmentT;
    private PlayerDataManager playerDataManager;

    //����ʳ�Ｐ���ܽ���
    private Text foodT;
    private Text powerT;

    //С��ͼ
    private GameObject map;
    private Button mapButton;
    private void Start()
    {
        //��ȡ���     
        healthT = GameObject.Find("UIGameIn/Canvas/Content/Health").GetComponent<Text>();
        fragmentT = GameObject.Find("UIGameIn/Canvas/Content/Fragment").GetComponent<Text>();
        foodT = GameObject.Find("UIGameIn/Canvas/MagicFoodContent/FoodName").GetComponent<Text>();
        powerT = GameObject.Find("UIGameIn/Canvas/MagicFoodContent/PowerName").GetComponent<Text>();
        mapButton = GameObject.Find("UIGameIn/Canvas/MapButton").GetComponent<Button>();
        map = GameObject.Find("UIGameIn/Canvas/Map");
        playerDataManager = GetComponent<PlayerDataManager>();
        //UI��ʼ��
        healthT.text = playerDataManager.playerData.health + "/" + playerDataManager.playerData.healthMax;
        fragmentT.text = playerDataManager.playerData.fragment + "/" + playerDataManager.playerData.fargmentMax;

        //��ť��Ӧ
        mapButton.onClick.AddListener( () => { map.SetActive(!map.activeSelf);  });
    }

    /// <summary>
    /// UI���º���
    /// </summary>
    public void UpdateUI()
    {
        healthT.text = playerDataManager.playerData.health + "/" + playerDataManager.playerData.healthMax;
        fragmentT.text = playerDataManager.playerData.fragment + "/" + playerDataManager.playerData.fargmentMax;
    }

    /// <summary>
    /// ����ʳ��
    /// </summary>
    public void UpdateFood(FoodType foodStates)
    {
        switch (foodStates)
        {
            case FoodType.APPLE:
                {
                    foodT.text = "ƻ��";
                    powerT.text = "����";
                }
                break;
            case FoodType.COOKIE:
                {
                    foodT.text = "����";
                    powerT.text = "����";
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
