using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextSplitter : MonoBehaviour
{
    private InputField field;

    private string input;

    void Start()
    {
        field = gameObject.GetComponent<InputField>();
    }

    // Update is called once per frame
    void Update ()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Return))
        {
            input = field.text.ToString();
            string[] splitString = input.Split(new string[] { ";" }, System.StringSplitOptions.None);

            int length = splitString.Length - 1;

            for (int count = 0; count < length; count++)
            {
                field.text = field.text + '\n' + splitString[count].Trim();
            }
        }
	}
}
