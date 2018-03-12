public class PlayerHasVital
{

	public int id { get; set; }

	public int vital_id { get; set; }

	public float value { get; set; }

	public PlayerHasVital (int id, int vital_id, float value)
	{

		this.id = id;
		this.vital_id = vital_id;
		this.value = value;
	}

	public PlayerHasVital ()
	{

		this.id = -1;

	}
}