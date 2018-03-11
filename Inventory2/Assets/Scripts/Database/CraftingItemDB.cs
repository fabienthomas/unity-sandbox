using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;

[RequireComponent (typeof(DatabaseManager))]
public class CraftingItemDB : MonoBehaviour
{

	#region SINGLETON

	public static CraftingItemDB ins;

	void Awake ()
	{
		if (ins != null && ins != this) {
			Destroy (this.gameObject);
			Debug.LogWarning ("More than one instance of CraftingItemDB found!");
			return;
		}
		ins = this;

		LoadCraftingItem ();
		ConstructCraftingItemDatabase ();

		DontDestroyOnLoad (this.gameObject);
	}

	#endregion

	#region VARIABLES

	JsonData craftingItem;
	public JsonData craftingItemJson;

	[HideInInspector] public List<CraftingItem> craftingItemDB = new List<CraftingItem> ();

	#endregion

	#region FILE ACCESS

	// items recipees database
	void LoadCraftingItem ()
	{
		craftingItem = JsonMapper.ToObject (File.ReadAllText (Application.dataPath + "/Resources/StreamingAssets/CraftingItem.json"));
	}

	public void SaveCraftingItem ()
	{
		craftingItemJson = JsonMapper.ToJson (craftingItemDB);
		File.WriteAllText (Application.dataPath + "/Resources/StreamingAssets/CraftingItem.json", craftingItemJson.ToString ());

		print ("saving craftingItemDB " + craftingItemDB.Count + " items");
	}

	#endregion

	#region CONSTRUCTOR

	void ConstructCraftingItemDatabase ()
	{
		if (craftingItem.Count > 0) {
			for (int i = 0; i < craftingItem.Count; i++) {
				craftingItemDB.Add (new CraftingItem ((int)craftingItem [i] ["id"], 
					(int)craftingItem [i] ["item_id"], 
					(int)craftingItem [i] ["phi_id"], 
					GetFloat (craftingItem [i] ["delay"].ToString (), 0.0f))
				);
			}
		}
	}

	#endregion

	#region FUNCTIONS

	public void StartCrafting ()
	{

		if (craftingItemDB.Count > 0) {
			for (int i = 0; i < craftingItemDB.Count; i++) {
				StartCoroutine (Craft (craftingItemDB [i]));
			}
		}

		Time.timeScale = 1;
	}

	public IEnumerator Craft (CraftingItem _craftingItem)
	{
		if (_craftingItem.delay > 0) {
			while (true) {
				if (_craftingItem.delay > 0) {
					yield return new WaitForSeconds (1f);
					_craftingItem.delay--;
					MainUI.ins.UpdateStatus (_craftingItem.phi_id, "CRAFTING");
				} else {
					Remove (_craftingItem);
					yield return null;
					break;
				}
			}
		} 
	}

	// add crafting
	public CraftingItem Add (PlayerHasItem _relation)
	{
		int _id = craftingItemDB.Count;
		CraftingItem _newCraftingItem = new CraftingItem (_id, _relation.item_id, _relation.id, 10f);
		craftingItemDB.Add (_newCraftingItem);

		SaveCraftingItem ();

		// UI UPDATE
		MainUI.ins.UpdateStatus (_relation.id, "CRAFTING");

		return _newCraftingItem;
	}

	// return a craftingItem list
	public CraftingItem GetCraftingItemByRelId (int _idPhi)
	{
		if (craftingItemDB.Count > 0) {
			return craftingItemDB.Find (i => i.phi_id == _idPhi);
		}

		return new CraftingItem ();
	}

	// remove crafting
	void Remove (CraftingItem _toRemove)
	{
		// UI UPDATE
		MainUI.ins.UpdateStatus (_toRemove.phi_id, "ACTIVE");

		// relation update
		PlayerHasItem _toUpdate = PlayerHasItemDB.ins.GetById (_toRemove.phi_id);

		PlayerHasItemDB.ins.UpdateStatus (_toUpdate, "ACTIVE");

		// remove from db
		craftingItemDB.Remove (_toRemove);
		SaveCraftingItem ();
	}

	private float GetFloat (string stringValue, float defaultValue)
	{
		float result = defaultValue;
		float.TryParse (stringValue, out result);
		return result;
	}

	#endregion
}
