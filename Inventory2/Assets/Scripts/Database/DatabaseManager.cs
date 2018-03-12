using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;

public class DatabaseManager : MonoBehaviour
{

	#region SINGLETON

	public static DatabaseManager ins;

	void Awake ()
	{
		if (ins != null && ins != this) {
			Destroy (this.gameObject);
			Debug.LogWarning ("More than one instance of DatabaseManager found!");
			return;
		}
		ins = this;

		DontDestroyOnLoad (this.gameObject);
	}

	#endregion

	#region VARIABLES

	// list that hold all items found in db
	JsonData items;
	[HideInInspector] public List<Item> itemsDB = new List<Item> ();

	JsonData vitals;
	public List<Vital> vitalsDB = new List<Vital> ();

	// list that hold all relations player/item
	[HideInInspector] public PlayerHasItemDB _playerHasItemDB;
	[HideInInspector] public List<PlayerHasItem> _phiDB = new List<PlayerHasItem> ();

	// list that hold all relations item/recipee
	[HideInInspector] public ItemHasRecipeDB _itemHasRecipeDB;
	[HideInInspector] public List<ItemHasRecipe> _ihrDB = new List<ItemHasRecipe> ();

	[HideInInspector] public CraftingItemDB _craftingItemDB;
	[HideInInspector] public List<CraftingItem> _criDB = new List<CraftingItem> ();

	#endregion

	#region FILE ACCESS

	// items database
	void LoadItems ()
	{
		items = JsonMapper.ToObject (File.ReadAllText (Application.dataPath + "/Resources/StreamingAssets/items.json"));	
	}

	// vitals database
	void LoadVitals ()
	{
		vitals = JsonMapper.ToObject (File.ReadAllText (Application.dataPath + "/Resources/StreamingAssets/Vitals.json"));	
	}

	#endregion

	#region CONSTRUCTORS

	void ConstructItemsDatabase ()
	{
		if (items.Count > 0) {

			for (int i = 0; i < items.Count; i++) {

				itemsDB.Add (new Item ((int)items [i] ["id"], items [i] ["slug"].ToString (), (int)items [i] ["stackable"], items [i] ["title"].ToString (), items [i] ["type"].ToString ()));

			}

		}
	}

	void ConstructVitalsDatabase ()
	{
		if (vitals.Count > 0) {

			for (int i = 0; i < vitals.Count; i++) {

				vitalsDB.Add (new Vital (
					(int)vitals [i] ["id"], 
					vitals [i] ["slug"].ToString (), 
					vitals [i] ["title"].ToString (), 
					vitals [i] ["description"].ToString (),
					vitals [i] ["type"].ToString (), 
					vitals [i] ["color"].ToString ()));

			}

		}
	}

	#endregion

	#region FUNCTION

	void Start ()
	{

		LoadItems ();

		ConstructItemsDatabase ();

		LoadVitals ();

		ConstructVitalsDatabase ();

		_playerHasItemDB = gameObject.GetComponent<PlayerHasItemDB> ();	
		_phiDB = _playerHasItemDB.playerHasItemDB;

		_itemHasRecipeDB = gameObject.GetComponent<ItemHasRecipeDB> ();
		_ihrDB = _itemHasRecipeDB.itemHasRecipeDB;

		_craftingItemDB = gameObject.GetComponent<CraftingItemDB> ();
		_criDB = _craftingItemDB.craftingItemDB;

		_craftingItemDB.StartCrafting ();

	}

	public Item GetItem (int _idItem)
	{
		return itemsDB.Find (i => i.ID == _idItem);
	}

	public Item GetItemByTitle (string _titleItem)
	{
		return itemsDB.Find (i => i.Title == _titleItem);
	}

	public Vital GetVital (int _idVital)
	{
		return vitalsDB.Find (i => i.ID == _idVital);
	}

	// just compare to Lits of int
	public bool CheckMatch (List<int> l1, List<int> l2)
	{
		if (l1.Count != l2.Count)
			return false;
		for (int i = 0; i < l1.Count; i++) {
			if (l1 [i] != l2 [i])
				return false;
		}
		return true;
	}

	#endregion

}

public class Item
{

	public int ID { get; set; }

	public string Slug { get; set; }

	public int Stackable { get; set; }

	public Sprite Sprite { get; set; }

	public string Title { get; set; }

	public string Type { get; set; }

	public Item (int id, string slug, int stackable, string title, string type)
	{

		this.ID = id;
		this.Slug = slug;
		this.Stackable = stackable;
		this.Title = title;
		this.Type = type;

		// NOTE: get more data from scriptable object ?

		this.Sprite = Resources.Load<Sprite> ("Sprites/Items/" + slug);

	}

	public Item ()
	{

		this.ID = -1;

	}
}

public class Vital
{

	public int ID { get; set; }

	public string Slug { get; set; }

	public string Title { get; set; }

	public Sprite Sprite { get; set; }

	public string Description { get; set; }

	public string Type { get; set; }

	public string Color { get; set; }

	public Vital (int id, string slug, string title, string description, string type, string color)
	{

		this.ID = id;
		this.Slug = slug;
		this.Title = title;
		this.Description = description;
		this.Type = type;
		this.Color = color;

		this.Sprite = Resources.Load<Sprite> ("Sprites/Vitals/" + slug);

	}

	public Vital ()
	{

		this.ID = -1;

	}
}