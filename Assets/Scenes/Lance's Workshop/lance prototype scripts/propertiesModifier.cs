using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text.RegularExpressions;

public class propertiesModifier : MonoBehaviour
{
    [SerializeField] private bool Interactable = true;

    [Header("Objects")]
    [SerializeField] private InputField playerIDEText;

    [Header("Properties")]
    public string objectID;
    public int x;
    public int y;
    public int z;
    public int allAxisScale;

    public float speed;

    [Header("Code")]
    [Multiline(10)]
    public string code;

    [Header("Syntax Arrays")]
    public string[] line;
    public string[] syntax;

    void Update()
    {
        playerIDEText = (InputField)GameObject.FindWithTag("InputField").GetComponent<InputField>();

        code = playerIDEText.text;
        line = code.Split(new string[] { ";" }, System.StringSplitOptions.None);

        string result = code.Split('(', ')')[1];
        string[] another = result.Split(new String[] { "," }, StringSplitOptions.None);

        for (int argumentCount = 0; argumentCount < line.Length; argumentCount++)
        {
            syntax = line[argumentCount].Split('.');
        }

        if (result.Contains(","))
        {
            x = int.Parse(another[0]);
            y = int.Parse(another[1]);
            z = int.Parse(another[2]);
        }
        else
        {
            allAxisScale = int.Parse(result);

            x = allAxisScale;
            y = allAxisScale;
            z = allAxisScale;
        }

        if (code.Contains("transform.scale"))
        {
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Return))
            {
                gameObject.transform.localScale = new Vector3(x, y, z);
            }
        }
    }
}