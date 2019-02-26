using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesManager : MonoBehaviour {

    public GameObject m_vBrickPrefab;
    public GameObject m_vGoldPrefab;
    public int m_iInitNum = 10;

    private List<GameObject> m_vGoldPool = new List<GameObject>();
    private List<GameObject> m_vBrickPool = new List<GameObject>();

    private void Start()
    {
        GenerateResource(m_vGoldPrefab, m_iInitNum, m_vGoldPool);
    }

    void GenerateResource(GameObject prefab, int num, List<GameObject> pool)
    {
        for (int i = 0; i < num; i++)
        {
            var go = Instantiate(prefab, transform);
            go.SetActive(false);
            pool.Add(go);
        }
    }

    public void RecycleResource(Object resource)
    {
        var mono = (MonoBehaviour)resource;
        mono.gameObject.SetActive(false);
        mono.transform.parent = transform;
        if (resource is Gold)
        {
            m_vGoldPool.Add(mono.gameObject);
        }
        else if (resource is Brick)
        {
            m_vBrickPool.Add(mono.gameObject);
        }
    }

    public GameObject[] GetGold(int num = 1)
    {
        var result = new GameObject[num];
        if (num > m_vGoldPool.Count)
        {
            GenerateResource(m_vGoldPrefab, num - m_vGoldPool.Count, m_vGoldPool);
        }
        m_vGoldPool.CopyTo(0, result, 0, num);
        m_vGoldPool.RemoveRange(0, num);
        return result;
    }

    public GameObject[] GetBrick(int num = 1)
    {
        var result = new GameObject[num];
        if (num > m_vBrickPool.Count)
        {
            GenerateResource(m_vBrickPrefab, num - m_vBrickPool.Count, m_vBrickPool);
        }
        m_vBrickPool.CopyTo(0, result, 0, num);
        m_vBrickPool.RemoveRange(0, num);
        return result;
    }

}
