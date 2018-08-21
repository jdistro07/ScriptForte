using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.PostProcessing;

public class GameSettingsManager : MonoBehaviour {

	[Header("Scripts")]
	public PostProcessingProfile ppProfile;

	[Header("Game Configuration State")]
	[SerializeField] string link;
	[SerializeField] bool bloom;
	[SerializeField] bool blur;

	private void LateUpdate()
	{
		link = SettingsAndControls.Settings.GetString("link");
		bloom = SettingsAndControls.Settings.GetBool("bloom");
		blur = SettingsAndControls.Settings.GetBool("blur");

		ppProfile.motionBlur.enabled = blur;
		ppProfile.bloom.enabled = bloom;
	}
}
