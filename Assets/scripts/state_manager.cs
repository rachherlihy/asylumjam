using UnityEngine;
using System.Collections.Generic;

public class state_manager
{
	static state current;
	static Queue<state> state_queue = new Queue<state>();

	static void update_current()
	{
		if ( ( current != null && current.completed ) || current == null )
		{
			if ( state_queue.Count > 0 )
			{
				current = state_queue.Dequeue();
			}
			else
			{
				current = null;
			}
		}
	}

	static public void add_queue( params state[] s )
	{
		foreach ( var i in s )
		{
			state_queue.Enqueue( i );
		}
		
		update_current();
	}

	static public void update()
	{
		if ( current != null )
		{
			current.update();

			update_current();
		}
	}
}
