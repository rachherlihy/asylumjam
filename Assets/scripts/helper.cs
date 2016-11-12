using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public enum direction
{
	none, up, down, left, right
};

public class helper
{
	static float offset_x = 32.0f / 120.0f;
	static float offset_y = 24.0f / 120.0f;
	static float tile_size = 64.0f / 60.0f;
	static int tile_x = 10;
	static int tile_y = 7;

	static Dictionary<direction, direction> reverse_lookup = new Dictionary<direction, direction>()
	{
		{direction.none, direction.none },
		{direction.up, direction.down },
		{direction.down, direction.up },
		{direction.left, direction.right },
		{direction.right, direction.left },
	};


	public static direction inverse_direction( direction _direction )
	{
		return reverse_lookup[ _direction ];
	}

	public static room create_random_room()
	{
		switch ( random_enum_excluding( room_type.room, room_type.start, room_type.end ) )
		{
			case room_type.basement:
				return new room_basement();
			case room_type.bathroom:
				return new room_bathroom();
			case room_type.bedroom:
				return new room_bedroom();
			case room_type.kitchen:
				return new room_kitchen();
			case room_type.lounge:
				return new room_lounge();
			default:
				return new room();
		}
	}

	public static T random_enum<T>()
	{
		var v = System.Enum.GetValues( typeof( T ) );
		return ( T ) v.GetValue( Random.Range( 0, v.Length ) );
	}

	public static T random_enum_excluding<T>( params T[] excluding )
	{
		var v = System.Enum.GetValues( typeof( T ) ).Cast<T>().ToList();
		var r = v.Except( excluding ).ToList();

		return ( T ) r[ Random.Range( 0, r.Count() ) ];
	}

	public static Vector3 tile_to_world( int x, int y )
	{
		float hwidth = ( tile_x * tile_size ) / 2.0f;
		float hheight = ( tile_y * tile_size ) / 2.0f;
		float hsize = tile_size / 2.0f;

		return new Vector3( x * tile_size - hwidth + hsize - offset_x, y * tile_size - hheight + hsize + offset_y );
	}

	public static Vector2 world_to_tile( Vector3 p )
	{
		float hwidth = ( tile_x * tile_size ) / 2.0f;
		float hheight = ( tile_y * tile_size ) / 2.0f;
		float hsize = tile_size / 2.0f;

		return new Vector2(
			Mathf.RoundToInt( round_tile_space( p.x + hwidth - hsize + offset_x ) / tile_size ),
			Mathf.RoundToInt( round_tile_space( p.y + hheight - hsize - offset_y ) / tile_size )
		);
	}

	public static float round_tile_space( float _value )
	{
		return tile_size * Mathf.Round( _value / tile_size );
	}
}