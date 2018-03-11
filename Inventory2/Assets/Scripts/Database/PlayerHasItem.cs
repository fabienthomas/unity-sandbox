public class PlayerHasItem
{

	public int id { get; set; }

	public int item_id { get; set; }

	public string slot_id { get; set; }

	public string status { get; set; }

	public int amount { get; set; }

	public PlayerHasItem (int id, int item_id, string slot_id, int amount, string status)
	{

		this.id = id;
		this.item_id = item_id;
		this.slot_id = slot_id;
		this.amount = amount;
		this.status = status;
	}

	public PlayerHasItem ()
	{

		this.id = -1;

	}
}