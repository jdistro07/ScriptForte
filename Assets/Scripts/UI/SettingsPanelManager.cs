using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanelManager : MonoBehaviour {

	[Header("Components")]
	public InputField InputLink;
	public Toggle Toggle_Bloom;
	public Toggle Toggle_MotionBlur;

	[Header("Debug Private Variables")]
	[SerializeField] string link;
	[SerializeField] bool bloom;
	[SerializeField] bool motionblur;

	private void OnEnable()
	{
		if(SettingsAndControls.IsLoaded()){
			InputLink.text = SettingsAndControls.Settings.GetString("link");
			Toggle_Bloom.isOn = SettingsAndControls.Settings.GetBool("bloom");
			Toggle_MotionBlur.isOn = SettingsAndControls.Settings.GetBool("blur");
		}
	}

	public void ApplySettings(){
		link = InputLink.text;
		bloom = Toggle_Bloom.isOn;
		motionblur = Toggle_MotionBlur.isOn;

		SettingsAndControls.Settings.SetSetting("link", new SACString(link), Setting.SettingType.STRING);
		SettingsAndControls.Settings.SetSetting("bloom", new SACBool(bloom), Setting.SettingType.BOOLEAN);
		SettingsAndControls.Settings.SetSetting("blur", new SACBool(motionblur), Setting.SettingType.BOOLEAN);
		SettingsAndControls.Save();
	}

}
