using UnityEngine;
using UnityEngine.UI;

public class TutorialPanel : MonoBehaviour
{
	[Header("Panels")]
	[SerializeField] Transform intro;
	[SerializeField] GameObject uinav;
	[SerializeField] GameObject learn;
	[SerializeField] GameObject playingTheGame;
	[SerializeField] GameObject misc;

	[Header("Scene Controls")]
	[SerializeField] Button btnBack;

	[Header("Scene Components")]
	[SerializeField] Transform content;
	[SerializeField] Transform sectionContent;

	private void Start()
	{
		intro = GameObject.Find("Scroll View").transform.Find("Viewport").transform.Find("tutorial_intro");
		UIManager uim = GameObject.Find("AIOGameManager").GetComponent<UIManager>();

		// add sounds to each buttons on the section panel
		foreach(Transform child in sectionContent){

			Button btnSection = child.gameObject.GetComponent<Button>();

			btnSection.onClick.AddListener(() => {
				uim.sfxOpen();
			});

		}

		// add sound and event to the back button
		btnBack.onClick.AddListener(() => {
			uim.sfxClose();
			uim.toMainUIFast();
		});
	}

	public void clickIntro(){
		content.gameObject.SetActive(false);
		content.gameObject.SetActive(true);
		intro.gameObject.SetActive(true);
	}

	public void clickPlayingTheGame(){
		panelSpawner(playingTheGame);
	}

	void panelSpawner(GameObject panelObject){
		if(intro.gameObject.activeInHierarchy){

			// deactivate intro and spawn the panel
			content.gameObject.SetActive(false);
			intro.gameObject.SetActive(false);
			content.gameObject.SetActive(true);

			var panel = Instantiate(panelObject);
			panel.transform.SetParent(content.transform, false);
		}
	}
}
