using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftPanelUI : MonoBehaviour
{
	#region SINGLETON

	public static CraftPanelUI ins;

	void Awake ()
	{
		if (ins != null && ins != this) {
			Destroy (this.gameObject);
			Debug.LogWarning ("More than one instance of CraftPanelUI found!");
			return;
		}
		ins = this;
	}

	#endregion

	#region VARIABLES

	[SerializeField] private GameObject craftPanel;

	[SerializeField] private GameObject craftingItemPrefab;
	[SerializeField] private GameObject craftingItemIngredientPrefab;

	#endregion

	#region FUNCTIONS

	// Use this for initialization
	void Start ()
	{
		StartCoroutine (InitCraftPanelUI ());
	}

	// loads recipees in craftpanel
	public IEnumerator InitCraftPanelUI ()
	{
		StartCoroutine (ClearCraftPanel ());

		if (ItemHasRecipeDB.ins.itemHasRecipeDB.Count > 0) {
			List<Item> _items = new List<Item> ();
			List<ItemHasRecipe> _recipees = new List<ItemHasRecipe> ();
			for (int i = 0; i < ItemHasRecipeDB.ins.itemHasRecipeDB.Count; i++) {
				Item _itemToCheck = DatabaseManager.ins.GetItem (ItemHasRecipeDB.ins.itemHasRecipeDB [i].Item_id);
				if (!_items.Contains (_itemToCheck)) {
					_recipees.Add (ItemHasRecipeDB.ins.itemHasRecipeDB [i]);
					_items.Add (_itemToCheck);
				}
			}
			if (_recipees.Count > 0 && _recipees.Count == _items.Count) {

				for (int i = 0; i < _recipees.Count; i++) {

					Item _itemToCraft = DatabaseManager.ins.GetItem (_items [i].ID);

					bool playerCanCraftitem = ItemHasRecipeDB.ins.CanPlayerCraftItem (_itemToCraft);

					// instanciate item
					GameObject itemObj = Instantiate (craftingItemPrefab);

					// update item icon
					Image icon = itemObj.transform.GetChild (0).transform.Find ("Icon").GetComponent<Image> ();
					icon.sprite = _items [i].Sprite;
					icon.enabled = true;

					// isolate details
					itemObj.transform.Find ("CraftingItemTitle").transform.GetComponent<Text> ().text = _items [i].Title; 

					// update ingredients
					Transform craftingItemIngredients = itemObj.transform.Find ("CraftingItemIngredients").transform;
					for (int ii = 0; ii < ItemHasRecipeDB.ins.itemHasRecipeDB.Count; ii++) {

						if (ItemHasRecipeDB.ins.itemHasRecipeDB [ii].Item_id == _recipees [i].Item_id) {

							Item _ingredient = DatabaseManager.ins.GetItem (ItemHasRecipeDB.ins.itemHasRecipeDB [ii].Ingredient_id);

							// instanciate item
							GameObject ingredientObject = Instantiate (craftingItemIngredientPrefab);

							// update ingredient title
							ingredientObject.transform.Find ("Label").GetComponent<Text> ().text = _ingredient.Title + ": ";

							// update ingredient amount
							ingredientObject.transform.Find ("Quantity").GetComponent<Text> ().text = ItemHasRecipeDB.ins.itemHasRecipeDB [ii].Quantity.ToString ();

							// set parent
							ingredientObject.transform.SetParent (craftingItemIngredients);

							// set position
							ingredientObject.transform.position = craftingItemIngredients.position;

						}

					}

					// on button click
					Button craftButton = itemObj.GetComponentInChildren<Button> ();

					// deactivate craft button if player can not craft item atm
					if (playerCanCraftitem == false) {
						craftButton.interactable = false;
					}

					// let's craft item on button click
					craftButton.onClick.AddListener (() => AddCraftItem (_itemToCraft));

					// set parent
					itemObj.transform.SetParent (craftPanel.transform);

					// set position
					itemObj.transform.position = craftPanel.transform.position;

				}

			}
		}
		yield return null;
	}

	// does a wipe in craftpanel, destroy all recipees
	IEnumerator ClearCraftPanel ()
	{
		if (craftPanel.transform.childCount > 0) {
			foreach (Transform child in craftPanel.transform) {
				Destroy (child.gameObject);
			}
		}
		yield return null;
	}

	// clear & refresh craftpanel
	public void RefreshCraftPanel ()
	{
		// clear panel
		StartCoroutine (ClearCraftPanel ());

		// load panel
		StartCoroutine (InitCraftPanelUI ());

		print ("Refreshed CraftPanel UI");
	}

	// add a new item to craft
	void AddCraftItem (Item _selectedItem)
	{
		// get available slot
		InventorySlot _slot = InventoryUI.ins.GetNextAvailableSlot (0, _selectedItem.ID);

		ItemHasRecipeDB.ins.UseIngredientsForCraftingItem (_selectedItem);

		// add player/item relation
		PlayerHasItem _relation = PlayerHasItemDB.ins.Add (_selectedItem, _slot.id, 1, "CRAFTING");

		// update crafting list
		CraftingItemDB.ins.Add (_relation);

		// update UI
		InventoryUI.ins.InstanciateInventoryItem (_relation);

		// start crafting
		CraftingItemDB.ins.StartCrafting ();

		// show User notification
		NotificationUI.ins.ItemAdded (_selectedItem, " : crafting started");

	}

	#endregion
}
