using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginController : MonoBehaviour {

	[SerializeField] InputField passwordText;

	private void OnEnable()
	{
		passwordText.text = "";
	}

}
