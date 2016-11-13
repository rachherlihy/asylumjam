using UnityEngine;

public class state
{
	public bool completed = false;

	public virtual void update()
	{
	}
}


public class pause : state
{
	float duration;
	float current;

	public pause( float _duration )
	{
		this.duration = _duration;
	}

	public override void update()
	{
		this.current = Mathf.Min( this.duration, this.current + Time.deltaTime );

		if ( this.current >= this.duration )
		{
			this.completed = true;
		}
	}
}

public class tween : state
{
	GameObject go;
	Vector3 start;
	Vector3 detla;
	float duration;
	float current = 0.0f;

	public tween( GameObject _go, Vector3 _start, Vector3 _end, float _duration )
	{
		this.go = _go;
		this.start = _start;
		this.detla = _end - this.start;
		this.duration = _duration;
	}

	public override void update()
	{
		this.current = Mathf.Min( this.duration, this.current + Time.deltaTime );
		
		this.go.transform.position = this.start + this.detla / this.duration * this.current;

		if ( this.current >= this.duration )
		{
			this.completed = true;
		}
	}
}