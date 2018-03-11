using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryZone : MonoBehaviour
{
	public int id = -1;
	public int space = 1;

	// general type, used if no type defined in List InventorySlotType
	public InventorySlotType slotType = InventorySlotType.INVENTORY;

	// List InventorySlotType
	public List<InventorySlotType> slotTypes = new List<InventorySlotType> ();
}
