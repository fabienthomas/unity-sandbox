using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;

[RequireComponent (typeof(DatabaseManager))]
public class ItemHasRecipeDB : MonoBehaviour
{

	#region SINGLETON

	public static ItemHasRecipeDB ins;

	void Awake ()
	{
		if (ins != null && ins != this) {
			Destroy (this.gameObject);
			Debug.LogWarning ("More than one instance of ItemHasRecipeDB found!");
			return;
		}
		ins = this;

		LoadItemHasRecipe ();
		ConstructItemHasRecipeDatabase ();

		DontDestroyOnLoad (this.gameObject);
	}

	#endregion

	#region VARIABLES

	DatabaseManager dbm;

	JsonData itemHasRecipe;

	[HideInInspector] public List<ItemHasRecipe> itemHasRecipeDB = new List<ItemHasRecipe> ();

	List<ItemRecipe> itemRecipes = new List<ItemRecipe> ();

	#endregion

	#region FILE ACCESS

	// items recipees database
	void LoadItemHasRecipe ()
	{
		itemHasRecipe = JsonMapper.ToObject (File.ReadAllText (Application.dataPath + "/Resources/StreamingAssets/ItemHasRecipe.json"));	
	}

	#endregion

	#region CONSTRUCTORS

	void ConstructItemHasRecipeDatabase ()
	{
		if (itemHasRecipe.Count > 0) {
			for (int i = 0; i < itemHasRecipe.Count; i++) {
				itemHasRecipeDB.Add (new ItemHasRecipe ((int)itemHasRecipe [i] ["id"], (int)itemHasRecipe [i] ["item_id"], (int)itemHasRecipe [i] ["ingredient_id"], (int)itemHasRecipe [i] ["quantity"]));
			}
		}
	}

	#endregion

	#region FUNCTION

	void Start ()
	{
		dbm = DatabaseManager.ins;
	}

	ItemRecipe BuildRecipeForItem (Item _itemToCraft)
	{
		// check if recipe already built
		ItemRecipe _checkedRecipe = itemRecipes.Find (i => i.item_id == _itemToCraft.ID);

		if (_checkedRecipe == null) {
			
			// extract this item from relations
			List<ItemHasRecipe> _recipeRelations = itemHasRecipeDB.FindAll (i => i.Item_id.Equals (_itemToCraft.ID));

			if (_recipeRelations.Count > 0) {

				// prepare ingredients list for item
				List<ItemRecipeIngredient> _itemRecipeIngredients = new List<ItemRecipeIngredient> ();

				// loop item relations
				for (int i = 0; i < _recipeRelations.Count; i++) {
				
					// add ingredients to list
					_itemRecipeIngredients.Add (new ItemRecipeIngredient (_recipeRelations [i].Ingredient_id, _recipeRelations [i].Quantity));

				}

				return new ItemRecipe (_itemToCraft.ID, _itemRecipeIngredients);
			}
		} else {
			return _checkedRecipe;
		}

		return new ItemRecipe ();
	}

	public bool CanPlayerCraftItem (Item _itemToCraft)
	{
		bool canCraft = false;

		// check if player has items
		if (dbm._phiDB.Count > 0) {
			
			// recipee for item
			ItemRecipe _itemRecipe = BuildRecipeForItem (_itemToCraft);

			// loop ingredients
			int _count = 0;
			foreach (var _ingredient in _itemRecipe.ingredients) {
				if (dbm._phiDB.Find (i => i.item_id == _ingredient.item_id && i.amount >= _ingredient.quantity) != null) {
					_count++;
				}
			}

			// check if player has all ingredients
			if (_count == _itemRecipe.ingredients.Count) {
				canCraft = true;
			}

		}

		return canCraft;
	}

	public void UseIngredientsForCraftingItem (Item _itemToCraft)
	{
		if (CanPlayerCraftItem (_itemToCraft)) {

			// recipee for item
			ItemRecipe _itemRecipe = BuildRecipeForItem (_itemToCraft);

			foreach (var _ingredient in _itemRecipe.ingredients) {
				PlayerHasItem _relation = dbm._phiDB.Find (i => i.item_id == _ingredient.item_id && i.amount >= _ingredient.quantity);
				MainUI.ins.Use (_relation.id);
			}
		}
	}

	#endregion
}

public class ItemRecipe
{

	public int item_id { get; set; }

	public List<ItemRecipeIngredient> ingredients { get; set; }

	public ItemRecipe (int item_id, List<ItemRecipeIngredient> ingredients)
	{

		this.item_id = item_id;

		this.ingredients = ingredients;

	}

	public ItemRecipe ()
	{
		this.item_id = -1;
	}
}

public class ItemRecipeIngredient
{

	public int item_id { get; set; }

	public int quantity { get; set; }

	public ItemRecipeIngredient (int item_id, int quantity)
	{

		this.item_id = item_id;

		this.quantity = quantity;

	}

	public ItemRecipeIngredient ()
	{
		this.item_id = -1;
	}
}