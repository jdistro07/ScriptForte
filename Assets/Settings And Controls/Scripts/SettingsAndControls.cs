using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

/// <summary>
/// <para>Settings and Controls Class</para>
/// <para>&#160;</para><br />
/// <para>Written by Nathan Fiscaletti (Fisc510)</para>
/// <para>(c) 2014 Redruin Studios, Inc. </para>
/// </summary>

public class SettingsAndControls : MonoBehaviour {
	
	private static SettingsAndControls current;
	
	private bool loaded = false;

	[SerializeField]
	public ControlKey[] DefaultInputs = new ControlKey[] {};

	[SerializeField]
	public Setting[] DefaultCustomSettings = new Setting[] {};

	[SerializeField]
	public string FileName = "MainSettings.dat";

	public Settings CurrentSettings;

	public Controls CurrentControls;

	private Controls lastControls;
	private Settings lastSettings;

	public static Controls LastLoadedControls {
		get { return SettingsAndControls.current.lastControls; }
	}

	public static Settings LastLoadedSettings {
		get { return SettingsAndControls.current.lastSettings; }
	}

	public static Controls Controls {
		get { return SettingsAndControls.current.CurrentControls; }
	}

	public static Settings Settings {
		get { return SettingsAndControls.current.CurrentSettings; }
	}


	private static SACEventHandler[] eventHandlers = new SACEventHandler[0];

	public static SACEventHandler[] EventHandlers {
		get { return eventHandlers; }
	}
	
	void Awake(){
		if(current == null){
			DontDestroyOnLoad(gameObject);
			current = this;
			Load ();
			Save ();
		}else if(current != this){
			Destroy(gameObject);
		}
	}

	/// <summary>
	/// Removes the settings file from system.
	/// </summary>
	public static void RemoveSettingsFileFromSystem(){
		if(File.Exists(Application.persistentDataPath + "/" + SettingsAndControls.current.FileName)){
			File.Delete(Application.persistentDataPath + "/" + SettingsAndControls.current.FileName);
		}
	}

	/// <summary>
	/// Resets all settings to defaults
	/// </summary>
	/// <param name="saveImmidiately">If set to true, The settings will automatically save after reseting</param>
	
	public static void SetToDefault(bool saveImmidiately){
		SettingsAndControls.current.CurrentSettings = new Settings (SettingsAndControls.current.DefaultCustomSettings);
		SettingsAndControls.current.CurrentControls = new Controls (SettingsAndControls.current.DefaultInputs);
		
		if(saveImmidiately){
			SettingsAndControls.Save();
		}
	}

	/// <summary>
	/// Loads the settings and controls from the disck into the system
	/// </summary>
	public static void Load(){
		SettingsAndControls.current.LoadPrivate ();
	}

	/// <summary>
	/// Saves the current settings and controls to the disk
	/// <returns>True if saved properly, else False. (Will return false if canceled by event handlers)</returns>
	/// </summary>
	public static bool Save(){
		return SettingsAndControls.current.SavePrivate ();
	}

	void LoadPrivate(){
		
		SettingsFile SettingsFile;
		
		if(File.Exists(Application.persistentDataPath + "/" + SettingsAndControls.current.FileName)){
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/" + SettingsAndControls.current.FileName, FileMode.Open);
			System.Object deser = bf.Deserialize(file);
			if(deser is SettingsFile){
				SettingsFile = (SettingsFile)deser;
				CurrentControls = SettingsFile.Controls;
				CurrentSettings = SettingsFile.Settings;
				lastControls = SettingsFile.Controls;
				lastSettings = SettingsFile.Settings;
				file.Close();
			}else{
				File.Delete(Application.persistentDataPath + "/" + SettingsAndControls.current.FileName);
				SettingsFile = new SettingsFile(this.DefaultInputs, this.DefaultCustomSettings);
				CurrentControls = SettingsFile.Controls;
				CurrentSettings = SettingsFile.Settings;
				lastControls = SettingsFile.Controls;
				lastSettings = SettingsFile.Settings;
			}
		}else{
			SettingsFile = new SettingsFile(this.DefaultInputs, this.DefaultCustomSettings);
			CurrentControls = SettingsFile.Controls;
			CurrentSettings = SettingsFile.Settings;
			lastControls = SettingsFile.Controls;
			lastSettings = SettingsFile.Settings;
		}
		loaded = true;
	}

	bool SavePrivate(){
		Controls saveControls = CurrentControls;
		Settings saveSettings = CurrentSettings;

		foreach(SACEventHandler handler in SettingsAndControls.EventHandlers){
			SACEventHandler.SACSaveEvent e = handler.OnSave(new SACEventHandler.SACSaveEvent(CurrentSettings, CurrentControls));
			saveControls = e.UpdatedControls;
			saveSettings = e.UpdatedSettings;
			if(e.IsCanceled()){
				return false;
			}
		}
		RemoveSettingsFileFromSystem ();
		try{
			SettingsFile SettingsFile = new SettingsFile (saveControls.GetAllControls (), saveSettings.GetAllSettings ());
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Create(Application.persistentDataPath + "/" + SettingsAndControls.current.FileName);
			bf.Serialize (file, SettingsFile);
			file.Close ();
			lastControls = SettingsFile.Controls;
			lastSettings = SettingsFile.Settings;
			return true;
		}catch(IOException e){
			Debug.LogWarning (e.Message);
			return false;
		}

	}
	
