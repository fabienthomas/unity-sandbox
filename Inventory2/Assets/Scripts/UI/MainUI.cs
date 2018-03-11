using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{

	#region SINGLETON

	public static MainUI ins;

	void Awake ()
	{
		if (ins != null && ins != this) {
			Destroy (this.gameObject);
			Debug.LogWarning ("More than one instance of MainUI found!");
			return;
		}
		ins = this;
	}

	#endregion

	#region VARIABLES

	#endregion

	#region FUNCTIONS

	//update InventoryItem Status
	public void UpdateStatus (int _idRelation, string _newStatus)
	{
	
		// update Inventoryitem status
		InventoryItem _inventoryItem = InventoryUI.ins.GetItemById (_idRelation);
		PlayerHasItem _relation = PlayerHasItemDB.ins.GetById (_idRelation);
	
		if (_relation != null) {
			Item _item = DatabaseManager.ins.GetItem (_relation.item_id);
			if (_inventoryItem != null) {
	
				// update UI
				if (_newStatus == "ACTIVE") {
	
					// update status text label
					GameObject _statusUI = _inventoryItem.transform.Find ("Status").gameObject;
					_statusUI.GetComponentInChildren<Image> ().enabled = false;
					_statusUI.GetComponentInChildren<Image> ().sprite = null;
					_statusUI.GetComponentInChildren<Text> ().text = "";
	
					if (_relation.amount > 1) {
						GameObject _amountUI = _inventoryItem.transform.Find ("Amount").gameObject;
						_amountUI.GetComponentInChildren<Image> ().enabled = true;
						_amountUI.GetComponentInChildren<Text> ().text = _relation.amount.ToString ("D2");
					}
	
					// show User notification
					NotificationUI.ins.ItemAdded (_item, "added to inventory!");
	
				} else if (_newStatus == "BROKEN") {
	
					// update amount text label
					GameObject _statusUI = _inventoryItem.transform.Find ("Status").gameObject;
	
					_statusUI.GetComponentInChildren<Image> ().sprite = Resources.Load<Sprite> ("Sprites/UI/broken");
	
					_statusUI.GetComponentInChildren<Image> ().enabled = true;
	
					_statusUI.GetComponentInChildren<Text> ().text = "broken";
	
				} else if (_newStatus == "CRAFTING") {
	
					// update status text label
					GameObject _statusUI = _inventoryItem.transform.Find ("Status").gameObject;
	
					CraftingItem _craftingItem = CraftingItemDB.ins.GetCraftingItemByRelId (_idRelation);
	
					if (_craftingItem != null) {
						float delay = _craftingItem.delay;
						_statusUI.GetComponentInChildren<Text> ().text = Mathf.Round (delay).ToString ();
						_statusUI.GetComponentInChildren<Image> ().sprite = Resources.Load<Sprite> ("Sprites/UI/crafting");
						_statusUI.GetComponentInChildren<Image> ().enabled = true;
					}
	
				}
	
			}
		} else {
			Debug.LogError ("relation not found for id #" + _idRelation);
		}
	}

	public void Use (int _idRelation)
	{

		PlayerHasItem _relation = PlayerHasItemDB.ins.GetById (_idRelation);
		if (_relation.amount > 1) {
			_relation.amount--;
			SavePlayerItems ();
			UpdateStatus (_idRelation, "ACTIVE");
		} else {
			InventoryItem _inventoryItem = InventoryUI.ins.GetItemById (_idRelation);
			Destroy (_inventoryItem.gameObject);
			PlayerHasItemDB.ins.Remove (_relation);
		}

	}

	public void SavePlayerItems ()
	{
		PlayerHasItemDB.ins.SavePlayerHasItem ();
	}

	#endregion
}