using UnityEngine;
using System.Collections;

public class game_manager : MonoBehaviour
{
	public static game_manager instance;

	public bool has_won;
	public float time_remaining;

	// Use this for initialization
	void Start ()
	{
		if ( instance != null )
		{
			DestroyObject( this.gameObject );
			return;
		}

		instance = this;

		state_manager.add_queue(
			new player.show_player( false ),
			new menu_manager.show_splash( true ),
			new pause( 0.5f ),
			fade.create_fade_in( 1.5f ),
			new pause( 2.0f ),
			new menu_manager.show_splash( false ),
			new io_manager.input_data(),
			new goto_menu()
        );
	}

	void Update()
	{
		if ( Input.GetKey( KeyCode.Escape ) )
		{
			Application.Quit();
		}
	}

	public class goto_menu : state
	{
		public override void update()
		{
			state_manager.add_queue(
				new audio_mananger.play_intro(),
				new menu_manager.show_menu( true ),
				new main_menu.show_menu(),
				new audio_mananger.play_level(),
				new menu_manager.wait_for_any_key(),
				fade.create_fade_out( 0.5f ),
				new menu_manager.show_menu( false ),
				new main_menu.hide_menu(),
				new room_manager.start_game()
			);

			this.completed = true;
		}
	}

	public class set_has_won : state
	{
		public override void update()
		{
			game_manager.instance.has_won = true;
			player.instance.COMPLETELY_FUCKED = true;
			this.completed = true;
		}
	}
}
