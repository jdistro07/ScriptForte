using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestingGroundMonitor : MonoBehaviour {
	[Header("UI Elements"),SerializeField] Text timer;
	[SerializeField] Text totalQuestions;
	[SerializeField] Text correctAnswers;
	[SerializeField] Text botCount;

	[Header("Reference Objects for Game Over")]
	[SerializeField] Button btnRetry;
	[SerializeField] Button btnQuit;
	[SerializeField] GameObject mainCamera;
	[SerializeField] GameObject gameOverPanel;

	[Header("Pause Menu Screen")]
	[SerializeField] private Button btnResume;
	[SerializeField] private Button btnSettings;
	[SerializeField] private Button btnLeave;

	[Header("HP"),SerializeField] GameObject content;
	public Image hpLevel;

	testHandler testHandler;
	UIManager uiManager;
	DBContentProcessor dbProcessor;

	private void Start()
	{
		testHandler = GameObject.Find("The Testing Ground").GetComponent<testHandler>();
		uiManager = GameObject.Find("AIOGameManager").GetComponent<UIManager>();
		dbProcessor = GameObject.Find("AIOGameManager").GetComponent<DBContentProcessor>();

		//display static values on start for optimality
		totalQuestions.text = testHandler.totalQuestions.ToString();

		// add event handlers from the singleton Game Manager
		btnRetry.onClick.AddListener(()=>{

			uiManager.sfxOpen();
			uiManager.InGame_Retry();
			
		});

		btnQuit.onClick.AddListener(()=>{

			uiManager.sfxClose();
			uiManager.toMainUI();

		});

	}

	// Update is called once per frame
	void LateUpdate () {

		// initialize values
		GameObject[] aiCount = GameObject.FindGameObjectsWithTag("AI");

		// display values to player canvas
		// display ONLY what needs to be monitored all the time
		botCount.text = aiCount.Length.ToString();
		correctAnswers.text = testHandler.playerScore.ToString();
		timer.text = testHandler.time.ToString("0.00");


		/*trigger game over if:
		1. Camera gameobject is enabled AND...
		2. Player HP in testHandler is <= 0

		Disable Player canvas to highlight gameover panel to the player(optional)
		*/

		switch((int)testHandler.playerHealth)
		{
		case 5:
			hpLevel.color = Color.Lerp(Color.green, Color.yellow, 0f);
			break;
		case 4:
			hpLevel.color = Color.Lerp(Color.green, Color.yellow, 0.50f);
			break;
		case 3:
			hpLevel.color = Color.Lerp(Color.green, Color.yellow, 1f);
			break;
		case 2:
			hpLevel.color = Color.Lerp(Color.yellow, Color.red, 0.50f);
			break;
		case 1:
			hpLevel.color = Color.Lerp(Color.yellow, Color.red, 1f);
			break;
		default:
			hpLevel.color = Color.white;
			break;
		}

		if(mainCamera.activeSelf && testHandler.playerHealth <= 0){

			gameOverPanel.SetActive(true);
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;

		}

	}

	public void updateHP(float currentHealth, float maxHealth){

		float calc_Health = currentHealth / maxHealth;

		hpLevel.fillAmount = calc_Health;

	}
}
