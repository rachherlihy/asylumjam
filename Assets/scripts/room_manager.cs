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

	public Sprite sprite_door;
	public Sprite floor_a;
	public Sprite floor_b;

	public bool room_changing = false;

	SpriteRenderer[, ] tile_grid = new SpriteRenderer[ 10, 7 ];
	SpriteRenderer wall;

	void Start()
	{
		if ( instance != null )
		{
			DestroyObject( this.gameObject );
			return;
		}

		instance = this;
	}

	public bool on_tile( int x, int y )
	{
		return this.current_room == null ? false : this.current_room.floor[ x, y ] == room.floor_state.a;
	}

	void Update()
	{
		state_manager.update();
	}

	public void start()
	{
		this.doors[ direction.up ] = door.create( direction.up, new Vector3( -0.267f, 4.463f ), 0.0f );
		this.doors[ direction.down ] = door.create( direction.down, new Vector3( -0.267f, -4.07f ), 180.0f );
		this.doors[ direction.left ] = door.create( direction.left, new Vector3( -6.131f, 0.196f ), 90.0f );
		this.doors[ direction.right ] = door.create( direction.right, new Vector3( 5.6f, 0.196f ), 270.0f );

		// Create titles
		{
			var go = new GameObject( "Tiles" );

			for ( int i = 0; i < 10; i++ )
			{
				for ( int j = 0; j < 7; j++ )
				{
					var tile = new GameObject( "tile" );
					this.tile_grid[ i, j ] = tile.AddComponent<SpriteRenderer>();

					tile.transform.SetParent( go.transform );
					tile.transform.position = helper.tile_to_world( i, j );
				}
			}
		}

		// Create floor
		{
			var go = new GameObject( "wall" );
			this.wall = go.AddComponent<SpriteRenderer>();
		}

		state_manager.add_queue(
			new generate_rooms( this, 5 ),
			new change_start_room(),
			new player_movement.set_default_anim( "spidle_front" ),
			new player_movement.set_direction_position( direction.none ),
			fade.create_fade_in( 1.0f ),
			new give_control()
		);
	}

	public void end()
	{
		this.doors[ direction.up ] = null;
		this.doors[ direction.down ] = null;
		this.doors[ direction.left ] = null;
		this.doors[ direction.right ] = null;

		state_manager.add_queue(
			new take_control(),
			fade.create_fade_out( 1.0f )
		);
	}

	public static void join_rooms( room a, room b, direction a_connection )
	{
		a.adjancent_rooms[ a_connection ] = b;
		b.adjancent_rooms[ helper.inverse_direction( a_connection ) ] = a;
	}

	public void update_room(direction _direction = direction.none )
	{
		if ( !this.room_changing )
		{
			if ( this.current_room.adjancent_rooms[ _direction ] != null )
			{
				state_manager.add_queue(
					new lock_room_change( this, true ),
					new take_control(),
					fade.create_fade_out( 0.5f ),
					new change_room( this.current_room.adjancent_rooms[ _direction ] ),
					new player_movement.set_direction_position( helper.inverse_direction( _direction ) ),
					new player.reset_off_tile(),
					fade.create_fade_in( 0.5f ),
					new give_control(),
					new lock_room_change( this, false )
				);
			}			
		}
	}

	void load_room()
	{
		this.current_room.on_load();

		foreach ( KeyValuePair<direction, door> pair in this.doors )
		{
			pair.Value.set_to( this.current_room.adjancent_rooms[ pair.Key ] );
			pair.Value.sr.sprite = this.current_room.door;
		}

		for ( int i = 0; i < 10; i++ )
		{
			for ( int j = 0; j < 7; j++ )
			{
				tile_grid[ i, j ].sprite = this.current_room.get_sprite( i, j );
			}
		}

		this.wall.sprite = this.current_room.wall;
	}

	void unload_room()
	{
		this.current_room.on_unload();

		foreach ( KeyValuePair<direction, door> pair in this.doors )
		{
			pair.Value.set_to( null );
		}
	}

	public class start_game : state
	{
		public override void update()
		{
			room_manager.instance.start();
			this.completed = true;
		}
	}

	public class end_game : state
	{
		public override void update()
		{
			room_manager.instance.end();
			this.completed = true;
		}
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
		protected room to;

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

	class change_start_room : change_room
	{
		public change_start_room() :
			base( null )
		{
			// ...
		}
		public override void update()
		{
			this.to = room_manager.instance.start_room;
			base.update();
		}
	}

	public class set_player_position : state
	{
		Vector3 position;

		public set_player_position( Vector3 _position )
		{
			this.position = _position;
		}

		public override void update()
		{
			player.instance.transform.position = this.position;
			this.completed = true;
		}
	}

	public class set_player_animation : state
	{
		string animation;

		public set_player_animation( string animation )
		{
			this.animation = animation;
		}

		public override void update()
		{
			player.instance.pm.anim.Play( this.animation );
			this.completed = true;
		}
	}

	public class start_final_animation : state
	{
		public override void update()
		{
			state_manager.add_queue(
				new spider.drop_in( 1.5f ),
				new spider.bounce_a( 0.25f, 0.2f ),
				new spider.bounce_b( 0.25f, 0.3f ),
				new pause( 0.5f ),
				new camera.screen_shake( 1.0f, 0.5f )
			);

			this.completed = true;
		}
	}

	public class remove_doors : state
	{
		public override void update()
		{
			foreach ( KeyValuePair<direction, door> p in room_manager.instance.doors )
			{
				p.Value.set_to( null );
			}
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
			try
			{
				int hsize = this.size / 2;
				room[, ] rooms = new room[ this.size, this.size ];

				// Set the start room
				rooms[ hsize, hsize ] = new room_start();
				this.rm.start_room = rooms[ hsize, hsize ];

				// Calculate the position of the end room
				int fx = hsize, fy = hsize, distance = 0;
				while ( fx == hsize && fy == hsize || distance < 2)
				{
					fx = Random.Range( 0, this.size );
					fy = Random.Range( 0, this.size );

					distance = Mathf.RoundToInt( Mathf.Sqrt( (float)Mathf.Pow( fx - hsize, 2 ) + (float)Mathf.Pow( fy - hsize, 2 ) ) );
				}

				rooms[ fx, fy ] = new room_end();

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

				cx = fx;
				cy = fy;
				for ( int dx = cx, dy = cy; cx != hsize || cy != hsize; cx = dx, cy = dy )
				{
					var o = offsets[ helper.inverse_direction( paths[ cx, cy ] ) ];
					dx += o.x;
					dy += o.y;

					if ( rooms[ dx, dy ] == null )
					{
						rooms[ dx, dy ] = helper.create_random_room();
					}

					join_rooms( rooms[ dx, dy ], rooms[ cx, cy ], paths[ cx, cy ] );
				}

				cx = fx;
				cy = fy;
				for ( int dx = cx, dy = cy; cx != hsize || cy != hsize; cx = dx, cy = dy )
				{
					var o = offsets[ helper.inverse_direction( paths[ cx, cy ] ) ];
					dx += o.x;
					dy += o.y;

					// If random chance
					//	pick random adjanct / available node / not end room
					//		add room if null
					//		join rooms
					//		go to random chance

					for ( int rx = dx, ry = dy, px = dx, py = dy, max_rooms = 10; Random.Range( 0, 10 ) != 0 && max_rooms > 0; px = rx, py = ry, max_rooms-- )
					{
						var dir = helper.random_enum_excluding( direction.none );
						var ro = offsets[ dir ];
						rx += ro.x;
						ry += ro.y;

						if ( this.on_grid( rx, ry ) )
						{
							if ( rooms[ rx, ry ] != null )
							{
								if ( rooms[ rx, ry ].GetType() != typeof( room_end ) )
								{
									join_rooms( rooms[ px, py ], rooms[ rx, ry ], dir );
								}
							}
							else
							{
								rooms[ rx, ry ] = helper.create_random_room();
								join_rooms( rooms[ px, py ], rooms[ rx, ry ], dir );
							}
						}
					}
				}

				this.completed = true;
			}
			catch ( System.IndexOutOfRangeException )
			{
				// ...
			}
		}
	}
}
