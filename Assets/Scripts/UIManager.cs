using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	[Header("Main UI")]
	public GameObject loginPanel;

	[Header("Universal UI")]
	public GameObject gameSettingsPanel;

	public void OpengameSettings(){
		loginPanel.active = false;
		gameSettingsPanel.active = true;
	}

	public void ClosegameSettings(){
		loginPanel.active = true;
		gameSettingsPanel.active = false;
	}
	
}
