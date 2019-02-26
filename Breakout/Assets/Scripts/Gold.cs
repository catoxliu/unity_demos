using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gold : MonoBehaviour {

    //Gold is only collision with Paddle and sides due to layer-based collision.

    private void OnCollisionEnter(Collision collision)
    {
        GameManager.Instance.RecycleResource(this);
    }
}
