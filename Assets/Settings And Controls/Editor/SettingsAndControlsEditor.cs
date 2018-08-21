using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof(SettingsAndControls))]
public class SettingsAndControlsEditor : Editor {

	bool showDefaultInputs = false;
	bool showDefaultSettings = false;
	bool showFileName = false;
	bool showAddSetting = false;
	bool showAddControl = false;
	bool showErrorControl = false;
	bool showErrorSetting = false;

	string controlError = "";
	string settingError = "";

	string _ADD_S_name = "New Setting";
	Setting.SettingType _ADD_S_type = Setting.SettingType.STRING;
	Setting _ADD_S_Setting;

	ControlKey _ADD_C_Key;

	Texture2D _Logo;
	GUIStyle center = new GUIStyle();
	GUIStyle red = new GUIStyle();
	GUIStyle yellow = new GUIStyle();
	GUIStyle name = new GUIStyle();



	void Awake(){
		_ADD_S_Setting = new Setting (_ADD_S_name, new SACString("Setting Value"), _ADD_S_type);
		_ADD_C_Key = new ControlKey ("New Control", KeyCode.None);
	}

	void OnDestroy(){
		ResetTarget ();
	}

	void OnEnable()
	{
		_Logo = (Texture2D)Resources.Load("Logo",typeof(Texture2D));
		center.alignment = TextAnchor.MiddleCenter;
		red.normal.textColor = Color.red;
		red.alignment = TextAnchor.MiddleCenter;
		name.normal.textColor = new Color(.10f, .02f,.53f) ;
		name.alignment = TextAnchor.MiddleCenter;
		name.fontStyle = FontStyle.Bold;
		yellow.normal.textColor = new Color(.67f, .29f, .03f, 1f);
		yellow.alignment = TextAnchor.MiddleCenter;
		yellow.fontSize = 8;
	}

