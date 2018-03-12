using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabsUI : MonoBehaviour
{

	[SerializeField] private int defaultTab = 0;

	[SerializeField] private Transform tabsNav;
	[SerializeField] private Transform tabsPanels;

	List<GameObject> tabsButtons = new List<GameObject> ();
	List<GameObject> tabsPanelGo = new List<GameObject> ();

	// Use this for initialization
	void Start ()
	{
		foreach (Transform child in tabsNav) {
			tabsButtons.Add (child.gameObject);
		}

		foreach (Transform child in tabsPanels) {
			tabsPanelGo.Add (child.gameObject);
		}

		if (tabsButtons.Count > 0 && tabsButtons.Count == tabsPanelGo.Count) {
			InitTabsUI ();
		}
	}

	void InitTabsUI ()
	{
		for (int _buttonIndex = 0; _buttonIndex < tabsButtons.Count; _buttonIndex++) {

			ShowDefaultTab ();

			Button _button = tabsButtons [_buttonIndex].GetComponent<Button> ();
			GameObject _panel = tabsPanelGo [_buttonIndex];

			_button.onClick.AddListener (delegate {
				OnTabButtonClicked (_button, _panel);
			});

		}
	}

	void OnTabButtonClicked (Button _button, GameObject _panel)
	{
		
		ResetTabs ();

		_button.interactable = false;
		_panel.SetActive (true);
	}

	void ResetTabs ()
	{
		for (int i = 0; i < tabsButtons.Count; i++) {
			// hide all tabs
			tabsButtons [i].GetComponent<Button> ().interactable = true;
			tabsPanelGo [i].SetActive (false);
		}
	}

	void ShowDefaultTab ()
	{

		ResetTabs ();

		// show default tab
		Button _defaultButton = tabsButtons [defaultTab].GetComponent<Button> ();
		_defaultButton.interactable = false;

		GameObject _defaultPanel = tabsPanelGo [defaultTab];
		_defaultPanel.SetActive (true);
	}
}
