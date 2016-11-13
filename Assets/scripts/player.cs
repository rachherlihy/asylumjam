using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class player : MonoBehaviour
{
	static public player instance;
	public player_movement pm;

	GameObject go_glass;
	GameObject go_rim;
	GameObject go_shadow;
	SpriteRenderer sr_shadow;

	float off_tile_time = 0.0f;
	float max_off_time = 1.5f;

	float glass_height = 10.0f;
	float shadow_alpha = 0.75f;

	float scale_up = 0.25f;

	public direction door_direction;

	SpriteRenderer sr;

	void Start()
	{
		if ( instance != null )
		{
			DestroyObject( this.gameObject );
			return;
		}

		instance = this;
		this.pm = this.GetComponent<player_movement>();
		this.sr = this.GetComponent<SpriteRenderer>();

		this.go_glass = new GameObject( "glass" );
		this.go_glass.transform.SetParent( this.transform );
		{
			var sr = this.go_glass.AddComponent<SpriteRenderer>();
			sr.sprite = sprite_manager.instance.glass;
			sr.sortingLayerName = "foreground";
		}

		this.go_rim = new GameObject( "rim" );
		this.go_rim.transform.SetParent( this.transform );
		{
			var sr = go_rim.AddComponent<SpriteRenderer>();
			sr.sprite = sprite_manager.instance.glass_rim;
			sr.sortingLayerName = "background";
			sr.sortingOrder = 100;
		}

		this.set_glass_height();

		this.go_shadow = new GameObject( "shadow" );
		this.go_shadow.transform.position = new Vector3( 0.0f, -0.09f );
		this.go_shadow.transform.SetParent( this.transform );
		{
			this.sr_shadow = go_shadow.AddComponent<SpriteRenderer>();
			this.sr_shadow.sprite = sprite_manager.instance.glass_shadow;
			this.sr_shadow.sortingLayerName = "background";
			this.sr_shadow.sortingOrder = 99;
		}

		this.set_shadow();
	}

	public static void reset()
	{
		instance.off_tile_time = 0.0f;
		instance.set_glass_height();
		instance.set_shadow();
	}

	void set_glass_height()
	{
		var height = this.glass_height - ( this.glass_height / this.max_off_time * this.off_tile_time ) + this.glass_height / 5.0f;
		this.go_glass.transform.position = this.transform.position + new Vector3( 0.0f, height );
		this.go_rim.transform.position = this.transform.position + new Vector3( 0.0f, height );
	}

	void set_shadow()
	{
		var color = this.sr_shadow.color;
		color.a = this.shadow_alpha / this.max_off_time * this.off_tile_time;
		this.sr_shadow.color = color;

		var scale = this.scale_up - ( this.scale_up / this.max_off_time * this.off_tile_time ) + 1.0f;
		this.go_shadow.transform.localScale = new Vector3( scale, scale );
	}

	void Update()
	{
		if ( this.pm.has_control )
		{
			var p = helper.world_to_tile( this.transform.position );
			if ( room_manager.instance.on_tile( ( int ) p.x, ( int ) p.y ) )
			{
				this.off_tile_time = Mathf.Max( 0.0f, this.off_tile_time - Time.deltaTime );
			}
			else
			{
				this.off_tile_time = Mathf.Min( this.max_off_time, this.off_tile_time + Time.deltaTime );
			}

			this.set_glass_height();
			this.set_shadow();

			if ( this.off_tile_time == this.max_off_time )
			{
				state_manager.add_queue(
					new room_manager.take_control()
				);
			}
		}
	}

	public class show_player : state
	{
		bool show;

		public show_player( bool _show )
		{
			this.show = _show;
		}

		public override void update()
		{
			player.instance.sr.enabled = this.show;
			this.completed = true;
		}
	}

	public class reset_off_tile : state
	{
		public override void update()
		{
			player.reset();
			this.completed = true;
		}
	}
}
