using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;


public class player_movement : MonoBehaviour
{
	public bool has_control = false;
	
	public float speed = 5.0f;
	float prev = 0.0f;

	direction direction = direction.none;
	List<direction> input_queue = new List<direction>();

	Dictionary<direction, KeyCode> inputs = new Dictionary<direction, KeyCode>()
	{
		{direction.up, KeyCode.UpArrow },
		{direction.down, KeyCode.DownArrow },
		{direction.left, KeyCode.LeftArrow },
		{direction.right, KeyCode.RightArrow },
	};

	Dictionary<direction, Vector3> movement_offset = new Dictionary<direction, Vector3>()
	{
		{direction.none, new Vector3(0.0f, 0.0f) },
		{direction.up, new Vector3(0.0f, 1.0f) },
		{direction.down, new Vector3(0.0f, -1.0f) },
		{direction.left, new Vector3(-1.0f, 0.0f) },
		{direction.right, new Vector3(1.0f, 0.0f) },
	};

	Dictionary<direction, string> animation_idle = new Dictionary<direction, string>()
	{
		{direction.none, "spidle_back" },
		{direction.up, "spidle_back" },
		{direction.down, "spidle_front" },
		{direction.left, "spidle_left" },
		{direction.right, "spidle_right" },
	};

	Dictionary<direction, string> animation_walk = new Dictionary<direction, string>()
	{
		{direction.up, "spalk_back" },
		{direction.down, "spalk_front" },
		{direction.left, "spalk_left" },
		{direction.right, "spalk_right" },
	};

	public Rigidbody2D r2d2;
	public Animator anim;

	// Use this for initialization
	void Start()
	{
		this.r2d2 = this.GetComponent<Rigidbody2D>();
		this.anim = this.GetComponent<Animator>();
	}

	// Update is called once per frame
	void Update()
	{
		if ( !player.instance.COMPLETELY_FUCKED )
		{
			this.handle_keys();
			this.handle_movement();

			if ( Input.GetKeyDown( KeyCode.LeftShift ) )
			{
				this.prev = this.speed;
				this.speed = 0.25f;
			}
			else if ( Input.GetKeyUp( KeyCode.LeftShift ) )
			{
				this.speed = this.prev;
			}
		}
	}

	void handle_keys()
	{
		foreach ( KeyValuePair<direction, KeyCode> pair in this.inputs )
		{
			this.key_pressed( pair.Key, Input.GetKeyDown( pair.Value ) );
			this.key_released( pair.Key, Input.GetKeyUp( pair.Value ) );
		}
	}

	void key_pressed( direction _kp, bool _pressed )
	{
		if ( _pressed )
		{
			if ( this.direction == direction.none )
			{
				this.direction = _kp;
				this.anim.Play( this.animation_walk[ this.direction ] );
			}
			else
			{
				this.input_queue.Add( _kp );
			}
		}
	}

	void key_released( direction _kp, bool _released )
	{
		if ( _released )
		{
			if ( this.direction == _kp )
			{
				if ( this.input_queue.Count > 0 )
				{
					this.direction = this.input_queue[ 0 ];
					this.input_queue.RemoveAt( 0 );

					this.anim.Play( this.animation_walk[ this.direction ] );
				}
				else
				{
					this.anim.Play( this.animation_idle[ this.direction ] );
					this.direction = direction.none;
				}
			}
			else
			{
				this.input_queue.Remove( _kp );
			}

		}
	}

	void handle_movement()
	{
		if ( this.has_control && this.direction != direction.none )
		{
			var pos = this.transform.position + this.movement_offset[ this.direction ] * this.speed;
			this.r2d2.MovePosition( pos );

			this.anim.Play( this.animation_walk[ this.direction ] );

			if ( this.direction == player.instance.door_direction )
			{
				room_manager.instance.update_room( this.direction );
			}
		}
		else if ( !this.has_control )
		{
			this.anim.Play( this.animation_idle[ this.direction ] );
		}
	}

	public void invert_position( bool _horizontal )
	{
		var position = this.transform.position;
		if ( _horizontal )
		{
			this.transform.position = new Vector3( -position.x, 0.0f );
		}
		else
		{
			this.transform.position = new Vector3( 0.0f, -position.y );
		}
	}

	public class set_default_anim : state
	{
		string animation;

		public set_default_anim( string animation )
		{
			this.animation = animation;
		}

		public override void update()
		{
			player.instance.pm.animation_idle[ direction.none ] = this.animation;
			this.completed = true;
        }
	}

	public class set_direction_position : state
	{
		static Dictionary<direction, Vector3> positions = new Dictionary<direction, Vector3>()
		{
			{direction.none, new Vector3( -0.26f, 0.21f ) },
            {direction.up, new Vector3( -0.26f, 3.41f ) },
            {direction.down, new Vector3( -0.26f, -2.98f ) },
            {direction.left, new Vector3( -5.07f, 0.21f ) },
			{direction.right, new Vector3( 4.54f, 0.21f ) }
		};

		direction dir;

		public set_direction_position( direction _dir )
		{
			this.dir = _dir;
		}

		public override void update()
		{
			player.instance.transform.position = positions[ this.dir ];
			this.completed = true;
		}
	}
}
