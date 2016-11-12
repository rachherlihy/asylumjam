using UnityEngine;
using System.Collections;

public class player : MonoBehaviour
{
	static public player instance;
	public player_movement pm;

	void Start()
	{
		if ( instance != null )
		{
			DestroyObject( this.gameObject );
			return;
		}

		instance = this;
		this.pm = this.GetComponent<player_movement>();
	}
}
