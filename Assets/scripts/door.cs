using UnityEngine;
using System.Collections;

public class door : MonoBehaviour {

	public room_manager rm;
	public room to;
	public direction direction;

	static public Sprite texture;

	public SpriteRenderer sr;

	// Use this for initialization
	void Start ()
	{
		this.sr.sprite = room_manager.instance.sprite_door;
	}

	public static door create(direction _direction, Vector3 _position, float _rotation)
	{
		var go = new GameObject( "door_" + _direction.ToString() );
		var d = go.AddComponent<door>();
		d.direction = _direction;

		var bc = go.AddComponent<BoxCollider2D>();
		bc.size = new Vector2( 0.5f, 1.0f );
		bc.isTrigger = true;

		var sr = go.AddComponent<SpriteRenderer>();
		sr.sortingLayerName = "background";
		d.sr = sr;

		go.transform.position = _position;
		go.transform.Rotate( new Vector3( 0.0f, 0.0f, _rotation ) );

		return d;
	}

	void OnTriggerEnter2D(Collider2D _collider)
	{
		if ( _collider.gameObject.tag == "Player" && this.to != null )
		{
			room_manager.instance.update_room( this.to, this.direction );
		}
	}

	public void set_to( room _rm )
	{
		this.to = _rm;
		this.sr.enabled = this.to != null;
	}
}
