using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//���������
public class PlayerData
{
    public int healthMax = 10;
    public int fargmentMax = 3;
    public int health = 0;
    public int fragment = 0;
}

/// <summary>
/// ���������Ϸ����
/// </summary>
public class PlayerDataManager : MonoBehaviour
{
    public PlayerData playerData = new PlayerData();
    private UIGameInManager uIGameInManager;
    private PopViewManager popViewM;

    //������˸
    private bool isInjured;
    private float injuredTimer;
    private SkinnedMeshRenderer m_renderer;

    private void Awake()
    {
        //��ȡ�洢����������
        playerData.health  = PlayerPrefs.GetInt("Health", playerData.healthMax) % (playerData.healthMax + 1);
        //playerData.fargment = PlayerPrefs.GetInt("Energy", playerData.fargmentMax) % (playerData.fargmentMax + 1);
        //��ȡ���
        uIGameInManager = GetComponent<UIGameInManager>();
        popViewM = GetComponent<PopViewManager>();
        m_renderer = GameObject.Find("Lin/skin").GetComponent<SkinnedMeshRenderer>();
    }

    //��������
    public  void SavePlayerData()
    {
        PlayerPrefs.SetInt("Health", playerData.health);
        //PlayerPrefs.SetInt("Energy", playerData.fargment);
    }

    void Update()
    {
        //�ж�������˸
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

    //����
    public void Injured(int damage)
    {
        isInjured = true;
        if (playerData.health > damage)
        {
            playerData.health = playerData.health - damage;
            //����UI
            uIGameInManager.UpdateUI();
        }
        else
        {
            //�˴��ɲ�����������
            SceneManager.LoadScene("EndInterface");
        }
    }

    //����ξ���Ƭ
    public void getFragment()
    {
        playerData.fragment = playerData.fragment + 1;
        uIGameInManager.UpdateUI();
        popViewM.IntroducePopView("��ϲ���һ���ξ���Ƭ");
    }

    //���Ѫƿ
    public void getHealthBottle(int addHealth)
    {
        playerData.health = playerData.health + addHealth;
        if (playerData.health > playerData.healthMax)
        {
            playerData.health = playerData.healthMax;
        }
        uIGameInManager.UpdateUI();
        popViewM.IntroducePopView("��ϲ��Ѫ" + addHealth + "��");
    }
}
