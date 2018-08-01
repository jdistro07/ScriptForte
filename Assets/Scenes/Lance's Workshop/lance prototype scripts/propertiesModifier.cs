using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text.RegularExpressions;

public class propertiesModifier : MonoBehaviour
{
    private Vector3 position;
    private Quaternion rotation;
    public GameObject prefab;

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
    public float moveSpeed;

    [Header("Code")]
    [Multiline(10)]
    public string code;

    [Header("Syntax Arrays")]
    public string[] line;
    public string[] syntax;

     void Start()
    {
        position = gameObject.transform.position;
        rotation = gameObject.transform.localRotation;
    }

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

        if (Input.GetKey(KeyCode.F5))
        {
            /*
             * FOR FUTURE UPDATES:
             *  - Create a single line reader.
             *  - Change "code.Contains()" to "line.toString().Contains()" for individual line reading.
             * 
             */

            scale();
            move();
        }
    }

    void scale()
    {
        if (code.Contains("transform.scale"))
        {
            if (code.Contains("transform.scale.x"))
            {
                y = gameObject.transform.localScale.y;
                z = gameObject.transform.localScale.z;
            }
            else if (code.Contains("transform.scale.y"))
            {
                x = gameObject.transform.localScale.x;
                z = gameObject.transform.localScale.z;
            }
            else if (code.Contains("transform.scale.z"))
            {
                x = gameObject.transform.localScale.x;
                y = gameObject.transform.localScale.y;
            }

            StartCoroutine(scaleObject(speed));
        }
    }

    void move()
    {
        if (code.Contains("transform.translate"))
        {
            StartCoroutine(moveObject(moveSpeed));
        }
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

    IEnumerator moveObject(float time)
    {
        Vector3 currentPosition = gameObject.transform.localPosition;
        Vector3 targetPosition = new Vector3(x, y, z);
        Vector3 positionX = new Vector3(x, 0, 0);
        Vector3 positionY = new Vector3(0, y, 0);
        Vector3 positionZ = new Vector3(0, 0, z);

        float currentTime = 0.0f;

        do
        {
            if (code.Contains("transform.translate.x"))
            {
                targetPosition = currentPosition + positionX;
                gameObject.transform.localPosition = Vector3.Lerp(currentPosition, targetPosition, currentTime / time);
                currentTime += Time.deltaTime;
            }
            else if (code.Contains("transform.translate.y"))
            {
                targetPosition = currentPosition + positionY;
                gameObject.transform.localPosition = Vector3.Lerp(currentPosition, targetPosition, currentTime / time);
                currentTime += Time.deltaTime;
            }
            else if (code.Contains("transform.translate.z"))
            {
                targetPosition = currentPosition + positionZ;
                gameObject.transform.localPosition = Vector3.Lerp(currentPosition, targetPosition, currentTime / time);
                currentTime += Time.deltaTime;
            }

            yield return null;
        } while (currentTime <= time);
    }
    

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Player")
        {
            Debug.Log("Destroying Object");
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Player")
        {
            
            other.transform.parent = transform;
            Debug.Log("Player Attached as Child");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            if (code.Contains("transform.scale"))
            {
                other.transform.parent = null;
            }
            else
            {
                other.transform.parent = transform;
                Debug.Log("Player Attached as Child");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        other.transform.parent = null;
        Debug.Log("Player Detached as Child");
    }
}