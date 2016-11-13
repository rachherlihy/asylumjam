using UnityEngine;
using System.Collections;

public class particle_manager : MonoBehaviour {

	public static particle_manager instance;

	public GameObject heart_a;
	public GameObject heart_b;
	public GameObject baby_spider;

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

	public class create_particle : state
	{
		GameObject _game_object;
		Vector3 _start;
		Vector3 _direction;
		float _lifetime;
		float _fadeout;
		float _speed;

		public create_particle( GameObject _game_object, Vector3 _start, Vector3 _direction, float _lifetime, float _fadeout, float _speed )
		{
			this._game_object = _game_object;
			this._start = _start;
			this._direction = _direction;
			this._lifetime = _lifetime;
			this._fadeout = _fadeout;
			this._speed = _speed;
		}

		public override void update()
		{
			particle.create( this._game_object, this._start, this._direction, this._lifetime, this._fadeout, this._speed );
			this.completed = true;
		}
	}

	public class particle : MonoBehaviour
	{
		public Vector3 direction;
		public float lifetime;
		public float fadeout;
		public float fadeout_max;
		public float speed;
		public SpriteRenderer sr;

		public static void create( GameObject particle, Vector3 _start, Vector3 _direction, float _lifetime, float _fadeout, float _speed )
		{
			var go = Instantiate( particle, _start, Quaternion.identity ) as GameObject;
			
			var p = go.AddComponent<particle>();
			p.direction = _direction;
			p.lifetime = _lifetime;
			p.speed = _speed;
			p.fadeout = _fadeout;
			p.fadeout_max = _fadeout;

			p.sr = go.GetComponent<SpriteRenderer>();
		}

		void Update()
		{
			if ( this.lifetime > 0.0f )
			{
				this.lifetime -= Time.deltaTime;
				this.transform.position += direction.normalized * this.speed;
			}
			else if ( this.fadeout > 0.0f )
			{
				this.fadeout = Mathf.Max( 0.0f, this.fadeout - Time.deltaTime );
				this.transform.position += direction.normalized * this.speed;

				var c = this.sr.color;
				c.a = 1.0f / this.fadeout_max * this.fadeout;

				this.sr.color = c;
			}
			else
			{
				DestroyImmediate( this.gameObject );
			}
		}
	}
}

