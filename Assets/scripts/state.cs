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