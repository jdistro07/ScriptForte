using UnityEngine;
using System.Collections;

public class SettingsExample : MonoBehaviour {
	
	bool isShooting = false;
	bool isUsing = false;
	string username = "";
	
	IEnumerator Start(){
		//Check if the settings have been loaded 
		//( Since start is called after awake, and the settings are loaded on Awake, the settings should be loaded
		// assuming you have a settings Object in the scene )
		if(SettingsAndControls.IsLoaded()){
			//Set the local variable username to the value of "Player Name" in the settings
			username = SettingsAndControls.Settings.GetString("Player Name");
		}
		else{
			yield return new WaitForSeconds(0.1f);
		}
	}
	
	void Update(){
		//Check if the settings have been loaded
		if(SettingsAndControls.IsLoaded()){
			//You can either call controlls directly by their names
			isShooting = Controls.GetKey ("Shoot");
			isUsing = Controls.GetKey ("Use");
			
			//Or by retrieving their corresponding KeyCode
			isShooting = Controls.GetKey(SettingsAndControls.Controls.GetKeyCodeFor("Shoot"));
			isUsing = Controls.GetKey(SettingsAndControls.Controls.GetKeyCodeFor("Use"));
			
			
		}
	}
	
	
	void OnGUI(){


		//Here we convert a keycode to a string in order to generate a user friendly version
		GUI.Label (new Rect (10, 10, 300, 25), "Press '" +
						Controls.KeyCodeToString (SettingsAndControls.Controls.GetKeyCodeFor ("Shoot"))
						+ "' to shoot and '" + 
						Controls.KeyCodeToString (SettingsAndControls.Controls.GetKeyCodeFor ("Use"))
						+ "' to use.");


		if(isShooting){
			GUI.Label(new Rect(10, 45, 100, 25), "Shooting!");
		}
		
		if(isUsing){
			GUI.Label(new Rect(10, 80, 100, 25), "Using!");
		}
		
		if(GUI.Button(new Rect(10, 115, 300, 25), "Toggle 'shoot' between mouse buttons")){
			//Updates our current instance of "Shoot" command to "Mouse1"
			//(Alternately we can use this to add controls that are not listed as defaults in the Editor)
			SettingsAndControls.Controls.SetControl("Shoot", (SettingsAndControls.Controls.GetKeyCodeFor("Shoot") == KeyCode.Mouse0) ? KeyCode.Mouse1 : KeyCode.Mouse0);
			
			//Saves the modifications to the settings file so that the are there when the game is next loaded
			SettingsAndControls.Save();
		}
		
		GUI.Label (new Rect (10, 150, 100, 25), "Username: ");
		username = GUI.TextField (new Rect (120, 150, 300, 25), ""  + username);
		
		if (GUI.Button (new Rect (430, 150, 300, 25), "Save Username!")) {
			//Update the value for 'Player Name' in the settings to the current username
			//(Alternately we can use this to add settings that are not listed as defaults in the Editor)
			SettingsAndControls.Settings.SetSetting("Player Name", new SACString(username), Setting.SettingType.STRING);
			//Saves the modifications to the settings file so that they are there when the game is next loaded
			SettingsAndControls.Save ();
		}
		
		
	}
	
}