using UnityEngine;
using System.Collections;

public class spider : MonoBehaviour
{
	static spider instance;

	// Use this for initialization
	void Start ()
	{
		if ( instance != null )
		{
			DestroyObject( this.gameObject );
			return;
		}

		instance = this;
	}

	public class drop_in : state
	{
		float time;
		float current = 0.0f;
		

		float height = 7.0f;
		Vector3 end = new Vector3(-0.26f, 2.60f);

		public drop_in( float time )
		{
			this.time = time;
		}

		public override void update()
		{
			this.current = Mathf.Min( this.time, this.current + Time.deltaTime );

			var height = this.height - ( this.height / this.time * this.current );
			spider.instance.transform.position = this.end + new Vector3( 0.0f, height );

			if ( this.current >= this.time )
			{
				this.completed = true;
			}
		}
	}


	public class bounce_a : state
	{
		float time;
		float current;

		float height;
		Vector3 end = new Vector3( -0.26f, 2.60f );

		public bounce_a( float height, float time )
		{
			this.height = height;
			this.time = time;
		}

		public override void update()
		{
			this.current = Mathf.Min( this.time, this.current + Time.deltaTime );

			var height = ( this.height / this.time * this.current );
			spider.instance.transform.position = this.end + new Vector3( 0.0f, height );

			if ( this.current >= this.time )
			{
				this.completed = true;
			}
		}
	}
	public class bounce_b : state
	{
		float time;
		float current;

		float height;
		Vector3 end = new Vector3( -0.26f, 2.60f );

		public bounce_b( float height, float time )
		{
			this.height = height;
			this.time = time;
		}

		public override void update()
		{
			this.current = Mathf.Min( this.time, this.current + Time.deltaTime );

			var height = this.height - ( this.height / this.time * this.current );
			spider.instance.transform.position = this.end + new Vector3( 0.0f, height );

			if ( this.current >= this.time )
			{
				this.completed = true;
			}
		}
	}
}
