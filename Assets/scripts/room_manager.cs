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
	room start_room;
	room end_room;

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

		state_manager.add_queue(
			new generate_rooms( this, 5 ),
			new change_start_room(),
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
				new change_room( _room ),
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

	public class take_control : state
	{
		public override void update()
		{
			player.instance.pm.has_control = false;
			this.completed = true;
		}
	}

	public class give_control : state
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
		room to;

		public change_room( room _to )
		{
			this.to = _to;
		}


		public override void update()
		{
			if ( room_manager.instance != null )
			{
				if ( room_manager.instance.current_room != null )
				{
					room_manager.instance.current_room.on_exit();
					room_manager.instance.unload_room();
				}

				room_manager.instance.current_room = to;

				if ( room_manager.instance.current_room != null )
				{
					room_manager.instance.load_room();
					room_manager.instance.current_room.on_enter();
				}
			}

			this.completed = true;
		}
	}

	class change_start_room : state
	{
		public override void update()
		{
			var cr = new change_room( room_manager.instance.start_room );
			cr.update();

			this.completed = true;
		}
	}

	class generate_rooms : state
	{
		struct offset
		{
			public int x;
			public int y;

			public offset( int _x, int _y )
			{
				this.x = _x;
				this.y = _y;
			}
		}

		static Dictionary<direction, offset> offsets = new Dictionary<direction, offset>()
		{
			{direction.up, new offset( 0, 1 ) },
			{direction.down, new offset( 0, -1 ) },
			{direction.left, new offset( -1, 0 ) },
			{direction.right, new offset( 1, 0 ) },
		};

		room_manager rm;
		int size;

		public generate_rooms( room_manager _rm, int _size )
		{
			this.rm = _rm;
			this.size = _size;
		}

		bool on_grid( int x, int y )
		{
			return 0 <= x && x < this.size && 0 <= y && y < this.size;
		}

		public override void update()
		{
			int hsize = this.size / 2;
			room[ , ] rooms = new room[ this.size, this.size ];

			// Populate initial grid with rooms
			for ( int x = 0; x < this.size; x++ )
			{
				for ( int y = 0; y < this.size; y++ )
				{
					rooms[ x, y ] = helper.create_random_room();
				}
			}
			
			// Set the start room
			rooms[ hsize, hsize ] = new room_start();
			this.rm.start_room = rooms[ hsize, hsize ];
			Debug.Log( this.rm.start_room );

			// Calculate the position of the end room
			int fx = hsize, fy = hsize;
			while ( fx == hsize && fy == hsize )
			{
				fx = Random.Range( 0, this.size );
				fy = Random.Range( 0, this.size );
			}

			rooms[ fx, fy ] = new room_end();
			this.rm.end_room = rooms[ fx, fy ];

			{
				direction[, ] paths = new direction[ this.size, this.size ];

				int cx = hsize, cy = hsize;
				while ( cx != fx || cy != fy )
				{
					var dir = helper.random_enum_excluding( direction.none );

					var o = offsets[ dir ];
					var dx = cx + o.x;
					var dy = cy + o.y;

					if ( this.on_grid( dx, dy ) )
					{
						cx = dx;
						cy = dy;

						if ( paths[ cx, cy ] == direction.none )
						{
							paths[ cx, cy ] = dir;
						}
					}
				}

				while ( cx != hsize || cy != hsize )
				{
					var o = offsets[ helper.inverse_direction( paths[ cx, cy ] ) ];
					var dx = cx + o.x;
					var dy = cy + o.y;

					join_rooms( rooms[ dx, dy ], rooms[ cx, cy ], paths[ cx, cy ] );

					cx = dx;
					cy = dy;
				}
			}

			this.completed = true;
		}
	}
}
