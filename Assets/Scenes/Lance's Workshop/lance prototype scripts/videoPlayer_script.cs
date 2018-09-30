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

	[SerializeField] Sprite sprite_pause;
	[SerializeField] Sprite sprite_play;

	[SerializeField] private Image icn_playButton;
	[SerializeField] private Button playButton;
	[SerializeField] private Button stopButton;
	[SerializeField] private Button fullscreenButton;

	//[SerializeField] private Slider seekBar;
	[SerializeField] private Slider slider_volume;
	[SerializeField] Text statusText;

	[Header("UI References")]
	[SerializeField] GameObject viewPort;

	public float vidLength;
	private float playTime;
	private bool isSet = false;
	private bool donePreparing = false;
	bool isFullscreen = false;

	//Video rawImage canvas settings
	private Vector2 originalPosition;
	private float originalWidth;
	private float originalHeight;

	private void Start()
	{
		playButton.onClick.AddListener (playVideo);

		stopButton.onClick.AddListener (()=>{

			stopVideo();
			closePlayer();

		});

		fullscreenButton.onClick.AddListener(fullscreen);
	}

	private void Awake()
	{
		// get the components in this same gameobject
		audioSource = gameObject.GetComponent<AudioSource>();
		rawImage = gameObject.GetComponent<RawImage>();
		videoPlayer = gameObject.GetComponent<VideoPlayer>();

		originalPosition = new Vector2 (transform.position.x, transform.position.y);
		originalWidth = rawImage.rectTransform.rect.width;
		originalHeight = rawImage.rectTransform.rect.height;
	}
		
	private void OnEnable()
	{
		slider_volume.value = audioSource.volume;
	}

	void Update()
	{
		if (videoPlayer.url != videoURL && isSet == false)
		{
			statusText.text = "Loading...";
			Debug.Log("Loading video from server");

			videoPlayer.url = videoURL;

			videoPlayer.Prepare ();
			donePreparing = false;
			isSet = true;
		}

		if (videoPlayer.url != videoURL)
		{
			isSet = false;
		}

		if (!videoPlayer.isPrepared)
		{
			statusText.gameObject.SetActive(true);
			videoPlayer.Prepare ();
		}

		if (videoPlayer.isPrepared && donePreparing == false)
		{
			statusText.gameObject.SetActive(false);

			vidLength = (float)videoPlayer.frameCount / (float)videoPlayer.frameRate;
			donePreparing = true;

			//Automatically play the video after preparing
			playVideo ();
		}

		audioSource.volume = slider_volume.value;
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
				
				icn_playButton.sprite = sprite_pause;
				playButton.image.color = new Color(0, 255, 118, 255);
			}
			else if (videoPlayer.isPlaying)
			{
				rawImage.texture = videoPlayer.texture;
				videoPlayer.Pause ();
				audioSource.Pause ();

				icn_playButton.sprite = sprite_play;
				playButton.image.color = new Color(199, 255, 255, 255);
			}
		}
	}

	void stopVideo()
	{
		videoPlayer.Stop ();
		donePreparing = false;
	}

	void closePlayer(){

		// enable viewport and disable the player from player view
		gameObject.SetActive(false);
		viewPort.SetActive(true);

	}

	void fullscreen()
	{

		if(!isFullscreen)
		{
			transform.position = new Vector2(Screen.width / 2, Screen.height/ 2);
			rawImage.rectTransform.sizeDelta = new Vector2 (Screen.width, Screen.height);

			isFullscreen = true;

			Debug.Log("Fullscreen");

		}
		else
		{
			transform.position = originalPosition;
			rawImage.rectTransform.sizeDelta = new Vector2 (originalWidth, originalHeight);

			isFullscreen = false;

			Debug.Log("Not fullscreen");
		}
	}
}
