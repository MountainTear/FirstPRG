using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��Ϸ��ʼ����
/// </summary>
public class GameInitialization : MonoBehaviour
{
    private void Awake()
    {
        ClearData();
    }

    /// <summary>
    /// ���ע����������
    /// </summary>
    private void ClearData()
    {
        PlayerPrefs.DeleteAll();
    }
}
