using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// �������������ڶ�NPC�ĶԻ��ȣ�����ģʽ
/// </summary>
public class MouseManager : MonoBehaviour
{
    public static MouseManager Instance;

    public Texture2D normal, talk, attack, door;

    //����������߽��
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
        //����������������λ�õ���
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hitInfo))
        {
            //�л������ͼ
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

    //��갴�´���������¼�
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
