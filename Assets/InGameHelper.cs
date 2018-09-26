using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameHelper : MonoBehaviour {

	AudioSource audioSource;
	
	public AudioClip intenseTheme;
	public AudioClip calmTheme;

	[Header("SFX")]
	public AudioClip crashSFX;

	// Use this for initialization
	void Awake () {
		
		audioSource = gameObject.GetComponent<AudioSource>();

	}

	private void Start()
	{
		
		playCalmTheme();
		
	}
	
	public void intenseMusicPlay(){

		audioSource.PlayOneShot(intenseTheme);
		audioSource.loop = true;

	}

	public void playCalmTheme(){

		audioSource.PlayOneShot(calmTheme);
		audioSource.loop = true;

	}

}
