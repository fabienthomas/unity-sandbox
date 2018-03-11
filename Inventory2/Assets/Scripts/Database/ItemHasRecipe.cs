public class ItemHasRecipe
{

	public int ID { get; set; }

	public int Item_id { get; set; }

	public int Ingredient_id { get; set; }

	public int Quantity { get; set; }

	public ItemHasRecipe (int id, int item_id, int ingredient_id, int quantity)
	{

		this.ID = id;
		this.Item_id = item_id;
		this.Ingredient_id = ingredient_id;
		this.Quantity = quantity;

	}

	public ItemHasRecipe ()
	{
		this.ID = -1;
	}
}