	/// <summary>
	/// Check if the settings have been loaded
	/// </summary>
	/// <returns><c>true</c> if settings are loaded; otherwise, <c>false</c>.</returns>
	public static bool IsLoaded(){
		return SettingsAndControls.current.loaded;
	}


	/// <summary>
	/// Adds an event handler.
	/// </summary>
	/// <param name="handler">The Event Handler</param>
	public static void AddEventHandler(SACEventHandler handler){
		SACEventHandler[] n = new SACEventHandler[eventHandlers.Length + 1];
		int i = 0;
		foreach (SACEventHandler h in eventHandlers) {
			n[i] = h;
			i++;
		}
		n [i] = handler;
		eventHandlers = n;
	}

}


[Serializable]
public class SettingsFile {
	public Controls Controls;
	public Settings Settings;
	public SettingsFile(ControlKey[] inputs, Setting[] setts){
		Controls = new Controls (inputs);
		Settings = new Settings (setts);
	}
}

[Serializable]
public class Setting {
	[Serializable]
	public enum SettingType {
		STRING,
		INTEGER,
		FLOAT,
		BOOLEAN,
		COLOR,
		VECTOR2,
		VECTOR3,
		VECTOR4,
		QUATERNION,
		TIME
	}

	[SerializeField]
	public string Name;
	[SerializeField]
	public SettingType Type;

	public SACString _VAL_String = new SACString("Setting Value");
	public SACInt _VAL_Int = new SACInt(0);
	public SACFloat _VAL_Float = new SACFloat(0.0f);
	public SACBool _VAL_Bool = new SACBool(true);
	public SACColor _VAL_Color = new SACColor(Color.white.r, Color.white.g, Color.white.b);
	public SACVector2 _VAL_Vector2 = new SACVector2(Vector2.zero);
	public SACVector3 _VAL_Vector3 = new SACVector3(Vector3.zero);
	public SACVector4 _VAL_Vector4 = new SACVector4(Vector4.zero);
	public SACQuaternion _VAL_Quaternion = new SACQuaternion(Quaternion.identity);
	public SACDate _VAL_DateTime = new SACDate(DateTime.Now);
	
	public Setting(string name, SACObject value, SettingType type){
		this.Name = name;
		switch(type){
		case SettingType.BOOLEAN : {
			this._VAL_Bool = (SACBool)value;
			break;
		}
		case SettingType.COLOR : {
				this._VAL_Color = (SACColor)value;
			break;
		}
		case SettingType.FLOAT : {
			this._VAL_Float = (SACFloat)value;
			break;
		}
		case SettingType.INTEGER : {
			this._VAL_Int = (SACInt)value;
			break;
		}
		case SettingType.QUATERNION : {
			this._VAL_Quaternion = (SACQuaternion)value;
			break;
		}
		case SettingType.STRING : {
			this._VAL_String = (SACString)value;
			break;
		}
		case SettingType.TIME : {
			this._VAL_DateTime = (SACDate)value;
			break;
		}
		case SettingType.VECTOR2 : {
			this._VAL_Vector2 = (SACVector2)value;
			break;
		}
		case SettingType.VECTOR3 : {
			this._VAL_Vector3 = (SACVector3)value;
			break;
		}
		case SettingType.VECTOR4 : {
			this._VAL_Vector4 = (SACVector4)value;
			break;
		}

		default : {
			break;
		}

		}
		this.Type = type;
	}
}

[Serializable]
public class ControlKey {
	public string Name = "";
	public KeyCode keyCode = KeyCode.None;
	
	public ControlKey(string Name, KeyCode key){
		this.Name = Name;
		this.keyCode = key;
	}
	
	public ControlKey(){}
}

[Serializable]
public class Settings {
	public System.Collections.Generic.List<string> names;
	public System.Collections.Generic.List<string> settings;
	public System.Collections.Generic.List<Setting.SettingType> types;
	
	Setting[] settingsL;
	
