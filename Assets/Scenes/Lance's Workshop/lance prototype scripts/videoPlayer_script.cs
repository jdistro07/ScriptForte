using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class videoPlayer_script : MonoBehaviour
{
	public string videoURL;

	private RawImage rawImage;
	private VideoPlayer videoPlayer;
	private AudioSource audioSource;

	[SerializeField] private Button playButton;
	[SerializeField] private Button stopButton;
	[SerializeField] private Slider seekBar;
	[SerializeField] Text statusText;

	[Header("UI References")]
	[SerializeField] GameObject viewPort;

	private float vidLength;
	private bool isDone = false;

	private void Awake()
	{
		
		// get the components in this same gameobject
		audioSource = gameObject.GetComponent<AudioSource>();
		rawImage = gameObject.GetComponent<RawImage>();
		videoPlayer = gameObject.GetComponent<VideoPlayer>();

	}

	private void Start()
	{

		playButton.onClick.AddListener (playVideo);
		stopButton.onClick.AddListener (()=>{
			
			stopVideo();
			closePlayer();
			
			});

		//StartCoroutine (playVideo ());
	}

	void Update()
	{
		if (videoPlayer.url != videoURL && isDone == false)
		{
			statusText.text = "Loading...";
			Debug.Log("Loading video from server");

			videoPlayer.url = videoURL;
			videoPlayer.Prepare ();
		}
		isDone = true;

		if (videoPlayer.isPrepared)
		{
			statusText.gameObject.SetActive(false);

			vidLength = (float)videoPlayer.frameCount / (float)videoPlayer.frameRate;
			seekBar.minValue = 0;
			seekBar.maxValue = vidLength;
		}

		seekBar.value = (float)videoPlayer.time;
	}

	void playVideo()
	{
		if (!videoPlayer.isPrepared)
		{
			statusText.text = "Loading...";
			Debug.Log ("Video is still preparing");
		}
		else
		{
			if (!videoPlayer.isPlaying)
			{
				rawImage.texture = videoPlayer.texture;
				videoPlayer.Play ();
				audioSource.Play ();
			}
			else if (videoPlayer.isPlaying)
			{
				rawImage.texture = videoPlayer.texture;
				videoPlayer.Pause ();
				audioSource.Pause ();
			}
		}
	}

	void stopVideo()
	{
		if (!videoPlayer.isPlaying)
			return;
		videoPlayer.Stop ();
		videoPlayer.Prepare ();
	}

	void closePlayer(){

		// enable viewport and disable the player from player view
		gameObject.SetActive(false);
		viewPort.SetActive(true);

	}

	/*private IEnumerator playVideo()
	{
		videoPlayer.Prepare ();
		while (!videoPlayer.isPrepared)
		{
			yield return new WaitForSeconds (2);
			break;
		}
		rawImage.texture = videoPlayer.texture;
		videoPlayer.Play ();
		audioSource.Play ();
	}*/
}
