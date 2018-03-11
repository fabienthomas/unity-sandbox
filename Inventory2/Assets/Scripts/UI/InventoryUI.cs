using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{

	#region SINGLETON

	public static InventoryUI ins;

	void Awake ()
	{
		if (ins != null && ins != this) {
			Destroy (this.gameObject);
			Debug.LogWarning ("More than one instance of InventoryUI found!");
			return;
		}
		ins = this;
	}

	#endregion

	[SerializeField] private GameObject slotPrefab;
	[SerializeField] private GameObject itemPrefab;

	private Transform parentZone;
	private Transform parentSlot;

	private List<InventoryZone> inventoryZones = new List<InventoryZone> ();

	// Use this for initialization
	void Start ()
	{

		StartCoroutine (InitUI ());

	}

	public IEnumerator InitUI ()
	{

		// init zones
		StartCoroutine (InitInventoryZones ());

		StartCoroutine (InitPlayerItems ());

		yield return null;
	}

	IEnumerator InitInventoryZones ()
	{

		inventoryZones = new List<InventoryZone> (this.GetComponentsInChildren<InventoryZone> (true));

		for (int zoneIndex = 0; zoneIndex < inventoryZones.Count; zoneIndex++) {

			// clear zone
			foreach (Transform _slot in  inventoryZones [zoneIndex].transform) {
				Destroy (_slot.gameObject);
			}

			// then fill with slots
			for (int slotIndex = 0; slotIndex < inventoryZones [zoneIndex].space; slotIndex++) {

				// build slot id
				string _idSlot = inventoryZones [zoneIndex].id.ToString () + "_" + slotIndex.ToString ();

				// instanciate slot gameobject
				GameObject _go = Instantiate (slotPrefab);

				// update slot name
				_go.transform.name = "Slot:" + _idSlot;

				// set zone as parent
				_go.transform.SetParent (inventoryZones [zoneIndex].transform);

				// update position
				_go.transform.position = inventoryZones [zoneIndex].transform.position;

				// get InventorySlot
				InventorySlot _inventorySlot = _go.GetComponent<InventorySlot> ();

				// update InventorySlot
				_inventorySlot.id = _idSlot;
				_inventorySlot.idZone = inventoryZones [zoneIndex].id;

				if (inventoryZones [zoneIndex].slotTypes.Count >= zoneIndex) {
					_inventorySlot.inventorySlotType = inventoryZones [zoneIndex].slotTypes [slotIndex];
				} else {
					_inventorySlot.inventorySlotType = inventoryZones [zoneIndex].slotType;
				}

				if (_inventorySlot.inventorySlotType == InventorySlotType.DESTROY) {
					_inventorySlot.transform.Find ("IconSlot").GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Sprites/UI/delete");
					_inventorySlot.transform.Find ("IconSlot").GetComponent<Image> ().enabled = true;
				}

				if (_inventorySlot.inventorySlotType == InventorySlotType.USE) {
					_inventorySlot.transform.Find ("IconSlot").GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Sprites/UI/use");
					_inventorySlot.transform.Find ("IconSlot").GetComponent<Image> ().enabled = true;
				}

			}

		}

		yield return null;
	}

	IEnumerator InitPlayerItems ()
	{
		if (PlayerHasItemDB.ins.playerHasItemDB.Count > 0) {

			for (int relationIndex = 0; relationIndex < PlayerHasItemDB.ins.playerHasItemDB.Count; relationIndex++) {

				InstanciateInventoryItem (PlayerHasItemDB.ins.playerHasItemDB [relationIndex]);

				// trace
				// dbm._playerHasItemDB._traceRel (_phiDB [relationIndex]);
			}

		}

		yield return null;
	}

	public void InstanciateInventoryItem (PlayerHasItem _relation)
	{

		if (_relation != null && _relation.id > -1) {

			// item data
			Item _item = DatabaseManager.ins.GetItem (_relation.item_id);

			// instanciate item gameobject
			GameObject _go = Instantiate (itemPrefab);

			// udpate name
			_go.transform.name = _item.Title;

			// update UI
			_go.GetComponentInChildren<Image> (true).sprite = _item.Sprite;
			_go.GetComponentInChildren<Image> (true).enabled = true;

			if (_relation.amount > 1) {

				// udpate amount text label
				GameObject _amountText = _go.transform.Find ("Amount").gameObject;

				// show background
				_amountText.GetComponent<Image> ().enabled = true;

				// update text
				_amountText.GetComponentInChildren<Text> ().text = _relation.amount.ToString ("D2");

				_amountText.SetActive (true);
			}

			// UI status
			GameObject _statusUI = _go.transform.Find ("Status").gameObject;

			if (_relation.status == "CRAFTING") {
				CraftingItem _craftingItem = DatabaseManager.ins._craftingItemDB.GetCraftingItemByRelId (_relation.id);
				if (_craftingItem.id > -1) {
					_statusUI.GetComponentInChildren<Text> ().text = _craftingItem.delay.ToString ();
					_statusUI.GetComponentInChildren<Image> ().sprite = Resources.Load<Sprite> ("Sprites/UI/crafting");
					_statusUI.GetComponentInChildren<Image> ().enabled = true;
				}
			} else if (_relation.status == "BROKEN") {
				_statusUI.GetComponentInChildren<Text> ().text = "broken";
				_statusUI.GetComponentInChildren<Image> ().sprite = Resources.Load<Sprite> ("Sprites/UI/broken");
				_statusUI.GetComponentInChildren<Image> ().enabled = true;
			} else {

			}

			// udpate relation
			InventoryItem _inventoryItem = _go.GetComponent<InventoryItem> ();
			_inventoryItem.id = _relation.id;
			_inventoryItem._relation = _relation;

			// update parent & position
			InventorySlot _inventorySlot = GetSlotById (_relation.slot_id);

			_go.transform.SetParent (_inventorySlot.transform);
			_go.transform.position = _inventorySlot.transform.position;

		} else {
			Debug.LogError ("_relation is null or empty");
		}
			
	}

	// get a zone by its ID
	InventoryZone GetZoneByID (int _idZone)
	{
		if (inventoryZones.Count > 0) {
			for (int i = 0; i < inventoryZones.Count; i++) {
				if (inventoryZones [i].id == _idZone) {
					return inventoryZones [i];
				}
			}
		}

		return null;
	}

	public InventorySlot GetSlotById (string _idSlot)
	{
		List<InventorySlot> _inventorySlots = new List<InventorySlot> (this.GetComponentsInChildren<InventorySlot> (true));

		return _inventorySlots.Find (i => i.id == _idSlot);
	}

	// get next empty slot in a zone for an item
	InventorySlot GetNextEmptySlot (int idZone, int idItem = -1)
	{

		InventoryZone _itemZone = GetZoneByID (idZone);
		foreach (Transform slot in _itemZone.transform) {
			if (slot.childCount == 1) {
				return slot.GetComponent<InventorySlot> ();
			}
		}

		return null;
	}

	// get next available slot in a zone for an item
	public InventorySlot GetNextAvailableSlot (int _idZone, int _idItem)
	{

		InventoryZone _itemZone = GetZoneByID (_idZone);
		Item _item = DatabaseManager.ins.GetItem (_idItem);

		parentZone = _itemZone.transform;

		// look for same item ID && stackable
		for (int i = 0; i < PlayerHasItemDB.ins.playerHasItemDB.Count; i++) {

			// IDItem already in list & item is stackable
			if (PlayerHasItemDB.ins.playerHasItemDB [i].item_id == _idItem && _item.Stackable == 1) {

				// find valid slot to stack to in gamescene
				foreach (Transform slot in parentZone) {
					if (slot.GetComponent<InventorySlot> ().id == PlayerHasItemDB.ins.playerHasItemDB [i].slot_id) {
						parentSlot = slot;
						break;
					}
				}

				// we found our slot, get out of loop
				if (parentSlot != null) {
					break;
				}

			} else { // IDItem not in List nor item is not stackable
				// find empty slot in zone in gamescene
				parentSlot = GetNextEmptySlot (_idZone, _idItem).transform;

			}
		}

		if (parentSlot == null) {
			parentSlot = GetNextEmptySlot (_idZone, _idItem).transform;
		}

		return parentSlot.GetComponent<InventorySlot> ();
	}

	public InventoryItem GetItemById (int _idItem)
	{
		PlayerHasItem _rel = PlayerHasItemDB.ins.playerHasItemDB.Find (i => i.id == _idItem);

		if (_rel != null && _rel.id > -1) {
			List<InventoryItem> _allInventoryItems = new List<InventoryItem> (this.GetComponentsInChildren<InventoryItem> (true));
			foreach (InventoryItem _inventoryItem in _allInventoryItems) {
				if (_inventoryItem._relation == _rel) {
					return _inventoryItem;
				}
			}	
		}

		return null;
	}

	public void MoveInventoryItem (InventoryItem _inventoryItem, InventorySlot _inventorySlot, bool _save = true)
	{
		_inventoryItem.inventorySlot = _inventorySlot;
		_inventoryItem.transform.SetParent (_inventorySlot.transform);
		_inventoryItem.transform.position = _inventorySlot.transform.position;

		_inventoryItem._relation.slot_id = _inventorySlot.id;

		if (_save) {
			MainUI.ins.SavePlayerItems ();
		}
	}

}
