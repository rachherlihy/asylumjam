using UnityEngine;
using System.Collections;

public class fade : MonoBehaviour
{
	static fade instance;

	SpriteRenderer sr;

	void Start()
	{
		if ( instance != null )
		{
			DestroyObject( instance );
			return;
		}

		instance = this;
		this.sr = this.GetComponent<SpriteRenderer>();
	}

	static public fade_in create_fade_in(float _time)
	{
		return new fade_in( instance, _time );
	}

	static public fade_out create_fade_out( float _time )
	{
		return new fade_out( instance, _time );
	}

	public class fade_in : state
	{
		fade f;
		float time;
		float current = 0.0f;

		public fade_in( fade _f, float _time )
		{
			this.f = _f;
			this.time = _time;
		}

		public override void update()
		{
			this.current += Time.deltaTime;

			var color = this.f.sr.color;
			color.a = 1.0f - Mathf.Min( 1.0f, ( 1.0f / this.time ) * this.current );

			this.f.sr.color = color;

			if ( this.current >= this.time )
			{
				this.completed = true;
			}
		}
	}

	public class fade_out : state
	{
		fade f;
		float time;
		float current = 0.0f;

		public fade_out( fade _f, float _time )
		{
			this.f = _f;
			this.time = _time;
		}

		public override void update()
		{			
			this.current += Time.deltaTime;

			var color = this.f.sr.color;
			color.a = Mathf.Min( 1.0f, ( 1.0f / this.time ) * this.current );

			this.f.sr.color = color;

			if ( this.current >= this.time )
			{
				this.completed = true;
			}
		}
	}
}
