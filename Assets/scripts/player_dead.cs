using UnityEngine;
using System.Collections;

public class player_dead : MonoBehaviour {
	public static player_dead instance;
    public SpriteRenderer sr;

	// Use this for initialization
	void Start ()
	{
		if ( instance != null )
		{
			DestroyObject( this.gameObject );
			return;
		}

		instance = this;
		this.sr = this.GetComponent<SpriteRenderer>();
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
			if ( player.instance.COMPLETELY_FUCKED )
			{
				player_dead.instance.sr.enabled = false;
			}
			else
			{
				player_dead.instance.sr.enabled = this.show;
			}
			this.completed = true;
		}
	}
}
