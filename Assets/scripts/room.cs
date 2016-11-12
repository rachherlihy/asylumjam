using UnityEngine;
using System.Collections.Generic;

public class room
{
	protected room_type type = room_type.room;


	public Dictionary<direction, room> adjancent_rooms = new Dictionary<direction, room>()
	{
		{ direction.up, null },
		{ direction.down, null },
		{ direction.left, null },
		{ direction.right, null }
	};

	public Sprite wall;
	public Sprite door;

	public Sprite floor_sprite_a;
	public Sprite floor_sprite_b;
	public enum floor_state { a, b };

	public floor_state[, ] floor = new floor_state [10,7];
	bool generated = false;

	public virtual void on_load()
	{
		if ( !generated )
		{
			room_sprites rs = sprite_manager.instance.get_sprites( this.type );
			if ( rs != null )
			{
				this.floor_sprite_a = rs.floor_a;
				this.floor_sprite_b = rs.floor_b;
				this.wall = rs.wall;
				this.door = rs.door;
			}

			for ( int i = 0; i < 10; i++ )
			{
				for ( int j = 0; j < 7; j++ )
				{
					floor[ i, j ] = helper.random_enum<floor_state>();
				}
			}

			this.generated = true;
		}
	}

	public Sprite get_sprite( int x, int y )
	{
		return floor[ x, y ] == floor_state.a ? floor_sprite_a : floor_sprite_b;
	}

	public virtual void on_unload()
	{
	}

	public virtual void on_enter()
	{
	}

	public virtual void on_exit()
	{
	}
}

public class room_start : room
{
	public room_start()
	{
		this.type = room_type.start;
	}
}

public class room_end : room
{
	public room_end()
	{
		this.type = room_type.end;
	}
	
	public override void on_enter()
	{
		state_manager.add_queue(
			new room_manager.end_game()
		);
	}
}

public enum room_type  { room, start, end, kitchen, lounge, bathroom, basement, bedroom }

public class room_kitchen : room
{
	public room_kitchen()
	{
		this.type = room_type.kitchen;
	}
}

public class room_lounge : room
{
	public room_lounge()
	{
		this.type = room_type.lounge;
	}
}

public class room_bathroom : room
{
	public room_bathroom()
	{
		this.type = room_type.bathroom;
	}
}

public class room_basement : room
{
	public room_basement()
	{
		this.type = room_type.basement;
	}
}

public class room_bedroom : room
{
	public room_bedroom()
	{
		this.type = room_type.bedroom;
	}
}

