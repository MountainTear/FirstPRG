using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏初始化器
/// </summary>
public class GameInitialization : MonoBehaviour
{
    private void Awake()
    {
        ClearData();
    }

    /// <summary>
    /// 清空注册表保存的数据
    /// </summary>
    private void ClearData()
    {
        PlayerPrefs.DeleteAll();
    }
}
