using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.EventSystems;

public class seekFunction : MonoBehaviour, IDragHandler, IPointerDownHandler
{
	private VideoPlayer videoPlayer;
	private videoPlayer_script videoPlayer_script;
	public Image progress;

	private void Start()
	{
		videoPlayer = GameObject.Find ("Player").GetComponent<VideoPlayer> ();
		videoPlayer_script = GameObject.Find ("Player").GetComponent<videoPlayer_script> ();
	}

	private void Awake()
	{
		progress = gameObject.transform.Find ("seekFill").GetComponent<Image> ();
	}

	/*private void Update()
	{
		if (videoPlayer.isPrepared && videoPlayer.isPlaying)
			progress.fillAmount = (float)(videoPlayer.time / (videoPlayer.frameCount / videoPlayer.frameRate)); //(float)videoPlayer.frame / (float)videoPlayer.frameCount;
	}*/

	public void OnPointerDown(PointerEventData eventData)
	{
		Seek (eventData);
	}

	public void OnDrag(PointerEventData eventData)
	{
		Seek (eventData);
	}

	private void Seek(PointerEventData eventData)
	{
		Vector2 localPoint;
		if (RectTransformUtility.ScreenPointToLocalPointInRectangle (progress.rectTransform, eventData.position, null, out localPoint))
		{
			float pct = Mathf.InverseLerp (progress.rectTransform.rect.xMin, progress.rectTransform.rect.xMax, localPoint.x);
			seekPercent (pct);
		}
	}

	private void seekPercent(float pct)
	{
		var frame = videoPlayer.frameCount * pct;
		videoPlayer.frame = (long)frame;
	}
}