using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class videoPlayer_script : MonoBehaviour
{
	public string videoURL;

	[SerializeField] private RawImage rawImage;
	[SerializeField] private VideoPlayer videoPlayer;
	[SerializeField] private AudioSource audioSource;
	[SerializeField] private Button playButton;
	[SerializeField] private Button stopButton;
	[SerializeField] private Slider seekBar;

	private float vidLength;
	private bool isDone = false;

	private void Start()
	{
		playButton.onClick.AddListener (playVideo);
		stopButton.onClick.AddListener (stopVideo);

		//StartCoroutine (playVideo ());
	}

	void Update()
	{
		if (videoPlayer.url != videoURL && isDone == false)
		{
			Debug.Log("Loading Video");
			videoPlayer.url = videoURL;
			videoPlayer.Prepare ();
		}
		isDone = true;

		if (videoPlayer.isPrepared)
		{
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