	public override void OnInspectorGUI(){
		SettingsAndControls sac = (SettingsAndControls)target;
		RectOffset defPadding = GUI.skin.box.padding;
		TextAnchor defAlign = GUI.skin.textField.alignment;
		TextAnchor defBoxAlign = GUI.skin.box.alignment;
		GUI.skin.box.padding = new RectOffset (5, 5, 5, 5);
		GUI.skin.box.alignment = TextAnchor.MiddleCenter;
		GUI.skin.textField.alignment = TextAnchor.MiddleCenter;
		serializedObject.Update ();
		int fs = center.fontSize;
		center.fontSize = 16;
		EditorGUILayout.BeginVertical ();
		GUILayout.Space (20);
		GUILayout.Box (_Logo, center);
		EditorGUILayout.LabelField ("Simple Settings And Controls", center);
		center.fontSize = fs;
		EditorGUILayout.LabelField ("By Nathan Fiscaletti (Fisc510)", center);
		EditorGUILayout.LabelField ("V1.8", center);
		GUILayout.Space (20);
		EditorGUILayout.EndVertical ();
		showDefaultInputs = EditorGUILayout.Foldout (showDefaultInputs, "Default Controls");
		if(showDefaultInputs){
			EditorGUILayout.BeginVertical();
			int i = 0;
			try{
			for(i = 0;i<sac.DefaultInputs.Length;i++){
				EditorGUILayout.BeginHorizontal("Box");
				EditorGUILayout.BeginVertical();
					EditorGUILayout.LabelField(sac.DefaultInputs[i].Name, name, GUILayout.Width(Screen.width * .32f));
					EditorGUILayout.TextField("Control", yellow, GUILayout.Width(Screen.width * .32f));
				EditorGUILayout.EndVertical();
				EditorGUILayout.BeginVertical();
					//EditorGUILayout.LabelField("Key Value", name, GUILayout.Width(Screen.width * .35f));
					sac.DefaultInputs[i].keyCode = (KeyCode)EditorGUILayout.EnumPopup(sac.DefaultInputs[i].keyCode);
				EditorGUILayout.EndVertical();
				bool delete = GUILayout.Button ("X");
				EditorGUILayout.EndHorizontal();
				if(delete){
					sac.DefaultInputs = RemoveAt(sac.DefaultInputs, i);
					break;
				}

			}
			}catch(System.Exception e){
				ResetTarget();
			}
			if(i == 0){
				EditorGUILayout.LabelField("No Default Controls Defined", red);
			}

			if(showAddControl){
				GUILayout.BeginVertical("Box", GUILayout.MinWidth(0), GUILayout.Width(Screen.width * .90f));
				if(showErrorControl){
					GUILayout.Label (controlError, red);
				}
				GUILayout.Label ("Add New Control", center);
				GUILayout.BeginHorizontal();
				GUILayout.Label ("Name", center, GUILayout.Width(Screen.width * .45f));
				GUILayout.Label ("Value", center, GUILayout.Width(Screen.width * .45f));
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal(center);
				_ADD_C_Key.Name = EditorGUILayout.TextField(_ADD_C_Key.Name, GUILayout.Width(Screen.width * .45f));
				_ADD_C_Key.keyCode = (KeyCode)EditorGUILayout.EnumPopup(_ADD_C_Key.keyCode);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				bool cancel = GUILayout.Button("Cancel");
				bool _add = GUILayout.Button("Add!");
				
				if(cancel){
					_ADD_C_Key = new ControlKey("New Control", KeyCode.None);
					showAddControl = false;
					showErrorControl = false;
				}
				
				if(_add){
					if(CheckControlAdd(sac.DefaultInputs)){
						controlError = "'" + _ADD_C_Key.Name + "' is already defined.";
						showErrorControl = true;
					}else{
						sac.DefaultInputs = AddTo (sac.DefaultInputs, _ADD_C_Key);
						_ADD_C_Key = new ControlKey("New Control", KeyCode.None);	
						showAddControl = false;
						showErrorControl = false;
					}
				}
				
				GUILayout.EndHorizontal();
				
				GUILayout.EndVertical();
			}

			GUILayout.BeginHorizontal();
			if(i > 0){
				bool removeAll = GUILayout.Button ("Remove All");
				if(removeAll){
					sac.DefaultInputs = new ControlKey[] {};
				}
			}
			if(!showAddControl){
				bool add = GUILayout.Button("Add New Control");
				if(add){
					showAddControl = true;
				}
			}
			GUILayout.EndHorizontal();

			EditorGUILayout.EndVertical();
		}



		showDefaultSettings = EditorGUILayout.Foldout (showDefaultSettings, "Default Settings");
		if(showDefaultSettings){
			EditorGUILayout.BeginHorizontal(center);
			EditorGUILayout.BeginVertical();

			int i = 0;
			try{
			for(i = 0;i<sac.DefaultCustomSettings.Length;i++){
				EditorGUILayout.BeginHorizontal("Box");
				EditorGUILayout.BeginHorizontal(center);
				EditorGUILayout.BeginVertical();
					EditorGUILayout.LabelField(sac.DefaultCustomSettings[i].Name, name, GUILayout.Width(Screen.width * .32f));
					EditorGUILayout.LabelField("(" + System.Enum.GetName(typeof(Setting.SettingType), (int)sac.DefaultCustomSettings[i].Type) + ")", yellow, GUILayout.Width(Screen.width * .32f));
				EditorGUILayout.EndVertical();
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal(center);
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal(center);

				switch(sac.DefaultCustomSettings[i].Type){
					case Setting.SettingType.STRING : {
						sac.DefaultCustomSettings[i]._VAL_String = new SACString(EditorGUILayout.TextField(sac.DefaultCustomSettings[i]._VAL_String.GetString()));
						break;
					}

					case Setting.SettingType.BOOLEAN : {
					sac.DefaultCustomSettings[i]._VAL_Bool = new SACBool(EditorGUILayout.Toggle(sac.DefaultCustomSettings[i]._VAL_Bool.GetBool()));
						break;
					}

					case Setting.SettingType.COLOR : {
						Color col = EditorGUILayout.ColorField(sac.DefaultCustomSettings[i]._VAL_Color.GetColor());
						sac.DefaultCustomSettings[i]._VAL_Color = new SACColor(col.r, col.g, col.b, col.a);
						break;
					}

					case Setting.SettingType.FLOAT : {
						sac.DefaultCustomSettings[i]._VAL_Float = new SACFloat(EditorGUILayout.FloatField(sac.DefaultCustomSettings[i]._VAL_Float.GetFloat()));
						break;	
					}

					case Setting.SettingType.INTEGER : {
						sac.DefaultCustomSettings[i]._VAL_Int = new SACInt(EditorGUILayout.IntField(sac.DefaultCustomSettings[i]._VAL_Int.GetInt()));
						break;
					}

					case Setting.SettingType.QUATERNION : {
						GUILayout.BeginVertical();
						EditorGUIUtility.labelWidth = 15;
						GUILayout.BeginHorizontal();
							sac.DefaultCustomSettings[i]._VAL_Quaternion = new SACQuaternion(new Quaternion(EditorGUILayout.FloatField("X", sac.DefaultCustomSettings[i]._VAL_Quaternion.GetQuaternion().x,GUILayout.MinWidth(0)), sac.DefaultCustomSettings[i]._VAL_Quaternion.GetQuaternion().y, sac.DefaultCustomSettings[i]._VAL_Quaternion.GetQuaternion().z, sac.DefaultCustomSettings[i]._VAL_Quaternion.GetQuaternion().w));
							sac.DefaultCustomSettings[i]._VAL_Quaternion = new SACQuaternion(new Quaternion(sac.DefaultCustomSettings[i]._VAL_Quaternion.GetQuaternion().x, EditorGUILayout.FloatField("Y", sac.DefaultCustomSettings[i]._VAL_Quaternion.GetQuaternion().y,GUILayout.MinWidth(0)), sac.DefaultCustomSettings[i]._VAL_Quaternion.GetQuaternion().z, sac.DefaultCustomSettings[i]._VAL_Quaternion.GetQuaternion().w));
						GUILayout.EndHorizontal();
						GUILayout.BeginHorizontal();
							sac.DefaultCustomSettings[i]._VAL_Quaternion = new SACQuaternion(new Quaternion(sac.DefaultCustomSettings[i]._VAL_Quaternion.GetQuaternion().x, sac.DefaultCustomSettings[i]._VAL_Quaternion.GetQuaternion().y, EditorGUILayout.FloatField("Z", sac.DefaultCustomSettings[i]._VAL_Quaternion.GetQuaternion().z,GUILayout.MinWidth(0)), sac.DefaultCustomSettings[i]._VAL_Quaternion.GetQuaternion().w));
							sac.DefaultCustomSettings[i]._VAL_Quaternion = new SACQuaternion(new Quaternion(sac.DefaultCustomSettings[i]._VAL_Quaternion.GetQuaternion().x, sac.DefaultCustomSettings[i]._VAL_Quaternion.GetQuaternion().y, sac.DefaultCustomSettings[i]._VAL_Quaternion.GetQuaternion().z, EditorGUILayout.FloatField("W", sac.DefaultCustomSettings[i]._VAL_Quaternion.GetQuaternion().w,GUILayout.MinWidth(0))));
						GUILayout.EndHorizontal();
						EditorGUIUtility.labelWidth = 0;
						GUILayout.EndVertical();

						break;
					}

					case Setting.SettingType.VECTOR2 : {
						GUILayout.BeginVertical();
						EditorGUIUtility.labelWidth = 15;
						GUILayout.BeginHorizontal();
							sac.DefaultCustomSettings[i]._VAL_Vector2 = new SACVector2(new Vector2(EditorGUILayout.FloatField("X", sac.DefaultCustomSettings[i]._VAL_Vector2.GetVector2().x,GUILayout.MinWidth(0)), sac.DefaultCustomSettings[i]._VAL_Vector2.GetVector2().y));
							sac.DefaultCustomSettings[i]._VAL_Vector2 = new SACVector2(new Vector2(sac.DefaultCustomSettings[i]._VAL_Vector2.GetVector2().x, EditorGUILayout.FloatField("Y", sac.DefaultCustomSettings[i]._VAL_Vector2.GetVector2().y,GUILayout.MinWidth(0))));
						GUILayout.EndHorizontal();
						EditorGUIUtility.labelWidth = 0;
						GUILayout.EndVertical();
						break;
					}

					case Setting.SettingType.VECTOR3 : {
						GUILayout.BeginVertical();
						EditorGUIUtility.labelWidth = 15;
						GUILayout.BeginHorizontal();
							sac.DefaultCustomSettings[i]._VAL_Vector3 = new SACVector3(new Vector3(EditorGUILayout.FloatField("X", sac.DefaultCustomSettings[i]._VAL_Vector3.GetVector3().x,GUILayout.MinWidth(0)), sac.DefaultCustomSettings[i]._VAL_Vector3.GetVector3().y, sac.DefaultCustomSettings[i]._VAL_Vector3.GetVector3().z));
							sac.DefaultCustomSettings[i]._VAL_Vector3 = new SACVector3(new Vector3(sac.DefaultCustomSettings[i]._VAL_Vector3.GetVector3().x, EditorGUILayout.FloatField("Y", sac.DefaultCustomSettings[i]._VAL_Vector3.GetVector3().y,GUILayout.MinWidth(0)), sac.DefaultCustomSettings[i]._VAL_Vector3.GetVector3().z));
							sac.DefaultCustomSettings[i]._VAL_Vector3 = new SACVector3(new Vector3(sac.DefaultCustomSettings[i]._VAL_Vector3.GetVector3().x, sac.DefaultCustomSettings[i]._VAL_Vector3.GetVector3().y, EditorGUILayout.FloatField("Z", sac.DefaultCustomSettings[i]._VAL_Vector3.GetVector3().z,GUILayout.MinWidth(0))));
						GUILayout.EndHorizontal();
						EditorGUIUtility.labelWidth = 0;
						GUILayout.EndVertical();
						break;
					}

					case Setting.SettingType.VECTOR4 : {
						GUILayout.BeginVertical();
						EditorGUIUtility.labelWidth = 15;
						GUILayout.BeginHorizontal();
							sac.DefaultCustomSettings[i]._VAL_Vector4 = new SACVector4(new Vector4(EditorGUILayout.FloatField("X", sac.DefaultCustomSettings[i]._VAL_Vector4.GetVector4().x,GUILayout.MinWidth(0)), sac.DefaultCustomSettings[i]._VAL_Vector4.GetVector4().y, sac.DefaultCustomSettings[i]._VAL_Vector4.GetVector4().z, sac.DefaultCustomSettings[i]._VAL_Vector4.GetVector4().w));
					sac.DefaultCustomSettings[i]._VAL_Vector4 = new SACVector4(new Vector4(sac.DefaultCustomSettings[i]._VAL_Vector4.GetVector4().x, EditorGUILayout.FloatField("Y", sac.DefaultCustomSettings[i]._VAL_Vector4.GetVector4().y,GUILayout.MinWidth(0)), sac.DefaultCustomSettings[i]._VAL_Vector4.GetVector4().z, sac.DefaultCustomSettings[i]._VAL_Vector4.GetVector4().w));
						GUILayout.EndHorizontal();
						GUILayout.BeginHorizontal();
							sac.DefaultCustomSettings[i]._VAL_Vector4 = new SACVector4(new Vector4(sac.DefaultCustomSettings[i]._VAL_Vector4.GetVector4().x, sac.DefaultCustomSettings[i]._VAL_Vector4.GetVector4().y, EditorGUILayout.FloatField("Z", sac.DefaultCustomSettings[i]._VAL_Vector4.GetVector4().z,GUILayout.MinWidth(0)), sac.DefaultCustomSettings[i]._VAL_Vector4.GetVector4().w));
							sac.DefaultCustomSettings[i]._VAL_Vector4 = new SACVector4(new Vector4(sac.DefaultCustomSettings[i]._VAL_Vector4.GetVector4().x, sac.DefaultCustomSettings[i]._VAL_Vector4.GetVector4().y, sac.DefaultCustomSettings[i]._VAL_Vector4.GetVector4().z, EditorGUILayout.FloatField("W", sac.DefaultCustomSettings[i]._VAL_Vector4.GetVector4().w,GUILayout.MinWidth(0))));
						GUILayout.EndHorizontal();
						EditorGUIUtility.labelWidth = 0;
						GUILayout.EndVertical();
						
						break;
					}

					case Setting.SettingType.TIME : {
						GUILayout.BeginVertical();
						EditorGUIUtility.labelWidth = 15;
						GUILayout.BeginHorizontal();
					sac.DefaultCustomSettings[i]._VAL_DateTime = new SACDate(new System.DateTime(sac.DefaultCustomSettings[i]._VAL_DateTime.year,
					                                                                             EditorGUILayout.IntField("M", sac.DefaultCustomSettings[i]._VAL_DateTime.month,GUILayout.MinWidth(0)),
					                                                                             sac.DefaultCustomSettings[i]._VAL_DateTime.day,
					                                                                             sac.DefaultCustomSettings[i]._VAL_DateTime.hour,
					                                                                             sac.DefaultCustomSettings[i]._VAL_DateTime.min,
					                                                                             sac.DefaultCustomSettings[i]._VAL_DateTime.sec));

					sac.DefaultCustomSettings[i]._VAL_DateTime = new SACDate(new System.DateTime(sac.DefaultCustomSettings[i]._VAL_DateTime.year,
				                                                                                 sac.DefaultCustomSettings[i]._VAL_DateTime.month,
				                                                                                 EditorGUILayout.IntField("D", sac.DefaultCustomSettings[i]._VAL_DateTime.day,GUILayout.MinWidth(0)),
				                                                                                 sac.DefaultCustomSettings[i]._VAL_DateTime.hour,
				                                                                                 sac.DefaultCustomSettings[i]._VAL_DateTime.min,
				                                                                                 sac.DefaultCustomSettings[i]._VAL_DateTime.sec));

					sac.DefaultCustomSettings[i]._VAL_DateTime = new SACDate(new System.DateTime(EditorGUILayout.IntField("Y", sac.DefaultCustomSettings[i]._VAL_DateTime.year,GUILayout.MinWidth(0)),
				                                                                                 sac.DefaultCustomSettings[i]._VAL_DateTime.month,
				                                                                                 sac.DefaultCustomSettings[i]._VAL_DateTime.day,
				                                                                                 sac.DefaultCustomSettings[i]._VAL_DateTime.hour,
				                                                                                 sac.DefaultCustomSettings[i]._VAL_DateTime.min,
				                                                                                 sac.DefaultCustomSettings[i]._VAL_DateTime.sec));

						GUILayout.EndHorizontal();

						GUILayout.BeginHorizontal();
					sac.DefaultCustomSettings[i]._VAL_DateTime = new SACDate(new System.DateTime(sac.DefaultCustomSettings[i]._VAL_DateTime.year,
				                                                                                 sac.DefaultCustomSettings[i]._VAL_DateTime.month,
				                                                                                 sac.DefaultCustomSettings[i]._VAL_DateTime.day,
				                                                                                 EditorGUILayout.IntField("H", sac.DefaultCustomSettings[i]._VAL_DateTime.hour,GUILayout.MinWidth(0)),
				                                                                                 sac.DefaultCustomSettings[i]._VAL_DateTime.min,
				                                                                                 sac.DefaultCustomSettings[i]._VAL_DateTime.sec));
						
					sac.DefaultCustomSettings[i]._VAL_DateTime = new SACDate(new System.DateTime(sac.DefaultCustomSettings[i]._VAL_DateTime.year,
				                                                                                 sac.DefaultCustomSettings[i]._VAL_DateTime.month,
				                                                                                 sac.DefaultCustomSettings[i]._VAL_DateTime.day,
				                                                                                 sac.DefaultCustomSettings[i]._VAL_DateTime.hour,
				                                                                                 EditorGUILayout.IntField("M", sac.DefaultCustomSettings[i]._VAL_DateTime.min,GUILayout.MinWidth(0)),
				                                                                                 sac.DefaultCustomSettings[i]._VAL_DateTime.sec));
						
					sac.DefaultCustomSettings[i]._VAL_DateTime = new SACDate(new System.DateTime(sac.DefaultCustomSettings[i]._VAL_DateTime.year,
				                                                                                 sac.DefaultCustomSettings[i]._VAL_DateTime.month,
				                                                                                 sac.DefaultCustomSettings[i]._VAL_DateTime.day,
				                                                                                 sac.DefaultCustomSettings[i]._VAL_DateTime.hour,
				                                                                                 sac.DefaultCustomSettings[i]._VAL_DateTime.min,
				                                                                                 EditorGUILayout.IntField("S", sac.DefaultCustomSettings[i]._VAL_DateTime.sec,GUILayout.MinWidth(0))));
						
						GUILayout.EndHorizontal();
						EditorGUIUtility.labelWidth = 0;
						GUILayout.EndVertical();
						break;
					}

					default : {
					break;
					}
				}
				GUILayout.FlexibleSpace();
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal(center, GUILayout.Width (Screen.width * .05f));
				bool delete = GUILayout.Button ("X");
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.EndHorizontal();
				if(delete){
					sac.DefaultCustomSettings = RemoveAt(sac.DefaultCustomSettings, i);
					break;
				}
				GUILayout.Space (5);
			}
		}catch(System.Exception e){
			ResetTarget();
		}

			if(i == 0){
				EditorGUILayout.LabelField("No Default Settings Defined", red);
			}

			if(showAddSetting){
				GUILayout.BeginVertical("Box", GUILayout.MinWidth(0), GUILayout.Width(Screen.width * .90f));
				if(showErrorSetting){
					GUILayout.Label (settingError, red);
				}
				GUILayout.Label ("Add New Setting", center);
				GUILayout.BeginHorizontal();
				GUILayout.Label ("Key", center, GUILayout.Width(Screen.width * .3f));
				GUILayout.Label ("Type", center, GUILayout.Width(Screen.width * .3f));
				GUILayout.Label ("Value", center, GUILayout.Width(Screen.width * .3f));
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal(center);
				_ADD_S_name = EditorGUILayout.TextField(_ADD_S_name, GUILayout.Width(Screen.width * .3f));
				_ADD_S_type = (Setting.SettingType)EditorGUILayout.EnumPopup(_ADD_S_type, GUILayout.Width(Screen.width * .3f));
				_ADD_S_Setting.Name = _ADD_S_name;
				_ADD_S_Setting.Type = _ADD_S_type;
				//START
				switch(_ADD_S_type){
				case Setting.SettingType.STRING : {
					_ADD_S_Setting._VAL_String = new SACString(EditorGUILayout.TextField(_ADD_S_Setting._VAL_String.GetString()));
					break;
				}
					
				case Setting.SettingType.BOOLEAN : {
					_ADD_S_Setting._VAL_Bool = new SACBool(EditorGUILayout.Toggle(_ADD_S_Setting._VAL_Bool.GetBool()));
					break;
				}
					
				case Setting.SettingType.COLOR : {
					Color col = EditorGUILayout.ColorField(_ADD_S_Setting._VAL_Color.GetColor());
					_ADD_S_Setting._VAL_Color = new SACColor(col.r, col.g, col.b, col.a);
					break;
				}
					
				case Setting.SettingType.FLOAT : {
					_ADD_S_Setting._VAL_Float = new SACFloat(EditorGUILayout.FloatField(_ADD_S_Setting._VAL_Float.GetFloat()));
					break;	
				}
					
				case Setting.SettingType.INTEGER : {
					_ADD_S_Setting._VAL_Int = new SACInt(EditorGUILayout.IntField(_ADD_S_Setting._VAL_Int.GetInt()));
					break;
				}
					
				case Setting.SettingType.QUATERNION : {
					GUILayout.BeginVertical();
					EditorGUIUtility.labelWidth = 15;
					GUILayout.BeginHorizontal();
					_ADD_S_Setting._VAL_Quaternion = new SACQuaternion(new Quaternion(EditorGUILayout.FloatField("X", _ADD_S_Setting._VAL_Quaternion.GetQuaternion().x,GUILayout.MinWidth(0)), _ADD_S_Setting._VAL_Quaternion.GetQuaternion().y, _ADD_S_Setting._VAL_Quaternion.GetQuaternion().z, _ADD_S_Setting._VAL_Quaternion.GetQuaternion().w));
					_ADD_S_Setting._VAL_Quaternion = new SACQuaternion(new Quaternion(_ADD_S_Setting._VAL_Quaternion.GetQuaternion().x, EditorGUILayout.FloatField("Y", _ADD_S_Setting._VAL_Quaternion.GetQuaternion().y,GUILayout.MinWidth(0)), _ADD_S_Setting._VAL_Quaternion.GetQuaternion().z, _ADD_S_Setting._VAL_Quaternion.GetQuaternion().w));
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal();
					_ADD_S_Setting._VAL_Quaternion = new SACQuaternion(new Quaternion(_ADD_S_Setting._VAL_Quaternion.GetQuaternion().x, _ADD_S_Setting._VAL_Quaternion.GetQuaternion().y, EditorGUILayout.FloatField("Z", _ADD_S_Setting._VAL_Quaternion.GetQuaternion().z,GUILayout.MinWidth(0)), _ADD_S_Setting._VAL_Quaternion.GetQuaternion().w));
					_ADD_S_Setting._VAL_Quaternion = new SACQuaternion(new Quaternion(_ADD_S_Setting._VAL_Quaternion.GetQuaternion().x, _ADD_S_Setting._VAL_Quaternion.GetQuaternion().y, _ADD_S_Setting._VAL_Quaternion.GetQuaternion().z, EditorGUILayout.FloatField("W", _ADD_S_Setting._VAL_Quaternion.GetQuaternion().w,GUILayout.MinWidth(0))));
					GUILayout.EndHorizontal();
					EditorGUIUtility.labelWidth = 0;
					GUILayout.EndVertical();
					
					break;
				}
					
				case Setting.SettingType.VECTOR2 : {
					GUILayout.BeginVertical();
					EditorGUIUtility.labelWidth = 15;
					GUILayout.BeginHorizontal();
					_ADD_S_Setting._VAL_Vector2 = new SACVector2(new Vector2(EditorGUILayout.FloatField("X", _ADD_S_Setting._VAL_Vector2.GetVector2().x,GUILayout.MinWidth(0)), _ADD_S_Setting._VAL_Vector2.GetVector2().y));
					_ADD_S_Setting._VAL_Vector2 = new SACVector2(new Vector2(_ADD_S_Setting._VAL_Vector2.GetVector2().x, EditorGUILayout.FloatField("Y", _ADD_S_Setting._VAL_Vector2.GetVector2().y,GUILayout.MinWidth(0))));
					GUILayout.EndHorizontal();
					EditorGUIUtility.labelWidth = 0;
					GUILayout.EndVertical();
					break;
				}
					
				case Setting.SettingType.VECTOR3 : {
					GUILayout.BeginVertical();
					EditorGUIUtility.labelWidth = 15;
					GUILayout.BeginHorizontal();
					_ADD_S_Setting._VAL_Vector3 = new SACVector3(new Vector3(EditorGUILayout.FloatField("X", _ADD_S_Setting._VAL_Vector3.GetVector3().x,GUILayout.MinWidth(0)), _ADD_S_Setting._VAL_Vector3.GetVector3().y, _ADD_S_Setting._VAL_Vector3.GetVector3().z));
					_ADD_S_Setting._VAL_Vector3 = new SACVector3(new Vector3(_ADD_S_Setting._VAL_Vector3.GetVector3().x, EditorGUILayout.FloatField("Y", _ADD_S_Setting._VAL_Vector3.GetVector3().y,GUILayout.MinWidth(0)), _ADD_S_Setting._VAL_Vector3.GetVector3().z));
					_ADD_S_Setting._VAL_Vector3 = new SACVector3(new Vector3(_ADD_S_Setting._VAL_Vector3.GetVector3().x, _ADD_S_Setting._VAL_Vector3.GetVector3().y, EditorGUILayout.FloatField("Z", _ADD_S_Setting._VAL_Vector3.GetVector3().z,GUILayout.MinWidth(0))));
					GUILayout.EndHorizontal();
					EditorGUIUtility.labelWidth = 0;
					GUILayout.EndVertical();
					break;
				}
					
				case Setting.SettingType.VECTOR4 : {
					GUILayout.BeginVertical();
					EditorGUIUtility.labelWidth = 15;
					GUILayout.BeginHorizontal();
					_ADD_S_Setting._VAL_Vector4 = new SACVector4(new Vector4(EditorGUILayout.FloatField("X", _ADD_S_Setting._VAL_Vector4.GetVector4().x,GUILayout.MinWidth(0)), _ADD_S_Setting._VAL_Vector4.GetVector4().y, _ADD_S_Setting._VAL_Vector4.GetVector4().z, _ADD_S_Setting._VAL_Vector4.GetVector4().w));
					_ADD_S_Setting._VAL_Vector4 = new SACVector4(new Vector4(_ADD_S_Setting._VAL_Vector4.GetVector4().x, EditorGUILayout.FloatField("Y", _ADD_S_Setting._VAL_Vector4.GetVector4().y,GUILayout.MinWidth(0)), _ADD_S_Setting._VAL_Vector4.GetVector4().z, _ADD_S_Setting._VAL_Vector4.GetVector4().w));
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal();
					_ADD_S_Setting._VAL_Vector4 = new SACVector4(new Vector4(_ADD_S_Setting._VAL_Vector4.GetVector4().x, _ADD_S_Setting._VAL_Vector4.GetVector4().y, EditorGUILayout.FloatField("Z", _ADD_S_Setting._VAL_Vector4.GetVector4().z,GUILayout.MinWidth(0)), _ADD_S_Setting._VAL_Vector4.GetVector4().w));
					_ADD_S_Setting._VAL_Vector4 = new SACVector4(new Vector4(_ADD_S_Setting._VAL_Vector4.GetVector4().x, _ADD_S_Setting._VAL_Vector4.GetVector4().y, _ADD_S_Setting._VAL_Vector4.GetVector4().z, EditorGUILayout.FloatField("W", _ADD_S_Setting._VAL_Vector4.GetVector4().w,GUILayout.MinWidth(0))));
					GUILayout.EndHorizontal();
					EditorGUIUtility.labelWidth = 0;
					GUILayout.EndVertical();
					
					break;
				}
					
				case Setting.SettingType.TIME : {
					GUILayout.BeginVertical();
					EditorGUIUtility.labelWidth = 15;
					GUILayout.BeginHorizontal();
					_ADD_S_Setting._VAL_DateTime = new SACDate(new System.DateTime(_ADD_S_Setting._VAL_DateTime.year,
					                                                                             EditorGUILayout.IntField("M", _ADD_S_Setting._VAL_DateTime.month,GUILayout.MinWidth(0)),
					                                                                             _ADD_S_Setting._VAL_DateTime.day,
					                                                                             _ADD_S_Setting._VAL_DateTime.hour,
					                                                                             _ADD_S_Setting._VAL_DateTime.min,
					                                                                             _ADD_S_Setting._VAL_DateTime.sec));
					
					_ADD_S_Setting._VAL_DateTime = new SACDate(new System.DateTime(_ADD_S_Setting._VAL_DateTime.year,
					                                                                             _ADD_S_Setting._VAL_DateTime.month,
					                                                                             EditorGUILayout.IntField("D", _ADD_S_Setting._VAL_DateTime.day,GUILayout.MinWidth(0)),
					                                                                             _ADD_S_Setting._VAL_DateTime.hour,
					                                                                             _ADD_S_Setting._VAL_DateTime.min,
					                                                                             _ADD_S_Setting._VAL_DateTime.sec));
					
					_ADD_S_Setting._VAL_DateTime = new SACDate(new System.DateTime(EditorGUILayout.IntField("Y", _ADD_S_Setting._VAL_DateTime.year,GUILayout.MinWidth(0)),
					                                                                             _ADD_S_Setting._VAL_DateTime.month,
					                                                                             _ADD_S_Setting._VAL_DateTime.day,
					                                                                             _ADD_S_Setting._VAL_DateTime.hour,
					                                                                             _ADD_S_Setting._VAL_DateTime.min,
					                                                                             _ADD_S_Setting._VAL_DateTime.sec));
					
					GUILayout.EndHorizontal();
					
					GUILayout.BeginHorizontal();
					_ADD_S_Setting._VAL_DateTime = new SACDate(new System.DateTime(_ADD_S_Setting._VAL_DateTime.year,
					                                                                             _ADD_S_Setting._VAL_DateTime.month,
					                                                                             _ADD_S_Setting._VAL_DateTime.day,
					                                                                             EditorGUILayout.IntField("H", _ADD_S_Setting._VAL_DateTime.hour,GUILayout.MinWidth(0)),
					                                                                             _ADD_S_Setting._VAL_DateTime.min,
					                                                                             _ADD_S_Setting._VAL_DateTime.sec));
					
					_ADD_S_Setting._VAL_DateTime = new SACDate(new System.DateTime(_ADD_S_Setting._VAL_DateTime.year,
					                                                                             _ADD_S_Setting._VAL_DateTime.month,
					                                                                             _ADD_S_Setting._VAL_DateTime.day,
					                                                                             _ADD_S_Setting._VAL_DateTime.hour,
					                                                                             EditorGUILayout.IntField("M", _ADD_S_Setting._VAL_DateTime.min,GUILayout.MinWidth(0)),
					                                                                             _ADD_S_Setting._VAL_DateTime.sec));
					
					_ADD_S_Setting._VAL_DateTime = new SACDate(new System.DateTime(_ADD_S_Setting._VAL_DateTime.year,
					                                                                             _ADD_S_Setting._VAL_DateTime.month,
					                                                                             _ADD_S_Setting._VAL_DateTime.day,
					                                                                             _ADD_S_Setting._VAL_DateTime.hour,
					                                                                             _ADD_S_Setting._VAL_DateTime.min,
					                                                                             EditorGUILayout.IntField("S", _ADD_S_Setting._VAL_DateTime.sec,GUILayout.MinWidth(0))));
					
					GUILayout.EndHorizontal();
					EditorGUIUtility.labelWidth = 0;
					GUILayout.EndVertical();
					break;
				}
					
				default : {
					break;
				}
				}

				//END
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				bool cancel = GUILayout.Button("Cancel");
				bool _add = GUILayout.Button("Add!");
				
				if(cancel){
					_ADD_S_name = "New Setting";
					_ADD_S_type = Setting.SettingType.STRING;
					_ADD_S_Setting = new Setting(_ADD_S_name, new SACString("Setting Value"), _ADD_S_type);
					showAddSetting = false;
					showErrorSetting = false;
				}
				
				if(_add){
					if(CheckSettingAdd(sac.DefaultCustomSettings)){
						settingError = "'" + _ADD_S_name + "' is already defined";
						showErrorSetting = true;
					}else{
						sac.DefaultCustomSettings = AddTo (sac.DefaultCustomSettings, _ADD_S_Setting);
						_ADD_S_name = "New Setting";
						_ADD_S_type = Setting.SettingType.STRING;
						_ADD_S_Setting = new Setting(_ADD_S_name, new SACString("Setting Value"), _ADD_S_type);
						showAddSetting = false;
						showErrorSetting = false;
					}
				}
				
				GUILayout.EndHorizontal();

				GUILayout.EndVertical();
			}

			GUILayout.BeginHorizontal();
			if(i > 0){
				bool removeAll = GUILayout.Button ("Remove All");
				if(removeAll){
					sac.DefaultCustomSettings = new Setting[] {};
				}
			}
			if(!showAddSetting){
				bool add = GUILayout.Button("Add New Setting");
				if(add){
					showAddSetting = true;
				}
			}
			GUILayout.EndHorizontal();
			EditorGUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();
		}

		showFileName = EditorGUILayout.Foldout (showFileName, "Save File Name");
		if(showFileName){
			EditorGUILayout.BeginVertical ();
			GUIStyle italic = new GUIStyle ();
			italic.fontStyle = FontStyle.Italic;
			italic.normal.textColor = Color.blue;
			italic.hover.textColor = Color.magenta;
			italic.padding.left = 5;
			if(GUILayout.Button ("Persistent Data Path/ (Click To Open)", italic)){
				Application.OpenURL(Application.persistentDataPath);
			}
			sac.FileName = EditorGUILayout.TextField (sac.FileName);
			EditorGUILayout.EndVertical ();
		}

		if (GUI.changed){
				EditorUtility.SetDirty (sac);
		}

		serializedObject.ApplyModifiedProperties ();

		GUILayout.Space (20);

		GUI.skin.box.padding = defPadding;
		GUI.skin.box.alignment = defBoxAlign;
		GUI.skin.textField.alignment = defAlign;
	}

