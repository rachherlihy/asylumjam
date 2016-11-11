using UnityEngine;
using System.Collections;

public class player : MonoBehaviour
{
	static player i;
	public player_movement pm;

	void Start()
	{
		if ( i != null )
		{
			DestroyObject( this.gameObject );
			return;
		}

		i = this;
		this.pm = this.GetComponent<player_movement>();
	}

	static public player instance
	{
		get
		{
			return i;
		}
	}
}
