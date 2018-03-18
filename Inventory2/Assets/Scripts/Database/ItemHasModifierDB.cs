using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;

[RequireComponent (typeof(DatabaseManager))]
public class ItemHasModifierDB : MonoBehaviour
{

	#region SINGLETON

	public static ItemHasModifierDB ins;

	void Awake ()
	{
		if (ins != null && ins != this) {
			Destroy (this.gameObject);
			Debug.LogWarning ("More than one instance of ItemHasModifierDB found!");
			return;
		}
		ins = this;

		LoadItemHasModifier ();
		ConstructItemHasModifierDatabase ();

		DontDestroyOnLoad (this.gameObject);
	}

	#endregion

	#region VARIABLES

	DatabaseManager dbm;

	JsonData itemHasModifier;

	[HideInInspector] public List<ItemHasModifier> itemHasModifierDB = new List<ItemHasModifier> ();

	List<Modifier> Modifiers = new List<Modifier> ();

	#endregion

	#region FILE ACCESS

	// items recipees database
	void LoadItemHasModifier ()
	{
		itemHasModifier = JsonMapper.ToObject (File.ReadAllText (Application.dataPath + "/Resources/StreamingAssets/ItemHasModifier.json"));	
	}

	#endregion

	#region CONSTRUCTORS

	void ConstructItemHasModifierDatabase ()
	{
		if (itemHasModifier.Count > 0) {
			for (int i = 0; i < itemHasModifier.Count; i++) {
				itemHasModifierDB.Add (new ItemHasModifier ((int)itemHasModifier [i] ["id"], (int)itemHasModifier [i] ["item_id"], (int)itemHasModifier [i] ["vital_id"], (int)itemHasModifier [i] ["value"]));
			}
		}
	}

	#endregion

	#region FUNCTION

	void Start ()
	{
		dbm = DatabaseManager.ins;
	}

	public List<ItemHasModifier> HasModifier (int _idItem)
	{

		return itemHasModifierDB.FindAll (i => i.Item_id == _idItem);
	}

	public void ApplyModifier (int _idItem)
	{

		List<ItemHasModifier> _modifiers = itemHasModifierDB.FindAll (i => i.Item_id == _idItem);

		if (_modifiers.Count > 0) {

			for (int i = 0; i < _modifiers.Count; i++) {
				
				Vital _vital = dbm.GetVital (_modifiers [i].Vital_id);
				PlayerHasVital _playerHasVitalRelation = PlayerHasVitalDB.ins.playerHasVitalDB.Find (v => v.vital_id == _modifiers [i].Vital_id);

				_playerHasVitalRelation.value += _modifiers [i].Value;

				_playerHasVitalRelation.value = Mathf.Clamp (_playerHasVitalRelation.value, 0, 100);

			}

			PlayerHasVitalDB.ins.SavePlayerHasVital ();
		}

	}

	#endregion

}

public class Modifier
{

	public int id { get; set; }

	public string slug { get; set; }

	public Modifier (int id, string slug)
	{

		this.id = id;

		this.slug = slug;

	}

	public Modifier ()
	{
		this.id = -1;
	}
}
