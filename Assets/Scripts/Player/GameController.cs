using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
	public string gameStatus;

	// Use this for initialization
	void Start ()
	{
		Cursor.lockState = CursorLockMode.Locked;
		gameStatus = "playing";
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetMouseButtonDown(2))
		{
			if (gameStatus == "playing")
			{
				PauseGame();
				Cursor.lockState = CursorLockMode.None;
			}
			else
			{
				ContinueGame();
				Cursor.lockState = CursorLockMode.Locked;
			}
		}
	}

	private void PauseGame()
	{
		Time.timeScale = 0;
		gameStatus = "paused";
	}

	private void ContinueGame()
	{
		Time.timeScale = 1;
		gameStatus = "playing";
	}
}