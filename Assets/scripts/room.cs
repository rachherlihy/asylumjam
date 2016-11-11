using UnityEngine;
using System.Collections.Generic;

public class room
{
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
