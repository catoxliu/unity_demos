using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paddle : MonoBehaviour {

    public float m_fPaddleSpeed = 1.0f;

    //TODO: This may could be calculated from screen.width.
    private float m_fMaxMove = 7.5f;
    private Vector3 m_vStartPos;

	void Start () {
        m_vStartPos = transform.position;
    }
	
	// Update is called once per frame
	void Update () {
        float move = Mathf.Clamp(transform.position.x + (Input.GetAxis("Horizontal") * m_fPaddleSpeed), -m_fMaxMove, m_fMaxMove) - transform.position.x;
        transform.Translate(new Vector3(move,0,0));
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Gold"))
        {
            GameManager.Instance.GoldCollection();
        }
    }

    public void Reset()
    {
        transform.position = m_vStartPos;
    }
}
