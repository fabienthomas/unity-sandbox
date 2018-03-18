public class ItemHasModifier
{
	
	public int ID { get; set; }

	public int Item_id { get; set; }

	public int Vital_id { get; set; }

	public int Value { get; set; }

	public ItemHasModifier (int id, int item_id, int vital_id, int value)
	{

		this.ID = id;
		this.Item_id = item_id;
		this.Vital_id = vital_id;
		this.Value = value;

	}

	public ItemHasModifier ()
	{
		this.ID = -1;
	}
}