	public Settings(Setting[] settingsL){
		
		names = new System.Collections.Generic.List<string>();
		settings = new System.Collections.Generic.List<string>();
		types = new System.Collections.Generic.List<Setting.SettingType> ();
		
		this.settingsL = settingsL;
		if(this.settingsL != null){
			foreach(Setting s in this.settingsL){
				names.Add(s.Name);
				switch(s.Type){
				case Setting.SettingType.BOOLEAN : {
					settings.Add(s._VAL_Bool.ToString());
					break;
				}
				case Setting.SettingType.COLOR : {
					settings.Add(s._VAL_Color.ToString());
					break;
				}
				case Setting.SettingType.FLOAT : {
					settings.Add(s._VAL_Float.ToString());
					break;
				}
				case Setting.SettingType.INTEGER : {
					settings.Add(s._VAL_Int.ToString());
					break;
				}
				case Setting.SettingType.QUATERNION : {
					settings.Add(s._VAL_Quaternion.ToString());
					break;
				}
				case Setting.SettingType.STRING : {
					settings.Add(s._VAL_String.ToString());
					break;
				}
				case Setting.SettingType.TIME : {
					settings.Add(s._VAL_DateTime.ToString());
					break;
				}
				case Setting.SettingType.VECTOR2 : {
					settings.Add(s._VAL_Vector2.ToString());
					break;
				}
				case Setting.SettingType.VECTOR3 : {
					settings.Add(s._VAL_Vector3.ToString());
					break;
				}
				case Setting.SettingType.VECTOR4 : {
					settings.Add(s._VAL_Vector4.ToString());
					break;
				}
					
				default : {
					break;
				}
					
				}
				types.Add (s.Type);
			}
		}
	}

	/// <summary>
	/// Gets a specific setting based on it's index
	/// </summary>
	/// <returns>The setting</returns>
	/// <param name="id">Index</param>
	public Setting GetSetting(int id){
		return this.GetAllSettings () [id];
	}
	

	
	/// <summary>
	/// Gets all of the current settings
	/// </summary>
	/// <returns>All current settings</returns>
	public Setting[] GetAllSettings(){
		Setting[] ret = new Setting[names.Count];
		int i = 0;
		foreach(string str in names){
			string val = settings[i];

			switch(types[i]){
			case Setting.SettingType.BOOLEAN : {
				ret[i] = new Setting(str, new SACBool(val) , types[i]);
				break;
			}
			case Setting.SettingType.COLOR : {
				ret[i] = new Setting(str, new SACColor(val) , types[i]);
				break;
			}
			case Setting.SettingType.FLOAT : {
				ret[i] = new Setting(str, new SACFloat(val) , types[i]);
				break;
			}
			case Setting.SettingType.INTEGER : {
				ret[i] = new Setting(str, new SACInt(val) , types[i]);
				break;
			}
			case Setting.SettingType.QUATERNION : {
				ret[i] = new Setting(str, new SACQuaternion(val) , types[i]);
				break;
			}
			case Setting.SettingType.STRING : {
				ret[i] = new Setting(str, new SACString(val) , types[i]);
				break;
			}
			case Setting.SettingType.TIME : {
				ret[i] = new Setting(str, new SACDate(val) , types[i]);
				break;
			}
			case Setting.SettingType.VECTOR2 : {
				ret[i] = new Setting(str, new SACVector2(val) , types[i]);
				break;
			}
			case Setting.SettingType.VECTOR3 : {
				ret[i] = new Setting(str, new SACVector3(val) , types[i]);
				break;
			}
			case Setting.SettingType.VECTOR4 : {
				ret[i] = new Setting(str, new SACVector4(val) , types[i]);
				break;
			}
				
			default : {
				break;
			}
			}


			i++;
		}
		return ret;
	}

	/// <summary>
	/// Returns a DateTime object based on the name
	/// </summary>
	/// <param name="name">Name.</param>
	public DateTime GetDate(string name){
		if(GetSettingValue(name) is SACDate){
			return ((SACDate)GetSettingValue(name)).ToDateTime();
		}

		return DateTime.Today;
	}

	/// <summary>
	/// Returns a boolean based on the name
	/// </summary>
	/// <param name="name">Name.</param>
	public bool GetBool(string name){
		if (GetSettingValue (name) is SACBool) {
			return ((SACBool)GetSettingValue(name)).GetBool();
		}

		return false;
	}

	/// <summary>
	/// Returns a float based on the name
	/// </summary>
	/// <param name="name">Name.</param>
	public float GetFloat(string name){
		if(GetSettingValue(name) is SACFloat){
			return ((SACFloat)GetSettingValue(name)).GetFloat();
		}
		
		return 0f;
	}

	/// <summary>
	/// Returns an int based on the name
	/// </summary>
	/// <returns>The int.</returns>
	/// <param name="name">Name.</param>
	public int GetInt(string name){
		if(GetSettingValue(name) is SACInt){
			return ((SACInt)GetSettingValue(name)).GetInt();
		}
		
		return 0;
	}

	/// <summary>
	/// Returns a string based on the name
	/// </summary>
	/// <returns>The string.</returns>
	/// <param name="name">Name.</param>
	public string GetString(string name){
		if(GetSettingValue(name) is SACString){
			return ((SACString)GetSettingValue(name)).GetString();
		}
		
		return null;
	}

