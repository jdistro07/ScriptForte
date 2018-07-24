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
    public float x;
    public float y;
    public float z;
    public float allAxisScale;

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
        line = code.Split('\n');

        string result = code.Split('(', ')')[1];
        string[] another = result.Split(new String[] { "," }, StringSplitOptions.None);

        for (int argumentCount = 0; argumentCount < line.Length; argumentCount++)
        {
            syntax = line[argumentCount].Split('.');
        }

        if (result.Contains(","))
        {
            x = float.Parse(another[0]);
            y = float.Parse(another[1]);
            z = float.Parse(another[2]);
        }
        else
        {
            allAxisScale = float.Parse(result);

            x = allAxisScale;
            y = allAxisScale;
            z = allAxisScale;
        }

        if (code.Contains("transform.scale"))
        {
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Return))
            {
                //gameObject.transform.localScale = new Vector3(x, y, z);

                StartCoroutine(scaleObject(speed));
            }
        }

        /*if (code.Contains("transform.scale.x"))
        {

        }*/
    }

    IEnumerator scaleObject(float time)
    {
        Vector3 currentScale = gameObject.transform.localScale;
        Vector3 newScale = new Vector3(x, y, z);

        float currentTime = 0.0f;

        do
        {
            gameObject.transform.localScale = Vector3.Lerp(currentScale, newScale, currentTime / time);
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= time);
    }
}