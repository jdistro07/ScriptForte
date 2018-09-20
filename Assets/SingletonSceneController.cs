using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SingletonSceneController : MonoBehaviour {

	[SerializeField] GameObject mainUI;
	
	// Update is called once per frame
	void LateUpdate () {
		
		if(SceneManager.GetActiveScene().name == "Main UI"){

			mainUI.SetActive(true);

		}else{

			StartCoroutine(itemCleanUp());
			mainUI.SetActive(false);
		
		}

	}

	IEnumerator itemCleanUp(){

		var itemPanels = GameObject.FindGameObjectsWithTag("List Item");
		
		foreach(GameObject items in itemPanels){

			Destroy(items);
			yield return null;

		}

	}
}