	/// <summary>
	/// Returns a Color object based on the name
	/// </summary>
	/// <returns>The color.</returns>
	/// <param name="name">Name.</param>
	public Color GetColor(string name){
		if(GetSettingValue(name) is SACColor){
			return ((SACColor)GetSettingValue(name)).GetColor();
		}
		
		return Color.black;
	}

	/// <summary>
	/// Returns a Vector2 object based on the name
	/// </summary>
	/// <returns>The vector2.</returns>
	/// <param name="name">Name.</param>
	public Vector2 GetVector2(string name){
		if(GetSettingValue(name) is SACVector2){
			return ((SACVector2)GetSettingValue(name)).GetVector2();
		}
		
		return Vector2.zero;
	}

	/// <summary>
	/// Returns a Vector3 object based on the name
	/// </summary>
	/// <returns>The vector3.</returns>
	/// <param name="name">Name.</param>
	public Vector3 GetVector3(string name){
		if(GetSettingValue(name) is SACVector3){
			return ((SACVector3)GetSettingValue(name)).GetVector3();
		}

		return Vector3.zero;
	}

	/// <summary>
	/// Returns a Vector4 object based on the name
	/// </summary>
	/// <returns>The vector4.</returns>
	/// <param name="name">Name.</param>
	public Vector4 GetVector4(string name){
		if(GetSettingValue(name) is SACVector4){
			return ((SACVector4)GetSettingValue(name)).GetVector4();
		}
		
		return Vector4.zero;
	}

	/// <summary>
	/// Returns a Quaternion object based on the name
	/// </summary>
	/// <returns>The quaternion.</returns>
	/// <param name="name">Name.</param>
	public Quaternion GetQuaternion(string name){
		if(GetSettingValue(name) is SACQuaternion){
			return ((SACQuaternion)GetSettingValue(name)).GetQuaternion();
		}
		
		return Quaternion.identity;
	}

	/// <summary>
	/// Gets a specific setting's value based on it's name
	/// </summary>
	/// <returns>The setting.</returns>
	/// <param name="name">Name.</param>
	public SACObject GetSettingValue(string name){
		int i = 0;
		foreach(string s in names){
			if(s == name){
				switch(types[i]){
				case Setting.SettingType.BOOLEAN : {
					return new SACBool(settings[i]);
				}
				case Setting.SettingType.COLOR : {
					return new SACColor(settings[i]);
				}
				case Setting.SettingType.FLOAT : {
					return new SACFloat(settings[i]);
				}
				case Setting.SettingType.INTEGER : {
					return new SACInt(settings[i]);
				}
				case Setting.SettingType.QUATERNION : {
					return new SACQuaternion(settings[i]);
				}
				case Setting.SettingType.STRING : {
					return new SACString(settings[i]);
				}
				case Setting.SettingType.TIME : {
					return new SACDate(settings[i]);
				}
				case Setting.SettingType.VECTOR2 : {
					return new SACVector2(settings[i]);
				}
				case Setting.SettingType.VECTOR3 : {
					return new SACVector3(settings[i]);
				}
				case Setting.SettingType.VECTOR4 : {
					return new SACVector4(settings[i]);
				}
					
				default : {
					break;
				}
				}
			}
			i++;
		}
		return null;
	}
	
