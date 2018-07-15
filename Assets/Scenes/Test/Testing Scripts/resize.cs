using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class resize : MonoBehaviour {

    [Header("GUI")]
    public InputField xyz;
    public InputField x;
    public InputField y;
    public InputField z;

    private int xyzValue;
    private int xValue;
    private int yValue;
    private int zValue;

	// Update is called once per frame
	void Update () {

        try
        {
            xyzValue = Convert.ToInt32(xyz.text);
            transform.localScale += new Vector3(xyzValue, xyzValue, xyzValue);
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
	}
}
