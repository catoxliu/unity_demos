using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour {

    //make brick has "life" to make it fun.
    public int m_iHitsToBreak = 1;

    private int m_iHitsRemains;
    private MeshRenderer m_vRenderer;
    private MaterialPropertyBlock m_vMPB;

    private void Awake()
    {
        m_vRenderer = GetComponent<MeshRenderer>();
        m_vMPB = new MaterialPropertyBlock();
    }

    private void OnEnable()
    {
        m_iHitsToBreak = Mathf.CeilToInt(Random.Range(0, 1.0f) * 3);
        m_iHitsRemains = m_iHitsToBreak;
        SetColor();
    }

    private void OnCollisionEnter(Collision collision)
    {
        m_iHitsRemains--;
        if(m_iHitsRemains == 0)
        {
            GameManager.Instance.BrickBreak(this);
            //Return for not pass 0 to shader.
            return;
        }
        SetColor();
    }

    //Using property block is more efficient than duplicating material.
    void SetColor()
    {
        m_vRenderer.GetPropertyBlock(m_vMPB);
        m_vMPB.SetInt("_Hit", m_iHitsRemains);
        m_vRenderer.SetPropertyBlock(m_vMPB);
    }
}
