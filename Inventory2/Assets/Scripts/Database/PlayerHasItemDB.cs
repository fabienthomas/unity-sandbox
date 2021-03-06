﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;

[RequireComponent (typeof(DatabaseManager))]
public class PlayerHasItemDB : MonoBehaviour
{
	#region SINGLETON

	public static PlayerHasItemDB ins;

	void Awake ()
	{
		if (ins != null && ins != this) {
			Destroy (this.gameObject);
			Debug.LogWarning ("More than one instance of PHIDB found!");
			return;
		}
		ins = this;

		LoadPlayerHasItems ();
		ConstructPlayerHasItemDatabase ();

		DontDestroyOnLoad (this.gameObject);

	}

	#endregion

	#region DATABASE

	// player items
	JsonData playerHasItem;
	[HideInInspector] public List<PlayerHasItem> playerHasItemDB = new List<PlayerHasItem> ();

	// loads player items database from JSON
	void LoadPlayerHasItems ()
	{
		playerHasItem = JsonMapper.ToObject (File.ReadAllText (Application.dataPath + "/Resources/StreamingAssets/PlayerHasItem.json"));	
	}

	// Construct player items database
	void ConstructPlayerHasItemDatabase ()
	{
		if (playerHasItem.Count > 0) {
			for (int i = 0; i < playerHasItem.Count; i++) {
				playerHasItemDB.Add (new PlayerHasItem (
					(int)playerHasItem [i] ["id"], 
					(int)playerHasItem [i] ["item_id"], 
					playerHasItem [i] ["slot_id"].ToString (), 
					(int)playerHasItem [i] ["amount"], 
					playerHasItem [i] ["status"].ToString ()));
			}
		}
	}

	public JsonData playerHasItemJson;

	// save player items database to JSON
	public void SavePlayerHasItem ()
	{
		playerHasItemJson = JsonMapper.ToJson (playerHasItemDB);
		File.WriteAllText (Application.dataPath + "/Resources/StreamingAssets/PlayerHasItem.json", playerHasItemJson.ToString ());

		CraftPanelUI.ins.RefreshCraftPanel ();

		print ("saving playerHasItemDB " + playerHasItemDB.Count + " items");
	}

	#endregion

	// add & return a player/item relation
	public PlayerHasItem Add (Item _item, string _idSlot, int amount, string _status)
	{
		PlayerHasItem _relation = new PlayerHasItem (GetNextId (), _item.ID, _idSlot, amount, _status);
		playerHasItemDB.Add (_relation);
	
		SavePlayerHasItem ();

		return _relation;
	}

	// return a player/item relation from id
	public PlayerHasItem GetById (int _id)
	{
		return playerHasItemDB.Find (p => p.id == _id);
	}

	// return a player/item relation from item_id
	public PlayerHasItem GetByItemId (int _idItem)
	{
		return playerHasItemDB.Find (p => p.item_id == _idItem);
	}

	// remove a player/item relation and save database
	public void Remove (PlayerHasItem _toRemove)
	{
		if (playerHasItemDB.Contains (_toRemove)) {
			playerHasItemDB.Remove (_toRemove);
			SavePlayerHasItem ();
		}
	}

	// update slot id on a player/item relation
	public void UpdateSlotId (PlayerHasItem _toUpdate, string _idSlot)
	{
		_toUpdate.slot_id = _idSlot;
		SavePlayerHasItem ();
	}

	// update a player/item relation status
	public void UpdateStatus (PlayerHasItem _toUpdate, string _status)
	{
		_toUpdate.status = _status;
		SavePlayerHasItem ();
	}

	// return next valid id
	public int GetNextId ()
	{
		int highestID = 0;

		if (playerHasItemDB.Count > 0) {
			for (int i = 0; i < playerHasItemDB.Count; i++) {
				if (playerHasItemDB [i].id > highestID) {
					highestID = playerHasItemDB [i].id;
				}
			}
			highestID++;
		}

		return highestID;
	}







	/* DEBUG */

	public void _trace ()
	{
		if (playerHasItemDB.Count > 0) {

			for (int i = 0; i < playerHasItemDB.Count; i++) {
				_traceRel (playerHasItemDB [i]);
			}

		}
	}

	public void _traceRel (PlayerHasItem _v)
	{
		if (_v != null) {
			print ("id: " + _v.id + " / item id: " + _v.item_id + " / amount: " + _v.amount + " / slot_id: " + _v.slot_id + " / status: " + _v.status);
		}
	}
}
