using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text.RegularExpressions;

public class objProperties : MonoBehaviour {

    public bool interactable = true;

    [Header("Objects")]
    public InputField playerIDEText;

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
    
	
	// Update is called once per frame
	void Update () {
        try
        {
            // Initialize  the required objects

            /* List of Requirements
             * 
             * Unique Object ID - To avoid manipulating other objects.
             * xyz values for calculations
             * 
             */
            playerIDEText = (InputField)GameObject.FindWithTag("InputField").GetComponent<InputField>();
            

            // Split by variable "code" by "\n" (for now MARKED TO BE FIXED)
            code = playerIDEText.text;
            line = code.Split('\n');

            // equal the amount of "argumentCount" with respect to the length of the "line" array
            for (int argumentCount = 0; argumentCount < line.Length; argumentCount = line.Length)
            {
                syntax = line[argumentCount].Split('.'); // split line "syntax" by "." from "line"
            }


            /* 
             * syntax filter for appropriate calculation
             * if true, return no errors and state the class on Debug <== Debug is optional
             * 
             */
            if(syntax[0] == "transform"){
            
                Debug.Log("Class: transform");

                try // catch Index Out of Range Exception
                {
                    if(syntax[1] == "scale"){
                        //x,y,z scaling calculations
                        Debug.Log("Property: scale");
                        ErrorChecker(false);

                        // test
                        allAxisScale = 5;
                        float scale = Mathf.Lerp(1, 5, Time.deltaTime/speed);
                        transform.localScale = new Vector3(scale,scale,scale);
                    }
                    else
                    {
                        // return error if no object property is found
                        ErrorChecker(true);
                    }
                }catch(IndexOutOfRangeException ioor){
                    Debug.Log(ioor);
                    ErrorChecker(true);
                }
            }
            else
            {
                // return error if no class is found
                ErrorChecker(true);
            }
        }catch(NullReferenceException nre){
            Debug.Log("Finding Input Fields: "+nre);
        }
	}

    public void ErrorChecker(bool error)
    {
        Text text = playerIDEText.transform.Find("Text").GetComponent<Text>();//initialize text object from player's inputfield

        //wait for 2 seconds before changing the color.
        if(error){
            text.color = Color.red;
        }
        else
        {
            text.color = Color.white;
        }
    }
}
