using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;

[RequireComponent (typeof(DatabaseManager))]
public class PlayerHasVitalDB : MonoBehaviour
{

	#region SINGLETON

	public static PlayerHasVitalDB ins;

	void Awake ()
	{
		if (ins != null && ins != this) {
			Destroy (this.gameObject);
			Debug.LogWarning ("More than one instance of PlayerHasVitalDB found!");
			return;
		}
		ins = this;

		LoadPlayerHasVitals ();
		ConstructPlayerHasVitalDatabase ();

		DontDestroyOnLoad (this.gameObject);

	}

	#endregion

	#region DATABASE

	// player items
	JsonData playerHasVital;
	[HideInInspector] public List<PlayerHasVital> playerHasVitalDB = new List<PlayerHasVital> ();

	// loads player items database from JSON
	void LoadPlayerHasVitals ()
	{
		playerHasVital = JsonMapper.ToObject (File.ReadAllText (Application.dataPath + "/Resources/StreamingAssets/PlayerHasVital.json"));	
	}

	// Construct player items database
	void ConstructPlayerHasVitalDatabase ()
	{
		if (playerHasVital.Count > 0) {
			for (int i = 0; i < playerHasVital.Count; i++) {
				playerHasVitalDB.Add (new PlayerHasVital (
					(int)playerHasVital [i] ["id"], 
					(int)playerHasVital [i] ["vital_id"], 
					GetFloat (playerHasVital [i] ["value"].ToString (), 0.0f)));
			}
		}
	}

	public JsonData playerHasItemJson;

	// save player items database to JSON
	public void SavePlayerHasVital ()
	{
		playerHasItemJson = JsonMapper.ToJson (playerHasVitalDB);
		File.WriteAllText (Application.dataPath + "/Resources/StreamingAssets/PlayerHasVital.json", playerHasItemJson.ToString ());

		print ("saving playerHasVitalDB " + playerHasVitalDB.Count + " items");
	}

	#endregion

	#region FUNCTIONS

	void Start ()
	{
		//_trace ();
	}

	private float GetFloat (string stringValue, float defaultValue)
	{
		float result = defaultValue;
		float.TryParse (stringValue, out result);
		return result;
	}

	#endregion

	/* DEBUG */

	public void _trace ()
	{
		if (playerHasVitalDB.Count > 0) {

			for (int i = 0; i < playerHasVitalDB.Count; i++) {
				_traceRel (playerHasVitalDB [i]);
			}

		}
	}

	public void _traceRel (PlayerHasVital _v)
	{
		if (_v != null) {
			print ("id: " + _v.id + " / item id: " + _v.vital_id + " / value: " + _v.value);
		}
	}
}