	/// <summary>
	/// Sets a specific settings value based on a key
	/// (If setting does not already exist, it will be created)
	/// </summary>
	/// <param name="name">The setting's name</param>
	/// <param name="val">The value to set</param>
	/// <returns>True if setting set, else false. (Will return false if canceled with EventHandler)</returns>
	public bool SetSetting(string name, SACObject val, Setting.SettingType type){
		if(GetSettingValue(name) != null){
			foreach(SACEventHandler handler in SettingsAndControls.EventHandlers){
				SACEventHandler.SACChangeSettingEvent e = handler.OnSettingChange(new SACEventHandler.SACChangeSettingEvent(SettingsAndControls.LastLoadedSettings, SettingsAndControls.Settings, SettingsAndControls.Settings.names.IndexOf(name)));
				if(e.IsCanceled()){
					return false;
				}
			}
		}else{
			foreach(SACEventHandler handler in SettingsAndControls.EventHandlers){
				SACEventHandler.SACAddSettingEvent e = handler.OnSettingAdd(new SACEventHandler.SACAddSettingEvent(new Setting(name, val, type)));
				if(e.IsCanceled()){
					return false;
				}
			}
		}
		int i = 0;
		bool d = false;
		foreach(string str in names){
			if(name == str){
				d = true;
				break;
			}
			i++;
		}
		
		if(d){
			switch(type){
			case Setting.SettingType.BOOLEAN : {
				settings[i] = ((SACBool)val).ToString();
				break;
			}
			case Setting.SettingType.COLOR : {
				settings[i] = ((SACColor)val).ToString();
				break;
			}
			case Setting.SettingType.FLOAT : {
				settings[i] = ((SACFloat)val).ToString();
				break;
			}
			case Setting.SettingType.INTEGER : {
				settings[i] = ((SACInt)val).ToString();
				break;
			}
			case Setting.SettingType.QUATERNION : {
				settings[i] = ((SACQuaternion)val).ToString();
				break;
			}
			case Setting.SettingType.STRING : {
				settings[i] = ((SACString)val).ToString();
				break;
			}
			case Setting.SettingType.TIME : {
				settings[i] = ((SACDate)val).ToString();
				break;
			}
			case Setting.SettingType.VECTOR2 : {
				settings[i] = ((SACVector2)val).ToString();
				break;
			}
			case Setting.SettingType.VECTOR3 : {
				settings[i] = ((SACVector3)val).ToString();
				break;
			}
			case Setting.SettingType.VECTOR4 : {
				settings[i] = ((SACVector4)val).ToString();
				break;
			}
				
			default : {
				break;
			}
			}
			types[i] = type;
		}else{
			names.Add(name);
			switch(type){
			case Setting.SettingType.BOOLEAN : {
				settings.Add (((SACBool)val).ToString());
				break;
			}
			case Setting.SettingType.COLOR : {
				settings.Add (((SACColor)val).ToString());
				break;
			}
			case Setting.SettingType.FLOAT : {
				settings.Add (((SACFloat)val).ToString());
				break;
			}
			case Setting.SettingType.INTEGER : {
				settings.Add (((SACInt)val).ToString());
				break;
			}
			case Setting.SettingType.QUATERNION : {
				settings.Add (((SACQuaternion)val).ToString());
				break;
			}
			case Setting.SettingType.STRING : {
				settings.Add (((SACString)val).ToString());
				break;
			}
			case Setting.SettingType.TIME : {
				settings.Add (((SACDate)val).ToString());
				break;
			}
			case Setting.SettingType.VECTOR2 : {
				settings.Add (((SACVector2)val).ToString());
				break;
			}
			case Setting.SettingType.VECTOR3 : {
				settings.Add (((SACVector3)val).ToString());
				break;
			}
			case Setting.SettingType.VECTOR4 : {
				settings.Add (((SACVector4)val).ToString());
				break;
			}
				
			default : {
				break;
			}
			};
			types.Add(type);
		}

		return true;
	}

	/// <summary>
	/// Removes a specific setting from the current settings
	/// </summary>
	/// <returns><c>true</c>, if setting was removed, <c>false</c> otherwise.</returns>
	/// <param name="name">Name.</param>
	public bool RemoveSetting(string name){
		Setting s = GetSetting (names.IndexOf (name));
		foreach(SACEventHandler eventHandler in SettingsAndControls.EventHandlers){
			SACEventHandler.SACRemoveSettingEvent e = eventHandler.OnSettingRemove(new SACEventHandler.SACRemoveSettingEvent(s));
			if(e.IsCanceled()){
				return false;
			}
		}

		settings.RemoveAt (names.IndexOf (name));
		types.RemoveAt (names.IndexOf (name));
		names.Remove (name);
		return true;
	}
}

[Serializable]
public class Controls {
	System.Collections.Generic.List<string> names = new System.Collections.Generic.List<string>();
	System.Collections.Generic.List<string> keys = new System.Collections.Generic.List<string>();
	
	ControlKey[] inputs;
	
	public Controls(ControlKey[] inputs){
		
		this.inputs = inputs;
		if(this.inputs != null){
			foreach(ControlKey k in this.inputs){
				names.Add(k.Name);
				keys.Add(KeyCodeToString(k.keyCode));
			}
		}
	}

	/// <summary>
	/// Gets a specific Control based on it's index
	/// </summary>
	/// <returns>The control.</returns>
	/// <param name="id">Index</param>
	public ControlKey GetControl(int id){
		return GetAllControls () [ id ];
	}
	
	/// <summary>
	/// Gets all controls currently loaded
	/// </summary>
	/// <returns>All controls currently loaded</returns>
	public ControlKey[] GetAllControls(){
		ControlKey[] ret = new ControlKey[names.Count];
		int i = 0;
		foreach(string str in names){
			ret[i] = new ControlKey(str, StringToKeyCode(keys[i]));
			i++;
		}
		return ret;
	}
	
	/// <summary>
	/// Gets a KeyCode based on a specific value
	/// </summary>
	/// <returns>The key code for the specific value</returns>
	/// <param name="name">The name of the Key</param>
	public KeyCode GetKeyCodeFor(string name){
		int i = 0;
		foreach(string s in names){
			if(s == name){
				return StringToKeyCode(keys[i]);
			}
			i++;
		}
		return KeyCode.None;
	}
	