	public ControlKey[] AddTo(ControlKey[] array, ControlKey valToAdd){
		ControlKey[] ret = new ControlKey[array.Length + 1];

		int i = 0;
		foreach(ControlKey k in array){
			ret[i] = k;
			i++;
		}

		ret [i] = valToAdd;

		return ret;
	}

	public Setting[] AddTo(Setting[] array, Setting valToAdd){
		Setting[] ret = new Setting[array.Length + 1];
		
		int i = 0;
		foreach(Setting k in array){
			ret[i] = k;
			i++;
		}
		
		ret [i] = valToAdd;
		
		return ret;
	}

	public T[] RemoveAt<T>(T[] source, int index){
		T[] dest = new T[source.Length - 1];
		if (index > 0)
						System.Array.Copy (source, 0, dest, 0, index);

		if (index < source.Length - 1)
						System.Array.Copy (source, index + 1, dest, index, source.Length - index - 1);

		return dest;
	}

	public Quaternion Vector4ToQuaternion(Vector4 input){
		return new Quaternion (input.x, input.y, input.z, input.w);
	}

	public Vector4 QuaternionToVector4(Quaternion input){
		return new Vector4 (input.x, input.y, input.z, input.w);
	}

	public bool CheckSettingAdd(Setting[] sets){
		foreach(Setting s in sets){
			if(s.Name.ToLower() == _ADD_S_name.ToLower()){
				return true;
			}
		}

		return false;
	}

	public bool CheckControlAdd(ControlKey[] inputs){
		foreach(ControlKey k in inputs){
			if(k.Name.ToLower() == _ADD_C_Key.Name.ToLower()){
				return true;
			}
		}

		return false;
	}

}
