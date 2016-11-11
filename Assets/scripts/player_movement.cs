using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;


public class player_movement : MonoBehaviour
{
	public bool has_control = false;

	public Text debug_text;

	public float speed = 5.0f;

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
		{direction.up, new Vector3(0.0f, 1.0f) },
		{direction.down, new Vector3(0.0f, -1.0f) },
		{direction.left, new Vector3(-1.0f, 0.0f) },
		{direction.right, new Vector3(1.0f, 0.0f) },
	};

	Rigidbody2D r2d2;

	// Use this for initialization
	void Start () {
		this.r2d2 = this.GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
		this.handle_keys();
		this.handle_movement();

		if ( this.debug_text != null )
		{
			this.debug_text.text = this.direction.ToString();
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
				}
				else
				{
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
}
