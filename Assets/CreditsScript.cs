using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditsScript : MonoBehaviour
{
	public Button btnBack;

	UIManager UIM;

	void Start()
	{
		UIM = GameObject.Find ("AIOGameManager").GetComponent<UIManager> ();

		btnBack.onClick.AddListener (() => {
			UIM.sfxClose();
			UIM.toMainUIFast();
		});
	}
}
