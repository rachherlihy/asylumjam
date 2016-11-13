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
			//new player.show_player( false ),
			//new menu_manager.show_splash( true ),
			//new pause( 0.5f ),
			//fade.create_fade_in( 1.5f ),
			//new pause( 2.0f ),
			//fade.create_fade_out( 1.5f ),
			//new menu_manager.show_splash( false ),
			//new goto_menu()
			//new io_manager.input_data(),
			new room_manager.start_game()
        );
	}

	public class goto_menu : state
	{
		public override void update()
		{
			state_manager.add_queue(
				new menu_manager.show_menu( true ),
				fade.create_fade_in( 0.5f ),
				new menu_manager.wait_for_any_key(),
				fade.create_fade_out( 0.5f ),
				new menu_manager.show_menu( false ),
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
