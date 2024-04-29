using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SexChoose : MonoBehaviour
{
    public ToggleGroup toggleGroup;

    void Start()
    {
        // 获取所有 Toggle 组件并为它们添加监听器
        var toggles = toggleGroup.GetComponentsInChildren<Toggle>();
        foreach (var toggle in toggles)
        {
            toggle.onValueChanged.AddListener(delegate {
                OnToggleChanged(toggle);
            });
        }
    }

    // 当任何一个 Toggle 状态发生变化时调用
    private void OnToggleChanged(Toggle changedToggle)
    {
        if (changedToggle.isOn)
        {
            if (changedToggle.name == "Man")
                PlayerPrefs.SetInt("Sex", 0);
            else
                PlayerPrefs.SetInt("Sex", 1);
        }
    }
}