	/// <summary>
	/// Sets a specific key based on a value
	/// </summary>
	/// <param name="name">The Key's name</param>
	/// <param name="key">The new value for the key</param>
	/// <returns>True if the setting hase been set, else false. (Will returned false if </returns>
	public bool SetControl(string name, KeyCode key){
		if(GetControl(names.IndexOf(name)) != null){
			foreach(SACEventHandler handler in SettingsAndControls.EventHandlers){
				SACEventHandler.SACChangeControlEvent e = handler.OnControlChange(new SACEventHandler.SACChangeControlEvent(SettingsAndControls.LastLoadedControls, SettingsAndControls.Controls, SettingsAndControls.Controls.names.IndexOf(name)));
				if(e.IsCanceled()){
					return false;
				}
			}
		}else{
			foreach(SACEventHandler handler in SettingsAndControls.EventHandlers){
				SACEventHandler.SACAddControlEvent e = handler.OnControlAdd(new SACEventHandler.SACAddControlEvent(new ControlKey(name, key)));
				if(e.IsCanceled()){
					return false;
				}
			}
		}

		int i = 0;
		bool d = false;
		foreach(string str in names){
			if(name == str){
				d = true;
				break;
			}
			i++;
		}
		
		if(d){
			keys[i] = KeyCodeToString(key);
		}else{
			names.Add(name);
			keys.Add(KeyCodeToString(key));
		}
		return true;
	}

	/// <summary>
	/// Removes a specific control based on it's name.
	/// </summary>
	/// <returns><c>true</c>, if control was removed, <c>false</c> otherwise. (Will return false if canceled by event handler)</returns>
	/// <param name="name">Name.</param>
	public bool RemoveControl(string name){
		ControlKey s = GetControl (names.IndexOf (name));
		foreach(SACEventHandler eventHandler in SettingsAndControls.EventHandlers){
			SACEventHandler.SACRemoveControlEvent e = eventHandler.OnControlRemove(new SACEventHandler.SACRemoveControlEvent(s));
			if(e.IsCanceled()){
				return false;
			}
		}
		
		names.Remove (name);
		keys.Remove (KeyCodeToString (s.keyCode));
		return true;
	}

	/// <summary>
	/// Converts a keycode to a string representation of it
	/// </summary>
	/// <returns>The code as a string.</returns>
	/// <param name="val">Value.</param>
	public static string KeyCodeToString(KeyCode val){
		return Enum.GetName(typeof(KeyCode), (int)val);
	}

	/// <summary>
	/// Converts string value to a keycode
	/// </summary>
	/// <returns>The string as a keycode.</returns>
	/// <param name="val">Value.</param>
	public static KeyCode StringToKeyCode(string val){
		return (KeyCode)System.Enum.Parse(typeof(KeyCode), val);
	}

	/// <summary>
	/// Checks if a key has been pressed
	/// </summary>
	/// <returns><c>true</c>, if key is down, <c>false</c> otherwise.</returns>
	/// <param name="_key">_key.</param>
	public static bool GetKeyDown(string _key){
		if(SettingsAndControls.Controls.names.Contains(_key)){
			if(SettingsAndControls.IsLoaded()){
				return Input.GetKeyDown (StringToKeyCode(SettingsAndControls.Controls.keys[SettingsAndControls.Controls.names.IndexOf(_key)]));
			}
		}
		
		return false;
	}

	/// <summary>
	/// Checks if the key is released
	/// </summary>
	/// <returns><c>true</c>, if key was released, <c>false</c> otherwise.</returns>
	/// <param name="_key">_key.</param>
	public static bool GetKeyUp(string _key){
		if(SettingsAndControls.Controls.names.Contains(_key)){
			if(SettingsAndControls.IsLoaded()){
				return Input.GetKeyUp (StringToKeyCode(SettingsAndControls.Controls.keys[SettingsAndControls.Controls.names.IndexOf(_key)]));
			}
		}
		
		return false;
	}

	/// <summary>
	/// Checks if the key is currently being held down
	/// </summary>
	/// <returns><c>true</c>, if key is being held, <c>false</c> otherwise.</returns>
	/// <param name="_key">_key.</param>
	public static bool GetKey(string _key){
		if(SettingsAndControls.Controls.names.Contains(_key)){
			if(SettingsAndControls.IsLoaded()){
				return Input.GetKey (StringToKeyCode(SettingsAndControls.Controls.keys[SettingsAndControls.Controls.names.IndexOf(_key)]));
			}
		}
		
		return false;
	}	

	/// <summary>
	/// Checks if the key is released
	/// </summary>
	/// <returns><c>true</c>, if key was released, <c>false</c> otherwise.</returns>
	/// <param name="_key">_key.</param>
	public static bool GetKeyDown(KeyCode _key){
		return Input.GetKeyDown (_key);
	}

	/// <summary>
	/// Checks if the key is released
	/// </summary>
	/// <returns><c>true</c>, if key was released, <c>false</c> otherwise.</returns>
	/// <param name="_key">_key.</param>
	public static bool GetKeyUp(KeyCode _key){
		return Input.GetKeyUp (_key);
	}

	/// <summary>
	/// Checks if the key is currently being held down
	/// </summary>
	/// <returns><c>true</c>, if key is being held, <c>false</c> otherwise.</returns>
	/// <param name="_key">_key.</param>
	public static bool GetKey(KeyCode _key){
		return Input.GetKey (_key);
	}

}

