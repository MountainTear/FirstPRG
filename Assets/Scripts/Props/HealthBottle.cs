using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ξ���Ƭ
/// </summary>
public class HealthBottle : MonoBehaviour
{
    //�ָ�Ѫ��
    public int addHealth = 2;
    private PlayerDataManager playerDataManager;

    private void Start()
    {
        playerDataManager = GameObject.Find("GameManager").GetComponent<PlayerDataManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerDataManager.getHealthBottle(addHealth);
            Destroy(this.gameObject);
        }
    }

}
