using UnityEngine;
using System.Collections;

public class game_manager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		state_manager.add_queue(
			new menu_manager.show_splash( true ),
			new pause( 0.5f ),
			fade.create_fade_in( 1.5f ),
			new pause( 2.0f ),
			fade.create_fade_out( 1.5f ),
			new menu_manager.show_splash( false ),
			new goto_menu()
        );
	}

	class goto_menu : state
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
}
