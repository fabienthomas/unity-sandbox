using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
	MainUI mainUI;

	public string id = string.Empty;
	public int idZone = -1;
	public InventorySlotType inventorySlotType;

	Image slotImage;

	private Color defaultSlotColor;
	[SerializeField] private Color highlightColor;
	[SerializeField] private Color destroyColor;
	[SerializeField] private Color useColor;

	void Start ()
	{
		mainUI = MainUI.ins;

		slotImage = GetComponent<Image> ();
		defaultSlotColor = slotImage.color;
	}

	public void OnPointerEnter (PointerEventData eventData)
	{
		if (eventData.dragging) {
			
			slotImage.color = HighlightColor ();
		}
	}

	public void OnPointerExit (PointerEventData eventData)
	{
		slotImage.color = defaultSlotColor;
	}

	public void OnDrop (PointerEventData eventData)
	{

		//mainUI.ivUI.HideUseAndDestroy ();

		Item _item = DatabaseManager.ins.GetItem (eventData.pointerDrag.GetComponent<InventoryItem> ()._relation.item_id);

		switch (inventorySlotType) {
		case InventorySlotType.DESTROY:
			slotImage.color = defaultSlotColor;
			OnDropDESTROY (eventData, _item);
			break;
		case InventorySlotType.USE:
			// CONSUMABLE
			if (_item.Type == "CONSUMABLE") {
				slotImage.color = defaultSlotColor;
				OnDropUSE (eventData, _item);	
			}
			break;
		case InventorySlotType.CONSUMABLE:
			if (_item.Type == "CONSUMABLE") {
				OnDropINVENTORY (eventData, _item);
			}
			break;
		case InventorySlotType.WEAPON:
			if (_item.Type == "WEAPON") {
				OnDropINVENTORY (eventData, _item);
			}
			// WEAPON
			break;
		case InventorySlotType.INVENTORY:
			OnDropINVENTORY (eventData, _item);
			break;
		}
	}

	private void OnDropDESTROY (PointerEventData eventData, Item _item)
	{
		// dropped item
		InventoryItem _ivItem1 = eventData.pointerDrag.GetComponent<InventoryItem> ();
		PlayerHasItem _ivItem1Relation = PlayerHasItemDB.ins.GetById (_ivItem1.id);

		if (_ivItem1Relation.status != "CRAFTING") {

			_ivItem1Relation.amount--;

			if (_ivItem1Relation.amount <= 0) {

				// remove relation 
				PlayerHasItemDB.ins.playerHasItemDB.Remove (_ivItem1Relation);

				// remove item from UI
				Destroy (_ivItem1.gameObject);

			} else {

				// update UI
				Transform _amountText = _ivItem1.transform.Find ("Amount").transform;
				_amountText.GetComponent<Image> ().enabled = true;
				_amountText.GetComponentInChildren<Text> ().text = _ivItem1Relation.amount.ToString ("D2");

			}

			// save relation
			mainUI.SavePlayerItems ();

		}
	}

	private void OnDropUSE (PointerEventData eventData, Item _item)
	{
		// dropped item
		InventoryItem _ivItem1 = eventData.pointerDrag.GetComponent<InventoryItem> ();
		PlayerHasItem _ivItem1Relation = PlayerHasItemDB.ins.GetById (_ivItem1.id);

		_ivItem1Relation.amount--;

		if (_ivItem1Relation.amount <= 0) {

			// remove relation 
			PlayerHasItemDB.ins.playerHasItemDB.Remove (_ivItem1Relation);

			// remove item from UI
			Destroy (_ivItem1.gameObject);

		} else {

			// save relation
			mainUI.SavePlayerItems ();

			// update UI
			Transform _amountText = _ivItem1.transform.Find ("Amount").transform;
			_amountText.GetComponent<Image> ().enabled = true;
			_amountText.GetComponentInChildren<Text> ().text = _ivItem1Relation.amount.ToString ("D2");

		}

		print ("Using Item #" + _ivItem1Relation.item_id);
	}

	private void OnDropINVENTORY (PointerEventData eventData, Item _item)
	{
		// dropped item
		InventoryItem _ivItem1 = eventData.pointerDrag.GetComponent<InventoryItem> ();
		PlayerHasItem _ivItem1Relation = PlayerHasItemDB.ins.GetById (_ivItem1.id);
		InventorySlot _ivSlot1 = InventoryUI.ins.GetSlotById (_ivItem1Relation.slot_id);

		// slot is not empty
		if (transform.childCount > 1) {

			// item already in slot
			InventoryItem _ivItem2 = GetComponentInChildren<InventoryItem> ();
			PlayerHasItem _ivItem2Relation = PlayerHasItemDB.ins.GetById (_ivItem2.id);

			if (_ivItem2._relation.item_id == _ivItem1._relation.item_id && _item.Stackable > 0) {

				_ivItem2Relation.amount += _ivItem1Relation.amount;

				Transform _amountText = _ivItem2.transform.Find ("Amount").transform;
				if (_ivItem2Relation.amount > 1) {
					_amountText.GetComponent<Image> ().enabled = true;
					_amountText.GetComponentInChildren<Text> ().text = _ivItem2Relation.amount.ToString ("D2");
				} else {
					_amountText.GetComponent<Image> ().enabled = false;
					_amountText.GetComponentInChildren<Text> ().text = string.Empty;
				}

			} else {
				InventoryUI.ins.MoveInventoryItem (_ivItem1, this, false);	
				InventoryUI.ins.MoveInventoryItem (_ivItem2, _ivSlot1);	
			}


		} else {
			InventoryUI.ins.MoveInventoryItem (_ivItem1, this);	
		}

	}

	private Color HighlightColor ()
	{
		Color retColor = highlightColor;

		switch (inventorySlotType) {
		case InventorySlotType.DESTROY:
			retColor = destroyColor;
			break;
		case InventorySlotType.USE:
			retColor = useColor;
			break;
		}

		return retColor;
	}
}
