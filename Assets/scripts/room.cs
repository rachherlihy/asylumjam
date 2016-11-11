using UnityEngine;
using System.Collections.Generic;

public class room
{
	static int i = 0;

	public room()
	{
		i++;
		Debug.Log( i );
	}

	~room()
	{
		i--;
		Debug.Log( i );
	}

	public Dictionary<direction, room> adjancent_rooms = new Dictionary<direction, room>()
	{
		{ direction.up, null },
		{ direction.down, null },
		{ direction.left, null },
		{ direction.right, null }
	};

	public Sprite floor_sprite;

	public virtual void on_enter()
	{
	}

	public virtual void on_exit()
	{
	}
}

public class room_start : room
{
}

public class room_end : room
{
	public override void on_enter()
	{
		state_manager.add_queue(
			new room_manager.take_control(),
			fade.create_fade_out( 1.0f )
		);
	}
}

public enum room_type  { kitchen, lounge, bathroom, basement, bedroom }

public class room_kitchen : room
{
}

public class room_lounge : room
{
}

public class room_bathroom : room
{
}

public class room_basement : room
{
}

public class room_bedroom : room
{
}
