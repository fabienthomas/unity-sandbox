public class CraftingItem
{

	public int id { get; set; }

	public int item_id { get; set; }

	public int phi_id { get; set; }

	public float delay { get; set; }

	public CraftingItem (int id, int item_id, int phi_id, float delay)
	{

		this.id = id;
		this.item_id = item_id;
		this.phi_id = phi_id;

		this.delay = delay;

	}

	public CraftingItem ()
	{
		this.id = -1;
	}
}