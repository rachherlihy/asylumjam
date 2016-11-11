using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public enum direction
{
	none, up, down, left, right
};

public class helper
{
	static Dictionary<direction, direction> reverse_lookup = new Dictionary<direction, direction>()
	{
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
		var v = System.Enum.GetValues( typeof( room_type ) );
		switch ( random_enum<room_type>() )
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

		return ( T ) r[Random.Range( 0, r.Count() )];
	}
}