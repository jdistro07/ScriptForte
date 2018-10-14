using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

public class LearningController : MonoBehaviour {

	[Header("UI Elements")]
	[SerializeField] Button btnBackToMain;
	[SerializeField] Button btnSceneReload;

	[Header("Simulation")]
	[SerializeField] Text IDEtext;
	[SerializeField] InputField IDE;
	[SerializeField] Toggle tgl_localSim;
	[SerializeField] string fileTarget = "index.html";

	SimpleWebBrowser.BrowserEngine mainBrowserEngine;
	UIManager uIManager;

	string appFolder;

	//static values
	static bool isReload;
	static string txt_code;

	public string workURL;

	private void Awake()
	{
		
		appFolder = System.IO.Path.Combine(System.Environment.GetFolderPath(
			System.Environment.SpecialFolder.MyDocuments),
			Application.companyName);

		StartCoroutine(dirCreator(appFolder, fileTarget));

		workURL = "file:///"+appFolder;

		// clear file every awake
		if(File.Exists(appFolder+"/"+fileTarget)){

			StreamWriter file = new StreamWriter(appFolder+"/"+fileTarget);
			file.WriteLine(string.Empty);
			file.Close();
			Debug.Log("Write success in "+fileTarget);

		}

		if(isReload){

			Debug.Log("Reloaded scene!");
			IDE.text = txt_code;

		}

	}

	// Use this for initialization
	void Start () {

		SimpleWebBrowser.WebBrowser2D webBrowser2D = GameObject.Find("Browser2D").GetComponent<SimpleWebBrowser.WebBrowser2D>();
		uIManager = GameObject.Find("AIOGameManager").GetComponent<UIManager>();

		IDEtext.horizontalOverflow = HorizontalWrapMode.Overflow;

		// add listeners to buttons
		btnBackToMain.onClick.AddListener(()=>{

			uIManager.sfxClose();
			uIManager.toMainUIFast();

		});

		btnSceneReload.onClick.AddListener(() =>{

			isReload = true;
			uIManager.sfxSpecial();

			txt_code = IDEtext.text;

			Initiate.Fade("learn", Color.black, 3f);

		});

	}

	public void toggleSimulation(bool isEnabled){

		isEnabled = tgl_localSim.isOn;

		InputField urltext = GameObject.Find("UrlField").GetComponent<InputField>();
		mainBrowserEngine = GameObject.Find("Browser2D").GetComponent<SimpleWebBrowser.WebBrowser2D>()._mainEngine;

		if(isEnabled){

			urltext.text = workURL+"/index.html";
			mainBrowserEngine.SendNavigateEvent(urltext.text, false, false);

		}
	}

	public IEnumerator dirCreator(string path, string fileName){

		// create directory if it does not exist in MyDocuments folder
		if(!Directory.Exists(path)){

			var create = Directory.CreateDirectory(path);

			yield return create;


			// create file if does not exist to the directory
			if(!File.Exists(path)){

				var fileCreate = File.Create(path+"/"+fileName);

				yield return fileCreate;

			}

			Debug.Log("Directory created!");

		}

	}

	public void fileWriter(){

		Debug.Log(appFolder);

		if(tgl_localSim.isOn){

			if(File.Exists(appFolder+"/"+fileTarget)){

				StreamWriter file = new StreamWriter(appFolder+"/"+fileTarget);

				file.WriteLine(IDE.text);

				file.Close();

				Debug.Log("Write success in "+fileTarget);

			}
			
		}

	}

}
