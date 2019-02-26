using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomSide : MonoBehaviour {

    private void OnCollisionEnter(Collision collision)
    {
        //If the ball collider the bottom, it means paddle miss it.
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ball"))
        {
            GameManager.Instance.Loose();
        }
    }
}
