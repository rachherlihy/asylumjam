using UnityEngine;
using System.Collections;

public class main_menu : MonoBehaviour
{
	static public main_menu instance;
	SpriteRenderer sr;
	Animator anim;

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
		this.anim = this.GetComponent<Animator>();
	}

	public class show_menu : state
	{
		bool first = true;

		public override void update()
		{
			if ( this.first )
			{
				main_menu.instance.anim.Play( "menu_actual_initial" );
				main_menu.instance.anim.enabled = true;
				main_menu.instance.sr.enabled = true;

				this.first = false;
			}

			if ( !main_menu.instance.anim.GetCurrentAnimatorStateInfo( 0 ).IsName( "menu_actual_initial" ) )
			{
				this.completed = true;
			}
		}
	}

	public class hide_menu : state
	{
		public override void update()
		{
			main_menu.instance.sr.enabled = false;
			this.completed = true;
		}
	}
}
