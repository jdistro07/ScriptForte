using UnityEngine;
using System.Collections;


//The event handler is used for in execution manipulation of Events. This is for advanced users.
//Below you can see the basic setup for a SAC Event System
public class EventHandlerExample : MonoBehaviour {
	void Start () {
		if(SettingsAndControls.IsLoaded()){
			SettingsAndControls.AddEventHandler(new MyEventHandler());
		}
	}

	class MyEventHandler : SACEventHandler {
		public override SACSaveEvent OnSave (SACSaveEvent _event)
		{
			//You can modifiy the _event.UpdatedSettings if you want as well
			if(_event.UpdatedSettings.GetAllSettings()[0].Type == Setting.SettingType.STRING){
				Debug.Log ("(EventHandler Output) -> Saved: " + _event.UpdatedSettings.GetAllSettings() [0]._VAL_String.GetString());
			}
			return _event;
		}

		public override SACAddControlEvent OnControlAdd (SACAddControlEvent _event)
		{
			//Simply return the event if you don't want to make any modifications
			return _event;
		}

		public override SACChangeControlEvent OnControlChange (SACChangeControlEvent _event)
		{
			return _event;
		}

		public override SACRemoveControlEvent OnControlRemove (SACRemoveControlEvent _event)
		{
			return _event;
		}

		public override SACAddSettingEvent OnSettingAdd (SACAddSettingEvent _event)
		{
			return _event;
		}

		public override SACChangeSettingEvent OnSettingChange (SACChangeSettingEvent _event)
		{
			return _event;
		}

		public override SACRemoveSettingEvent OnSettingRemove (SACRemoveSettingEvent _event)
		{
			return _event;
		}
	}
}
