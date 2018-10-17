using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class item_video : MonoBehaviour {

	public string videoTarget;
	public int testid;

	[SerializeField] Text txtVideoTitle;

	GameSettingsManager gameSettings_Manager;
	
	[SerializeField] AnimationClip itemVideoAnimation;

	[Header("UI References")]
	Transform videoPlayerPanel;
	Transform viewport;

	[Header("Video list item Manager UI elements")]
	[SerializeField] Button btn_Watch;
	[SerializeField] Text txtName;

	LoginModule lgm;

	private void Start()
	{
		StartCoroutine(videoHighlight());
	}

	// Use this for initialization
	void OnEnable () {

		lgm = GameObject.Find("AIOGameManager").GetComponent<LoginModule>();
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

	IEnumerator videoHighlight(){

		string link = "http://"+gameSettings_Manager.link+"/game_client/video_highlight.php";

		//check on database if the player has no POST TEST on the current test ID
		WWWForm wwwform = new WWWForm();

		wwwform.AddField("username", lgm.accountUsername);
		wwwform.AddField("user_ID", lgm.userID);
		wwwform.AddField("testID", testid);

		WWW www = new WWW(link, wwwform);

		yield return www;

		Debug.Log(www.text);

		if(www.isDone){
			
			Animator highlight = this.gameObject.GetComponent<Animator>();
			
			// change the title to an appropriate readable format for regular users
			txtVideoTitle.text = lgm.CredentialSeperator(txtVideoTitle.text, "LessonName=");

			// highlight video if the user played the pre-test for the very first time
			if(www.text == "Highlight"){

				highlight.SetTrigger("higlight");

			}else{

				highlight.ResetTrigger("higlight");

			}
		}

	}

}
