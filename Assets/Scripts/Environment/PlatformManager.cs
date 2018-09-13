using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlatformManager : MonoBehaviour {

	[Header("Timer")]
	[SerializeField] Text timerDisplay;
	[SerializeField] GameObject testPlatformHandler;

	private void Start()
	{
		testPlatformHandler = GameObject.Find("The Testing Ground");
	}
	
	// Update is called once per frame
	void LateUpdate () {
		timerDisplay.text = testPlatformHandler.GetComponent<testHandler>().time.ToString("F1");
	}
}
