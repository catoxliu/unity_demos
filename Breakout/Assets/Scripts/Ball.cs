using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {

    public float m_fInitialSpeed = 10f;

    private Vector3 m_vStartPos;
    private Rigidbody m_vRgidibody;
    private bool m_bFireBall = false;

    // Use this for initialization
    void Start () {
        m_vRgidibody = GetComponent<Rigidbody>();
        m_vStartPos = transform.position;
    }

    public void Reset()
    {
        gameObject.SetActive(true);
        m_bFireBall = false;
        m_vRgidibody.velocity = Vector3.zero;
        m_vRgidibody.isKinematic = true;
        transform.position = m_vStartPos;
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetButtonDown("Fire1") && m_bFireBall == false)
        {
            transform.parent = null;
            m_bFireBall = true;
            m_vRgidibody.isKinematic = false;
            m_vRgidibody.velocity = new Vector3(m_fInitialSpeed, m_fInitialSpeed, 0);
        }
    }
}
