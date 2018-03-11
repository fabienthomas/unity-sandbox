using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
	Vector2 offset;
	public InventorySlot inventorySlot;
	public int id = -1;
	public PlayerHasItem _relation;

	// Use this for initialization
	void Start ()
	{
		inventorySlot = this.GetComponentInParent<InventorySlot> ();
	}

	public void OnPointerDown (PointerEventData eventData)
	{
		if (_relation != null) {
			
			offset = eventData.position - new Vector2 (transform.position.x, transform.position.y);
			transform.position = eventData.position - offset;

			// tooltip off
			TooltipUI.ins.Hide ();
		}
	}

	public void OnBeginDrag (PointerEventData eventData)
	{
		if (_relation != null) {
			// default name to empty slot
			transform.SetParent (transform.parent.parent.parent);

			GetComponent<CanvasGroup> ().blocksRaycasts = false;

			// show destroy & use if item is consumable
			// bool onlyDestroy = (item.Type != "CONSUMABLE") ? true : false;
			//mainUI.ivUI.ShowUseAndDestroy (onlyDestroy);
		}
	}

	public void OnDrag (PointerEventData eventData)
	{
		if (_relation != null) {
			transform.position = eventData.position - offset;
		}
	}

	public void OnEndDrag (PointerEventData eventData)
	{
		// hide use & destroy
		//mainUI.ivUI.HideUseAndDestroy ();

		transform.SetParent (inventorySlot.transform);
		transform.position = inventorySlot.transform.position;

		GetComponent<CanvasGroup> ().blocksRaycasts = true;
	}

	public void OnPointerEnter (PointerEventData eventData)
	{
		if (!eventData.dragging) {

			Item _item = DatabaseManager.ins.GetItem (this._relation.item_id);

			// tooltip on
			TooltipUI.ins.ShowItemToolTip (_item);	
		}
	}

	public void OnPointerExit (PointerEventData eventData)
	{
		// tooltip off
		TooltipUI.ins.Hide ();
	}
}
