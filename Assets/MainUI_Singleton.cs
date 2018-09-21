using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainUI_Singleton : MonoBehaviour {

	static MainUI_Singleton instance;
	[SerializeField] LoginModule loginModule;

	private void Awake()
	{

		if(instance != null){

			Destroy(gameObject);

		}else{

			instance = this;
			DontDestroyOnLoad(gameObject);

		}

	}

	private void OnEnable()
	{

		if(loginModule.LoggedIn){

			StartCoroutine(loginModule.musicPlay(loginModule.mainTheme));

		}

	}

}
