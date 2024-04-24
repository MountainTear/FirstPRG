using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// �Ź�����������ʧ�ܣ�ʤ���͹ؿ�
/// </summary>
public class Door : MonoBehaviour
{
    //��ת�Ĺؿ�Id
    public int levelId;
    private PlayerDataManager playerDataM;
    private PopViewManager popViewM;

    private void Start()
    {
        //��ȡ���ݹ�����
        playerDataM = GameObject.Find("GameManager").GetComponent<PlayerDataManager>();
        popViewM = GameObject.Find("GameManager").GetComponent<PopViewManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<CharacterController>())
        {
            //����ξ���Ƭ������
            if (playerDataM.playerData.fragment < playerDataM.playerData.fargmentMax)
            {
                popViewM.IntroducePopView("�ξ���Ƭ���㣬�޷���Խ�ξ�");
                return;
            }
            //��������
            playerDataM.SavePlayerData();
            //���عؿ�
            SceneManager.LoadScene(levelId);
        }
    }  


}
