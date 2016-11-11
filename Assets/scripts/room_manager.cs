using UnityEngine;
using System.Collections.Generic;

public class room_manager : MonoBehaviour
{
	static public room_manager instance;

	Dictionary<direction, door> doors = new Dictionary<direction, door>()
	{
		{ direction.up, null },
		{ direction.down, null },
		{ direction.left, null },
		{ direction.right, null },
	};

	room current_room;

	SpriteRenderer sr_floor;

	public Sprite sprite_door;
	public Sprite floor_a;
	public Sprite floor_b;

	bool room_changing = false;

	void Start()
	{
		if ( instance != null )
		{
			DestroyObject( this.gameObject );
			return;
		}

		instance = this;

		this.doors[ direction.up ] = door.create( direction.up, new Vector3( 0.0f, 5.0f ) );
		this.doors[ direction.down ] = door.create( direction.down, new Vector3( 0.0f, -5.0f ) );
		this.doors[ direction.left ] = door.create( direction.left, new Vector3( -6.68f, 0.0f ) );
		this.doors[ direction.right ] = door.create( direction.right, new Vector3( 6.68f, 0.0f ) );

		this.sr_floor = this.GetComponent<SpriteRenderer>();

		var a = new room();
		var b = new room();

		join_rooms( a, b, direction.left );

		state_manager.add_queue(
			new change_room( this, a ),
			fade.create_fade_in( 1.0f ),
			new give_control()
		);
	}

	void Update()
	{
		state_manager.update();
	}

	public static void join_rooms( room a, room b, direction a_connection )
	{
		a.adjancent_rooms[ a_connection ] = b;
		b.adjancent_rooms[ helper.inverse_direction( a_connection ) ] = a;
	}

	public void update_room( room _room, direction _direction = direction.none )
	{
		if ( !this.room_changing )
		{
			state_manager.add_queue(
				new lock_room_change( this, true ),
				new take_control(),
				fade.create_fade_out( 0.5f ),
				new change_room( this, _room ),
				new invert_position( _direction ),
				fade.create_fade_in( 0.5f ),
				new give_control(),
				new lock_room_change( this, false )
			);
		}
	}

	void unload_room()
	{
		foreach ( KeyValuePair<direction, door> pair in this.doors )
		{
			pair.Value.set_to( null );
		}

		this.sr_floor.sprite = null;
    }

	void load_room()
	{
		foreach ( KeyValuePair<direction, door> pair in this.doors )
		{
			pair.Value.set_to( this.current_room.adjancent_rooms[ pair.Key ] );
		}

		this.sr_floor.sprite = this.current_room.floor_sprite;
	}

	class take_control : state
	{
		public override void update()
		{
			player.instance.pm.has_control = false;
			this.completed = true;
		}
	}

	class give_control : state
	{
		public override void update()
		{
			player.instance.pm.has_control = true;
			this.completed = true;
		}
	}

	class lock_room_change : state
	{
		room_manager rm;
		bool _lock;

		public lock_room_change( room_manager _rm, bool _lock )
		{
			this.rm = _rm;
			this._lock = _lock;
		}

		public override void update()
		{
			this.rm.room_changing = this._lock;
			this.completed = true;
		}
	}

	class invert_position : state
	{
		bool horizontal;

		public invert_position( bool _horizontal )
		{
			this.horizontal = _horizontal;
		}

		public invert_position( direction _direction )
		{
			this.horizontal = _direction == direction.left || _direction == direction.right;
        }

		public override void update()
		{
			player.instance.pm.invert_position( this.horizontal );
			this.completed = true;
		}
	}

	class change_room : state
	{
		room_manager rm;
		room to;

		public change_room( room_manager _rm, room _to )
		{
			this.rm = _rm;
			this.to = _to;
		}


		public override void update()
		{
			if ( rm != null )
			{
				if ( this.rm.current_room != null )
				{
					this.rm.current_room.on_exit();
					this.rm.unload_room();
				}

				this.rm.current_room = to;

				if ( this.rm.current_room != null )
				{
					this.rm.load_room();
					this.rm.current_room.on_enter();
				}
			}

			this.completed = true;
		}
	}
}
