using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogWindow : MonoBehaviour {

    public Transform Text;
    public Image Charactor;

    public float shakeRange = 10f; // shake range change be changed from inspector,
                                   //keep it mind that max it can go is half in either direction

    private bool IsCancel = false;

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    [ContextMenu("SetLeft")]
    public void SetLeftHand()
    {
        Text.localRotation = Quaternion.AngleAxis(180, Vector3.up);
        transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
    }

    [ContextMenu("SetRight")]
    public void SetRightHand()
    {
        Text.localRotation = Quaternion.identity;
        transform.rotation = Quaternion.identity;
    }

    public void UserInput()
    {
        if (!IsCancel)
            IsCancel = true;
    }

    public void ShakeWindow(float seconds)
    {
        IsCancel = false;
        StartCoroutine(ShakeCoroutine(seconds));
    }

    IEnumerator ShakeCoroutine(float seconds)
    {
        float elapsed = 0.0f;
        Quaternion originalRotation = transform.localRotation;

        while (elapsed < seconds)
        {
            elapsed += Time.deltaTime;
            float z = Random.value * shakeRange - (shakeRange / 2);
            transform.eulerAngles = new 
                Vector3(originalRotation.x, originalRotation.y, originalRotation.z + z);
            yield return null;

            if (IsCancel) break;
        }

        transform.localRotation = originalRotation;
    }
}
