using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModMenuUI : MonoBehaviour
{

	#region SINGLETON

	public static ModMenuUI ins;

	void Awake ()
	{
		if (ins != null && ins != this) {
			Destroy (this.gameObject);
			Debug.LogWarning ("More than one instance of ModMenuUI found!");
			return;
		}
		ins = this;
	}

	#endregion

	MainUI mainUI;

	// mod menu panel
	public GameObject modeMenuPanel;

	// add item dropdown
	Dropdown itemsDropdown;
	List<string> itemsDropdownOptions = new List<string> ();

	// dropdown buttons
	Button addItemButton;
	Button craftItemButton;


	// Use this for initialization
	void Start ()
	{
		mainUI = MainUI.ins;

		InitMenu ();
	}

	void InitMenu ()
	{
		addItemButton = modeMenuPanel.transform.Find ("AddButton").GetComponent<Button> ();
		craftItemButton = modeMenuPanel.transform.Find ("CraftButton").GetComponent<Button> ();

		// Add Item DropDown
		itemsDropdown = modeMenuPanel.transform.Find ("AddDropdown").GetComponent<Dropdown> ();
		itemsDropdown.ClearOptions ();

		itemsDropdownOptions.Add (" -- item");

		if (DatabaseManager.ins.itemsDB.Count > 0) {
			for (int i = 0; i < DatabaseManager.ins.itemsDB.Count; i++) {
				if (i == 0)
					continue;
				itemsDropdownOptions.Add (DatabaseManager.ins.itemsDB [i].Title);
			}
		}

		if (itemsDropdownOptions.Count > 0) {
			itemsDropdown.AddOptions (itemsDropdownOptions);
		
			itemsDropdown.onValueChanged.AddListener (delegate {
				OnAddItemDropdownValueChanged (itemsDropdown);
			});
		}

		addItemButton.gameObject.SetActive (false);
		addItemButton.onClick.AddListener (delegate {
			OnAddButtonClicked (addItemButton);
		});

		craftItemButton.gameObject.SetActive (false);
		craftItemButton.onClick.AddListener (delegate {
			OnCraftButtonClicked (craftItemButton);
		});

	}

	void OnAddItemDropdownValueChanged (Dropdown change)
	{
		Item _selectedItem = DatabaseManager.ins.GetItemByTitle (itemsDropdownOptions [change.value]);
		switch (_selectedItem.Type) {
		case "INVENTORY":
			addItemButton.gameObject.SetActive (true);
			craftItemButton.gameObject.SetActive (false);
			break;
		case "WEAPON":
			addItemButton.gameObject.SetActive (true);
			craftItemButton.gameObject.SetActive (true);
			break;
		case "CONSUMABLE":
			addItemButton.gameObject.SetActive (true);
			craftItemButton.gameObject.SetActive (false);
			break;
		}
	}

	void OnAddButtonClicked (Button button)
	{
		// get item from DB
		Item _selectedItem = DatabaseManager.ins.GetItemByTitle (itemsDropdownOptions [itemsDropdown.value]);

		PlayerHasItem _relation = PlayerHasItemDB.ins.GetByItemId (_selectedItem.ID);

		// check if id is already present in inventory
		if (_relation != null && _relation.id > -1 && _selectedItem.Stackable == 1) {

			// update relation
			_relation.amount++;

			// udpate UI
			mainUI.UpdateStatus (_relation.id, _relation.status);

			// save
			mainUI.SavePlayerItems ();

		} else {

			// get available slot
			InventorySlot _slot = InventoryUI.ins.GetNextAvailableSlot (0, _selectedItem.ID);

			// add relation & save
			PlayerHasItem _newRelation = PlayerHasItemDB.ins.Add (_selectedItem, _slot.id, 1, "ACTIVE");

			// instanciate item
			InventoryUI.ins.InstanciateInventoryItem (_newRelation);

			// udpate UI
			mainUI.UpdateStatus (_newRelation.id, _newRelation.status);
		
		}

	}

	void OnCraftButtonClicked (Button button)
	{
		Item _selectedItem = DatabaseManager.ins.GetItemByTitle (itemsDropdownOptions [itemsDropdown.value]);

		// add item to inventory with crafting status
		InventorySlot _slot = InventoryUI.ins.GetNextAvailableSlot (0, _selectedItem.ID);
		PlayerHasItem _relation = PlayerHasItemDB.ins.Add (_selectedItem, _slot.id, 1, "CRAFTING");

		// update crafting list
		CraftingItem _newCraftingItem = CraftingItemDB.ins.Add (_relation);

		// update UI
		InventoryUI.ins.InstanciateInventoryItem (_relation);

		// start crafting
		StartCoroutine (CraftingItemDB.ins.Craft (_newCraftingItem));

		// show User notification
		NotificationUI.ins.ItemAdded (_selectedItem, " : crafting started");
	}

}
