using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 鼠标管理器，用于对NPC的对话等，单例模式
/// </summary>
public class MouseManager : MonoBehaviour
{
    public static MouseManager Instance;

    public Texture2D normal, talk, attack, door;

    //接受鼠标射线结果
    RaycastHit hitInfo;

    //public event Action<Vector3> OnMouseClicked;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }

    private void Update()
    {
        SetCursortexture();
        MouseControl();
    }

    void SetCursortexture()
    {
        //生成摄像机到鼠标点击位置的线
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hitInfo))
        {
            //切换鼠标贴图
            switch (hitInfo.collider.gameObject.tag)
            {
                case "Npc":
                    Cursor.SetCursor(talk, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case "Enemy":
                    Cursor.SetCursor(attack, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case "Door":
                    Cursor.SetCursor(door, new Vector2(16, 16), CursorMode.Auto);
                    break;
                default:
                    Cursor.SetCursor(normal, new Vector2(16, 16), CursorMode.Auto);
                    break;
            }
        }
    }

    //鼠标按下触发鼠标点击事件
    void MouseControl()
    {
        if (Input.GetMouseButtonDown(0) && hitInfo.collider != null)
        {
            if(hitInfo.collider.gameObject.CompareTag("Npc"))
            {
                NpcManager npc = hitInfo.collider.GetComponent<NpcManager>();
                if (npc != null)
                {
                    npc.DisplayDialog();
                }
            }
        }
    }
}
