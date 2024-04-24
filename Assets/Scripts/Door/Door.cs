using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 门管理器，触发失败，胜利和关卡
/// </summary>
public class Door : MonoBehaviour
{
    //跳转的关卡Id
    public int levelId;
    private PlayerDataManager playerDataM;
    private PopViewManager popViewM;

    private void Start()
    {
        //获取数据管理器
        playerDataM = GameObject.Find("GameManager").GetComponent<PlayerDataManager>();
        popViewM = GameObject.Find("GameManager").GetComponent<PopViewManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<CharacterController>())
        {
            //如果梦境碎片数不够
            if (playerDataM.playerData.fragment < playerDataM.playerData.fargmentMax)
            {
                popViewM.IntroducePopView("梦境碎片不足，无法穿越梦境");
                return;
            }
            //保存数据
            playerDataM.SavePlayerData();
            //加载关卡
            SceneManager.LoadScene(levelId);
        }
    }  


}
