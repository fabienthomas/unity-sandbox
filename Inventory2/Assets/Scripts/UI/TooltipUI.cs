using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TooltipUI : MonoBehaviour
{

	#region SINGLETON

	public static TooltipUI ins;

	void Awake ()
	{
		if (ins != null && ins != this) {
			Destroy (this.gameObject);
			Debug.LogWarning ("More than one instance of TooltipUI found!");
			return;
		}
		ins = this;
	}

	#endregion

	public GameObject tooltipPanel;

	RectTransform tooltipRect;

	float offsetX;
	float offsetY;

	void Start ()
	{
		tooltipRect = tooltipPanel.GetComponent<RectTransform> ();
	}

	void Update ()
	{
		if (tooltipPanel.activeInHierarchy) {

			offsetX = tooltipRect.rect.width / 1.75f;

			tooltipPanel.transform.position = Input.mousePosition - new Vector3 (-offsetX, 0, 0);
		
		}
	}

	public void ShowItemToolTip (Item _item)
	{
		string tt = _item.Title + " " + _item.Type;
		Show (tt);
	}

	public void Show (string _tooltipText)
	{
		// isolate text to update & update it
		tooltipPanel.transform.GetComponentInChildren<Text> (true).text = _tooltipText;

		tooltipPanel.SetActive (true);
	}

	public void Hide ()
	{
		tooltipPanel.SetActive (false);
	}
		
}
