using UnityEngine;
using System.Collections;
using System.IO;

[System.Serializable]
public class save_data
{
	public bool has_won;
	public float time_remaining;

	public save_data()
	{
		this.has_won = false;
		this.time_remaining = 0.0f;
	}

	public save_data( bool _has_won, float _time_remaining )
	{
		this.has_won = _has_won;
		this.time_remaining = _time_remaining;
	}
}

public class io_manager
{
	static public void output( save_data _output )
	{
		try
		{
			using ( StreamWriter sr = new StreamWriter( Application.persistentDataPath + "/this_is_racheals_fault.json" ) )
			{
				sr.Write( JsonUtility.ToJson( _output ) );
			}
		}
		catch {}
	}

	static public save_data input()
	{
		save_data data = new save_data();

		try
		{
			using ( StreamReader sr = new StreamReader( Application.persistentDataPath + "/this_is_racheals_fault.json" ) )
			{
				var i = sr.ReadToEnd();
				JsonUtility.FromJsonOverwrite( i, data );
			}
		}
		catch {}

		return data;
	}

	public class output_data : state
	{
		public override void update()
		{
			io_manager.output( new save_data( game_manager.instance.has_won, game_manager.instance.time_remaining ) );

			this.completed = true;
		}
	}

	public class input_data : state
	{
		public override void update()
		{
			var s = io_manager.input();
			game_manager.instance.has_won = s.has_won;
			game_manager.instance.time_remaining = s.time_remaining;

			this.completed = true;
		}
	}
}
