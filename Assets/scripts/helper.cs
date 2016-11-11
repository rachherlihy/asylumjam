using System.Collections.Generic;

public enum direction
{
	none, up, down, left, right
};

public class helper
{
	static Dictionary<direction, direction> reverse_lookup = new Dictionary<direction, direction>()
	{
		{direction.up, direction.down },
		{direction.down, direction.up },
		{direction.left, direction.right },
		{direction.right, direction.left },
	};


	public static direction inverse_direction( direction _direction )
	{
		return reverse_lookup[ _direction ];
	}
}