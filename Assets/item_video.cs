using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class item_video : MonoBehaviour {

	public string videoTarget;

	GameSettingsManager gameSettings_Manager;

	[Header("UI References")]
	Transform videoPlayerPanel;
	Transform viewport;

	[Header("Video list item Manager UI elements")]
	[SerializeField] Button btn_Watch;

	// Use this for initialization
	void OnEnable () {

		gameSettings_Manager = GameObject.Find("AIOGameManager").GetComponent<GameSettingsManager>();
		videoPlayerPanel = GameObject.Find("VideoPanelLessons").transform.GetChild(3);
		viewport = GameObject.Find("VideoPanelLessons").transform.GetChild(1);
		videoPlayer_script videoPlayerScript = videoPlayerPanel.GetComponent<videoPlayer_script>();
		
		// set the link for the video
		// get video name (with file extension) to directly access the video from the remote host
		string videoName = gameObject.name;

		btn_Watch.onClick.AddListener(()=> {

			// disable viewport and enable video player
			videoPlayerPanel.gameObject.SetActive(true);
			viewport.gameObject.SetActive(false);

			// send link towards the video player script made by Lance
			videoPlayerScript.videoURL = "http://"+gameSettings_Manager.link+"/videos/"+videoTarget;

		});

	}

}
