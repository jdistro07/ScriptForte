using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class videoPlayer_script : MonoBehaviour
{
	public string videoURL;

	public RawImage rawImage;
	public VideoPlayer videoPlayer;
	public AudioSource audioSource;

	private void Start()
	{
		StartCoroutine (playVideo ());
	}

	private IEnumerator playVideo()
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
	}
}
