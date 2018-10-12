using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPanel : MonoBehaviour
{
	[Header("Buttons")]
	public Button btnBack;
	public Button btnMove;
	public Button btnSprint;
	public Button btnJump;

	[Header("Panels")]
	public GameObject imgMove;
	public GameObject imgSprint;
	public GameObject imgJump;
	private GameObject[] panels;

	UIManager UIM;

	void Start()
	{
		UIM = GameObject.Find ("AIOGameManager").GetComponent<UIManager> ();

		Transform Content = transform.Find ("Scroll View").Find ("Viewport").Find ("Content");
		panels = new GameObject[Content.childCount];

		for (int x = 0; x < Content.childCount; x++)
		{
			panels [x] = Content.GetChild (x).gameObject;
		}

		foreach (GameObject panel in panels)
		{
			panel.SetActive (false);
		}

		btnBack.onClick.AddListener (() => {
			UIM.sfxClose ();
			UIM.toMainUIFast ();
		});

		btnMove.onClick.AddListener (() => {
			UIM.sfxSpecial ();
			enablePanel (imgMove);
		});

		btnSprint.onClick.AddListener (() => {
			UIM.sfxSpecial ();
			enablePanel (imgSprint);
		});

		btnJump.onClick.AddListener (() => {
			UIM.sfxSpecial ();
			enablePanel (imgJump);
		});
	}

	void enablePanel(GameObject panelName)
	{
		foreach (GameObject panel in panels)
		{
			panel.SetActive (false);
		}

		panelName.gameObject.SetActive (true);
	}
}
