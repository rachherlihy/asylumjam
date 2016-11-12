using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class room_sprites
{
	public room_type type;
	public Sprite floor_a;
	public Sprite floor_b;

	public Sprite wall;
	public Sprite door;
}

public class sprite_manager : MonoBehaviour
{
	public List<room_sprites> sprites;
	public Sprite glass;
	public Sprite glass_rim;
	public Sprite glass_shadow;
	
	public static sprite_manager instance;

	// Use this for initialization
	void Start ()
	{
		if ( instance != null )
		{
			DestroyObject( this.gameObject );
			return;
		}
		instance = this;
	}

	public room_sprites get_sprites( room_type type )
	{
		int i = 0;
		for ( ; i < this.sprites.Count && this.sprites[ i ].type != type; i++ );

		return i == this.sprites.Count ? null : this.sprites[ i ];
	}
}