public abstract class SACEventHandler {

	public abstract SACSaveEvent OnSave (SACSaveEvent _event);
	public abstract SACChangeControlEvent OnControlChange (SACChangeControlEvent _event);
	public abstract SACChangeSettingEvent OnSettingChange (SACChangeSettingEvent _event);
	public abstract SACAddControlEvent OnControlAdd (SACAddControlEvent _event);
	public abstract SACAddSettingEvent OnSettingAdd (SACAddSettingEvent _event);
	public abstract SACRemoveControlEvent OnControlRemove (SACRemoveControlEvent _event);
	public abstract SACRemoveSettingEvent OnSettingRemove (SACRemoveSettingEvent _event);

	public class SACRemoveSettingEvent : SACEvent {
		private Setting settingBeingRemoved;
		
		public Setting RemovedSetting {
			get { return this.settingBeingRemoved; }
		}
		
		public SACRemoveSettingEvent(Setting removedSetting){
			this.settingBeingRemoved = removedSetting;
		}
	}

	public class SACRemoveControlEvent : SACEvent {
		private ControlKey controlBeingRemoved;

		public ControlKey RemovedControl {
			get { return this.controlBeingRemoved; }
		}

		public SACRemoveControlEvent(ControlKey removedKey){
			this.controlBeingRemoved = removedKey;
		}
	}

	public class SACAddSettingEvent : SACEvent {
		private Setting settingBeingAdded;

		public Setting AddedSetting {
			get { return this.settingBeingAdded; }
		}

		public SACAddSettingEvent (Setting SettingBeingAdded){
			this.settingBeingAdded = SettingBeingAdded;
		}
	}

	public class SACAddControlEvent : SACEvent {
		private ControlKey controlBeingAdded;

		public ControlKey AddedControl {
			get { return this.controlBeingAdded; }
		}

		public SACAddControlEvent(ControlKey KeyBeingAdded){
			this.controlBeingAdded = KeyBeingAdded;
		}
	}

	public class SACChangeControlEvent : SACEvent {
		ControlKey _new;
		ControlKey _old;

		public ControlKey NewKey {
			get { return this._new; }
			set { this._new = value; }
		}

		public ControlKey OldKey {
			get { return this._old; }
			set { this._old = value; }
		}

		public SACChangeControlEvent(Controls _old, Controls _new, int Changed){
			this._old = _old.GetControl(Changed);
			this._new = _new.GetControl(Changed);
		}
	}

	public class SACChangeSettingEvent : SACEvent {
		Setting _new;
		Setting _old;
		
		public Setting NewSetting {
			get { return this._new; }
			set { this._new = value; }
		}
		
		public Setting OldKey {
			get { return this._old; }
			set { this._old = value; }
		}
		
		public SACChangeSettingEvent(Settings _old, Settings _new, int Changed){
			this._old  = _old.GetSetting(Changed);
			this._new = _new.GetSetting(Changed);
		}

	}

	public class SACSaveEvent : SACEvent{
		public Settings UpdatedSettings;
		public Controls UpdatedControls;

		public SACSaveEvent(Settings CS, Controls C){
			this.UpdatedSettings = CS;
			this.UpdatedControls = C;
		}

	}

	public class SACEvent {
		private bool Canceled = false;

		/// <summary>
		/// Sets the event's canceled status
		/// </summary>
		public void SetCanceled(bool val){
			this.Canceled = val;
		}

		/// <summary>
		/// Determines whether this instance is canceled.
		/// </summary>
		/// <returns><c>true</c> if this instance is canceled; otherwise, <c>false</c>.</returns>
		public bool IsCanceled(){
			return this.Canceled;
		}
	}

}

[Serializable]
public class SACColor : SACObject{
	[SerializeField]
	float r,g,b,a;

	public SACColor(float r, float g, float b, float a = 1.0f){
		this.r = r;
		this.g = g;
		this.b = b;
		this.a = a;
	}

	public Color GetColor(){
		return new Color (r, g, b, a);
	}

	public override string ToString(){
		return "[col]:" + r + "," + g + "," + b + "," + a;
	}

	public SACColor(string fromSTRING){
		string[] s = fromSTRING.Split (':');
		string[] s2 = s[1].Split (',');
		r = float.Parse(s2 [0]);
		g = float.Parse(s2 [1]);
		b = float.Parse(s2 [2]);
		a = float.Parse(s2 [3]);
	}

}

[Serializable]
public class SACDate : SACObject {
	[SerializeField]
	public int hour, min, sec, month, day, year;
	
	public SACDate(DateTime d){
		hour = d.Hour;
		min = d.Minute;
		sec = d.Second;
		month = d.Month;
		day = d.Day;
		year = d.Year;
	}
	
	public DateTime ToDateTime(){
		return new DateTime (year, month, day, hour, min, sec);
	}

