using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestingGroundMonitor : MonoBehaviour {

	[Header("UI Elements"),SerializeField] Text timer;
	[SerializeField] Text totalQuestions;
	[SerializeField] Text correctAnswers;
	[SerializeField] Text botCount;

	[Header("HP"),SerializeField] GameObject content;
	[SerializeField] Image hpLevel;

	testHandler testHandler;

	private void Start()
	{
		
		testHandler = GameObject.Find("The Testing Ground").GetComponent<testHandler>();

		//display static values on start for optimality
		totalQuestions.text = testHandler.totalQuestions.ToString();
	}

	// Update is called once per frame
	void LateUpdate () {

		// initialize values
		GameObject[] aiCount = GameObject.FindGameObjectsWithTag("AI");
		GameObject[] currentHPDisplay = GameObject.FindGameObjectsWithTag("List Item");

		// display values to player canvas
		// display ONLY what needs to be monitored all the time
		botCount.text = aiCount.Length.ToString();
		correctAnswers.text = testHandler.playerScore.ToString();
		timer.text = testHandler.time.ToString("0.00");

	}

	public void updateHP(float currentHealth, float maxHealth){

		float calc_Health = currentHealth / maxHealth;

		hpLevel.fillAmount = calc_Health;

	}
}