using UnityEngine;
using System.Collections;

public class menu_manager : MonoBehaviour
{
	static menu_manager instance;

	SpriteRenderer sr_splash;
	SpriteRenderer sr_menu;

	// Use this for initialization
	void Start()
	{
		if ( instance != null )
		{
			DestroyObject( this.gameObject );
			return;
		}

		instance = this;

		{
			var go = new GameObject( "splash" );
			go.transform.position = new Vector3( -0.25f, 0.18f );
			this.sr_splash = go.AddComponent<SpriteRenderer>();
			this.sr_splash.sprite = sprite_manager.instance.splash;
			this.sr_splash.sortingLayerName = "ui";
			this.sr_splash.sortingOrder = 999;
			this.sr_splash.enabled = false;
		}

		{
			var go = new GameObject( "menu" );
			go.transform.position = new Vector3( -0.25f, 0.18f );
			this.sr_menu = go.AddComponent<SpriteRenderer>();
			this.sr_menu.sprite = sprite_manager.instance.menu;
			this.sr_menu.sortingLayerName = "ui";
			this.sr_menu.sortingOrder = 1000;
			this.sr_menu.enabled = false;
		}
	}

	public class wait_for_any_key : state
	{
		public override void update()
		{
			if ( Input.anyKey )
			{
				this.completed = true;
			}
		}
	}

	public class show_splash : state
	{
		bool show;

		public show_splash( bool _show )
		{
			this.show = _show;
		}

		public override void update()
		{
			menu_manager.instance.sr_splash.enabled = this.show;

			this.completed = true;
		}
	}

	public class show_menu : state
	{
		bool show;

		public show_menu( bool _show )
		{
			this.show = _show;
		}

		public override void update()
		{
			menu_manager.instance.sr_menu.enabled = this.show;

			this.completed = true;
		}
	}
}
