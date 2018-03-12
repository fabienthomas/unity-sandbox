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
	public GameObject itemTooltipPrefab;

	bool hasItemTooltipPrefab = false;

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
			offsetY = tooltipRect.rect.height / 1.75f;
			tooltipPanel.transform.position = Input.mousePosition - new Vector3 (-offsetX, offsetY, 0);
		
		}
	}

	public void ShowItemToolTip (Item _item)
	{
		

		tooltipPanel.SetActive (true);

		if (hasItemTooltipPrefab) {
	
			Clear ();
			hasItemTooltipPrefab = false;

		} else {

			// instanciate item tool tip prefab
			GameObject _go = Instantiate (itemTooltipPrefab);

			// icon
			Image _icon = _go.transform.Find ("Slot").Find ("Icon").GetComponent<Image> ();
			_icon.sprite = _item.Sprite;

			// details
			Transform _details = _go.transform.Find ("Details");

			Text _title = _details.Find ("Title").GetComponent<Text> ();

			_title.text = _item.Title;

			Text _description = _details.Find ("Description").GetComponent<Text> ();

			_description.text = _item.Title;

			// parent & position
			_go.transform.SetParent (tooltipPanel.transform);
			_go.transform.position = tooltipPanel.transform.position;

			hasItemTooltipPrefab = true;
		
		}
	}

	void Clear ()
	{
		foreach (Transform child in tooltipPanel.transform) {
			Destroy (child.gameObject);
		}
	}

	public void Show (string _tooltipText)
	{
		// isolate text to update & update it
		tooltipPanel.transform.GetComponentInChildren<Text> (true).text = _tooltipText;

		tooltipPanel.SetActive (true);
	}

	public void Hide ()
	{
		Clear ();

		tooltipPanel.SetActive (false);
	
		hasItemTooltipPrefab = false;
	}
		
}
