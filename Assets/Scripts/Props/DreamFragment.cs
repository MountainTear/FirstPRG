using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// √Œæ≥ÀÈ∆¨
/// </summary>
public class DreamFragment : MonoBehaviour
{
    private PlayerDataManager playerDataManager;

    private void Start()
    {
        playerDataManager = GameObject.Find("GameManager").GetComponent<PlayerDataManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerDataManager.getFragment();
            Destroy(this.gameObject);
        }
    }

}
