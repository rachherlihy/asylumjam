using UnityEngine;
using System.Collections;

public class audio_mananger : MonoBehaviour
{
	static public audio_mananger instance;

	public AudioSource[] aus;
	public AudioClip spider_scream;
	public AudioClip title_screen;
	public AudioClip level_music;
	public AudioClip boss_music;

	// Use this for initialization
	void Start ()
	{
		if ( instance != null )
		{
			DestroyObject( this.gameObject );
			return;
		}

		instance = this;
		this.aus = this.GetComponents<AudioSource>();
	}

	public class play_one_shot : state
	{
		AudioClip clip;

		public play_one_shot( AudioClip clip )
		{
			this.clip = clip; 
		}

		public override void update()
		{
			audio_mananger.instance.aus[ 0 ].PlayOneShot( this.clip );
			this.completed = true;
		}
	};

	public class play_level : state
	{
		public override void update()
		{
			audio_mananger.instance.aus[ 2 ].Stop();
			audio_mananger.instance.aus[ 1 ].Play();
			audio_mananger.instance.aus[ 3 ].Stop();
			this.completed = true;
		}
	};

	public class play_boss : state
	{
		public override void update()
		{
			audio_mananger.instance.aus[ 1 ].Stop();
			audio_mananger.instance.aus[ 2 ].Play();
			audio_mananger.instance.aus[ 3 ].Stop();
			this.completed = true;
		}
	};

	public class play_intro : state
	{
		public override void update()
		{
			audio_mananger.instance.aus[ 1 ].Stop();
			audio_mananger.instance.aus[ 2 ].Stop();
			audio_mananger.instance.aus[ 3 ].Play();
			this.completed = true;
		}

	}

	public class stop_all : state
	{
		public override void update()
		{
			foreach ( var a in audio_mananger.instance.aus )
			{
				a.Stop();
			}

			this.completed = true;
		}

	};
}
