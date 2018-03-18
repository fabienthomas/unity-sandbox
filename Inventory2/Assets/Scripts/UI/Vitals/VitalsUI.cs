using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VitalsUI : MonoBehaviour
{
	#region SINGLETON

	public static VitalsUI ins;

	void Awake ()
	{
		if (ins != null && ins != this) {
			Destroy (this.gameObject);
			Debug.LogWarning ("More than one instance of VitalsUI found!");
			return;
		}
		ins = this;

	}

	#endregion


	public GameObject vitalsPanel;
	public GameObject vitalDetailsPanel;
	public GameObject vitalPrefab;

	public List<Color> vitalsColors = new List<Color> ();

	// Use this for initialization
	void Start ()
	{
		LoadVitals ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		UpdateVitals ();
	}

	void UpdateVitals ()
	{

		foreach (Transform child in vitalsPanel.transform) {

			Vital _vital = child.GetComponent<VitalGauge> ().vital;
			PlayerHasVital _playerHasVital = PlayerHasVitalDB.ins.playerHasVitalDB.Find (i => i.vital_id == _vital.ID);

			UpdateVital (_vital, _playerHasVital, child.gameObject);

		}

	}

	void UpdateVital (Vital _vital, PlayerHasVital _playerHasVital, GameObject _go)
	{
		// update value
		_go.transform.Find ("Value").GetComponent<Text> ().text = _playerHasVital.value.ToString () + ValueSuffix (_vital);

		// update slider
		_go.transform.GetComponentInChildren<Slider> ().value = _playerHasVital.value / 100;
	}

	void LoadVitals ()
	{
		if (PlayerHasVitalDB.ins.playerHasVitalDB.Count > 0) {

			for (int relationIndex = 0; relationIndex < PlayerHasVitalDB.ins.playerHasVitalDB.Count; relationIndex++) {

				InstanciateVital (PlayerHasVitalDB.ins.playerHasVitalDB [relationIndex]);

			}

			Vital _first = DatabaseManager.ins.GetVital (PlayerHasVitalDB.ins.playerHasVitalDB [0].vital_id);

			UpdateVitalDetails (_first);

		}
	}

	void InstanciateVital (PlayerHasVital _relation)
	{
		if (_relation != null && _relation.id > -1) {

			// item data
			Vital _vital = DatabaseManager.ins.GetVital (_relation.vital_id);

			// instanciate item gameobject
			GameObject _go = Instantiate (vitalPrefab);

			_go.GetComponent<VitalGauge> ().vital = _vital;

			// udpate name
			_go.transform.name = _vital.Title;

			// udpate title
			_go.transform.Find ("Title").GetComponent<Text> ().text = _vital.Title;

			// update value
			_go.transform.Find ("Value").GetComponent<Text> ().text = _relation.value.ToString () + ValueSuffix (_vital);

			// update slider
			_go.transform.GetComponentInChildren<Slider> ().value = _relation.value / 100;

			_go.transform.GetComponentInChildren<Slider> ().transform.GetChild (1).transform.Find ("Fill").GetComponent<Image> ().color = vitalsColors [_relation.id];

			_go.transform.SetParent (vitalsPanel.transform);
			_go.transform.position = vitalsPanel.transform.position;

		} else {
			Debug.LogError ("PlayerHasVital _relation is null or empty");
		}
	}

	public void UpdateVitalDetails (Vital _vital)
	{

		// title
		vitalDetailsPanel.transform.Find ("Title").GetComponent<Text> ().text = _vital.Title;

		// icon
		Image _icon = vitalDetailsPanel.transform.Find ("Slot").transform.Find ("Icon").GetComponent<Image> ();
		_icon.sprite = _vital.Sprite;
		_icon.color = vitalsColors [_vital.ID];

		// description
		vitalDetailsPanel.transform.Find ("Description").GetComponent<Text> ().text = _vital.Description;

	}

	string ValueSuffix (Vital _vital)
	{
		string _suffix = "%";

		switch (_vital.Type) {
		case "DEGREE":
			_suffix = "°C";
			break;
		}

		return _suffix;
	}

}
