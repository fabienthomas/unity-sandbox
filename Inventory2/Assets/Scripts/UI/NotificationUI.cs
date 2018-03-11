using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationUI : MonoBehaviour
{

	#region SINGLETON

	public static NotificationUI ins;

	void Awake ()
	{
		if (ins != null && ins != this) {
			Destroy (this.gameObject);
			Debug.LogWarning ("More than one instance of NotificationUI found!");
			return;
		}
		ins = this;
	}

	#endregion

	#region VARIABLES

	[SerializeField] private GameObject notificationPanel;
	[SerializeField] private GameObject notificationPrefab;

	#endregion

	#region FUNCTIONS

	// function called when a new item is added/crafted
	public void ItemAdded (Item _item, string _message = "")
	{
		StartCoroutine (InstanciateItemNotification (_item, _message));
	}

	// instaniate and show a new notifiation
	IEnumerator InstanciateItemNotification (Item _item, string _message = "")
	{
		GameObject _notificationItem = Instantiate (notificationPrefab);
		CanvasGroup _cg = _notificationItem.GetComponent<CanvasGroup> ();

		// opacity to 0
		_cg.alpha = 0;

		// update notification text
		Text _label = _notificationItem.GetComponentInChildren<Text> ();
		_label.text = _item.Title + " " + _message;

		// udpate notification icon
		Image _icon = _notificationItem.GetComponentInChildren<Image> ();
		_icon.sprite = _item.Sprite;
		_icon.enabled = false;

		_notificationItem.transform.SetParent (notificationPanel.transform);
		_notificationItem.transform.position = notificationPanel.transform.position;

		// let's fade in
		StartCoroutine (FadeNotificationItem (_notificationItem, 1, .5f));

		yield return new WaitForSeconds (4f);

		// now fade out
		Hide (_notificationItem);
	}

	// hide notification
	void Hide (GameObject _notificationItem)
	{
		StartCoroutine (FadeNotificationItem (_notificationItem, 0, .5f));
	}

	// fade in/out notification
	IEnumerator FadeNotificationItem (GameObject _notificationItem, float end, float lerpTime = 1f)
	{

		CanvasGroup _cg = _notificationItem.GetComponent<CanvasGroup> ();

		float _timeStartedLerping = Time.time;
		float timeSinceStarted = Time.time - _timeStartedLerping;
		float percentageComplete = timeSinceStarted / lerpTime;

		while (true) {
			timeSinceStarted = Time.time - _timeStartedLerping;
			percentageComplete = timeSinceStarted / lerpTime;

			float currentValue = Mathf.Lerp (_cg.alpha, end, percentageComplete);

			_cg.alpha = currentValue;

			if (percentageComplete >= 1)
				break;

			yield return new WaitForFixedUpdate ();
		}

		if (_cg.alpha == 0) {
			Destroy (_notificationItem);
		}
	}

	#endregion
}