	public override string ToString(){
		return "[date]:" + hour + "," + min + "," + sec + "," + month + "," + day + "," + year;
	}

	public SACDate(string fromSTRING){
		string[] s = fromSTRING.Split (':');
		string[] s2 = s[1].Split (',');
		hour = int.Parse(s2 [0]);
		min = int.Parse(s2 [1]);
		sec = int.Parse(s2 [2]);
		month = int.Parse(s2 [3]);
		day = int.Parse(s2 [2]);
		year = int.Parse(s2 [3]);
	}
}

[Serializable]
public class SACObject { }

[Serializable]
public class SACString : SACObject {
	[SerializeField]
	string val = "";
	public SACString(string val, bool isConvert = false){
		if(isConvert){
			string[] s = val.Split (':');
			val = s[1];
		}else{
			this.val = val;
		}
	}

	public string GetString(){
		return this.val.Replace("[str]:", "");
	}

	public override string ToString(){
		return "[str]:" + val;
	}
}

[Serializable]
public class SACFloat : SACObject {
	[SerializeField]
	float val = 0.0f;
	public SACFloat(float val){
		this.val = val;
	}

	public float GetFloat(){
		return this.val;
	}

	public override string ToString(){
		return "[float]:" + val;
	}

	public SACFloat(string fromSTRING){
		string[] s = fromSTRING.Split (':');
		val = float.Parse(s[1]);
	}
}

[Serializable]
public class SACInt : SACObject {
	[SerializeField]
	int val = 0;
	public SACInt(int val){
		this.val = val;
	}

	public int GetInt(){
		return val;
	}

	public override string ToString(){
		return "[int]:" + val;
	}

	public SACInt(string fromSTRING){
		string[] s = fromSTRING.Split (':');
		val = int.Parse(s[1]);
	}
}

[Serializable]
public class SACBool : SACObject {
	[SerializeField]
	bool val = true;
	public SACBool(bool val){
		this.val = val;
	}

	public bool GetBool(){
		return this.val;
	}

	public override string ToString(){
		return "[bool]:" + ((val) ? "true" : "false");
	}

	public SACBool(string fromSTRING){
		string[] s = fromSTRING.Split (':');
		val = bool.Parse(s[1]);
	}
}

[Serializable]
public class SACVector2 : SACObject {
	[SerializeField]
	float x,y;

	public SACVector2(Vector2 val){
		this.x = val.x;
		this.y = val.y;
	}

	public Vector2 GetVector2(){
		return new Vector2 (x, y);
	}

	public override string ToString(){
		return "[v2]:" + x + "," + y;
	}

	public SACVector2(string fromSTRING){
		string[] s = fromSTRING.Split (':');
		string[] s2 = s[1].Split (',');
		x = float.Parse(s2 [0]);
		y = float.Parse(s2 [1]);
	}
}

[Serializable]
public class SACVector3 : SACObject {
	[SerializeField]
	float x,y,z;


	public SACVector3(Vector3 val){
		x = val.x;
		y = val.y;
		z = val.z;
	}
	
	public Vector3 GetVector3(){
		return new Vector3 (x, y, z);
	}

	public override string ToString(){
		return "[v3]:" + x + "," + y + "," + z;
	}

	public SACVector3(string fromSTRING){
		string[] s = fromSTRING.Split (':');
		string[] s2 = s[1].Split (',');
		x = float.Parse(s2 [0]);
		y = float.Parse(s2 [1]);
		z = float.Parse(s2 [2]);
	}
}

[Serializable]
public class SACVector4 : SACObject {
	[SerializeField]
	float x,y,z,w;
	public SACVector4(Vector4 val){
		x = val.x;
		y = val.y;
		z = val.z;
		w = val.w;
	}
	
	public Vector4 GetVector4(){
		return new Vector4 (x, y, z, w);
	}

	public override string ToString(){
		return "[v4]:" + x + "," + y + "," + z + "," + w;
	}

	public SACVector4(string fromSTRING){
		string[] s = fromSTRING.Split (':');
		string[] s2 = s[1].Split (',');
		x = float.Parse(s2 [0]);
		y = float.Parse(s2 [1]);
		z = float.Parse(s2 [2]);
		w = float.Parse(s2 [3]);
	}
}

[Serializable]
public class SACQuaternion : SACObject {
	[SerializeField]
	float x,y,z,w;
	public SACQuaternion(Quaternion val){
		x = val.x;
		y = val.y;
		z = val.z;
		w = val.w;
	}

	public Quaternion GetQuaternion(){
		return new Quaternion (x, y, z, w);
	}

	public override string ToString(){
		return "[quat]:" + x + "," + y + "," + z + "," + w;
	}

	public SACQuaternion(string fromSTRING){
		string[] s = fromSTRING.Split (':');
		string[] s2 = s[1].Split (',');
		x = float.Parse(s2 [0]);
		y = float.Parse(s2 [1]);
		z = float.Parse(s2 [2]);
		w = float.Parse(s2 [3]);
	}
}