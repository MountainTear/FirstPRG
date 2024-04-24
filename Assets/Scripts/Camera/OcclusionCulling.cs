using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ڵ��޳�
/// </summary>
public class OcclusionCulling : MonoBehaviour
{
    //�۲�Ŀ��
    public Transform target;
    //�ϴ���ײ��������
    private List<GameObject> lastColliderObject  =new List<GameObject>();
    //������ײ��������
    private List<GameObject> colliderObject = new List<GameObject>();

    private void Update()
    {
        GetTerrain();
    }

    private void GetTerrain()
    {
        Vector3 aim = target.position;
        Vector3 ve = (target.position - transform.position).normalized;
        float an = transform.eulerAngles.y;
        aim -= an * ve;

        Debug.DrawLine(target.position, aim, Color.red);

        RaycastHit[] hit;
        hit = Physics.RaycastAll(target.position, aim, 100f);

        for (int i = 0; i < colliderObject.Count; i++)
        {
            lastColliderObject.Add(colliderObject[i]);
        }

        colliderObject.Clear();
        for (int i  = 0; i < hit.Length; i++)
        {
            if( hit[i].collider.gameObject.layer != LayerMask.NameToLayer("Player")
                && hit[i].collider.gameObject.layer != LayerMask.NameToLayer("Npc")
                && hit[i].collider.gameObject.layer != LayerMask.NameToLayer("Enemy"))
            {
                //Debug.LogError(hit[i].collider.gameObject);
                colliderObject.Add(hit[i].collider.gameObject);
                SetMaterialsColor(hit[i].collider.gameObject.GetComponent<MeshRenderer>(), 0.4f);
            }
        }
    }

    private void  SetMaterialsColor(Renderer _renderer, float transpa)
    {
        //��ȡ��ǰ�������������
        int materialsNumber = _renderer.sharedMaterials.Length;
        for(int i =0; i < materialsNumber; i++)
        {
            Debug.LogError(i);
            //��ȡ��ǰ��������ɫ
            Color color = _renderer.materials[i].color;
            //����͸����ȡֵ��Χ:0~l; 0=��ȫ͸��
            color.a = transpa;
            //�õ�ǰ��������ɫ
             _renderer.materials[i].SetColor("_Color", color);
        }
    }

}

