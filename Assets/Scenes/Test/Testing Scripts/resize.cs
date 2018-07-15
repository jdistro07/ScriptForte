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

    [Header("Transform")]
    public float speed = 5f;

    private int xyzValueSize;
    private int xValue;
    private int yValue;
    private int zValue;

	// Update is called once per frame
	void Update () {

        try
        {
            xyzValueSize = Convert.ToInt32(xyz.text);

            var size = xyzValueSize / 100;
            Debug.Log(size);
            
            
        }
        catch (Exception ex)
        {
            //Debug.Log(ex);
        }
	}
}
