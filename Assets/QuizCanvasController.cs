using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizCanvasController : MonoBehaviour {

	Animator canvasAnimator;

	private void Start()
	{
		canvasAnimator = this.GetComponent<Animator>();
	}

	private void OnDisable()
	{
		canvasAnimator.SetInteger("open", 1);
	}
}
