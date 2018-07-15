using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapeBehaviour : MonoBehaviour {

    public float mTapeIncrement = 0.003f;

    private Mesh mTapeMesh;

	// Use this for initialization
	void Start () {
        var mf = GetComponent<MeshFilter>();
        mTapeMesh = mf.mesh;
        if (mTapeMesh == null)
        {
            Debug.LogError("Could not find tape!");
        }
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey(KeyCode.Space))
        {
            Vector3[] vers = mTapeMesh.vertices;
            vers[1].y = vers[1].y + mTapeIncrement;
            vers[3].y = vers[3].y + mTapeIncrement;
            mTapeMesh.vertices = vers;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            TapeWrap();
        }
	}

    void TapeWrap()
    {

    }
}
