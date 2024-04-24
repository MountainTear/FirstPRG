using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 遮挡剔除
/// </summary>
public class OcclusionCulling : MonoBehaviour
{
    //观察目标
    public Transform target;
    //上次碰撞到的物体
    private List<GameObject> lastColliderObject  =new List<GameObject>();
    //本次碰撞到的物体
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
        //获取当前物体材质球数量
        int materialsNumber = _renderer.sharedMaterials.Length;
        for(int i =0; i < materialsNumber; i++)
        {
            Debug.LogError(i);
            //获取当前材质球颜色
            Color color = _renderer.materials[i].color;
            //设置透明度取值范围:0~l; 0=完全透明
            color.a = transpa;
            //置当前材质球颜色
             _renderer.materials[i].SetColor("_Color", color);
        }
    }

}

