using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class resize : MonoBehaviour {

    Vector3 minScale;
    public Vector3 scale;
    public float speed;

    private void Update()
    {
        StartCoroutine(FreeScale());
        minScale = transform.localScale;
    }

    IEnumerator FreeScale()
    {
        yield return new WaitForSeconds(2);
        transform.localScale = Vector3.Lerp(minScale, scale, Time.deltaTime * speed);
    }
}
