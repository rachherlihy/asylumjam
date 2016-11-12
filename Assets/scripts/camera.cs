using UnityEngine;
using System.Collections;

public class camera : MonoBehaviour
{
	public class screen_shake : state
	{
		float duration;
		float current;

		float intensity;

		Vector3 origin = new Vector3( -0.25f, 0.18f, -10.0f );

		public screen_shake( float duration, float intensity )
		{
			this.duration = duration;
			this.intensity = intensity;
		}

		public override void update()
		{
			this.current = Mathf.Min( this.duration, this.current + Time.deltaTime );

			Camera.main.transform.position = this.origin + new Vector3( Random.Range( -intensity, intensity ), Random.Range( -intensity, +intensity ) );

			if ( this.current >= this.duration )
			{
				Camera.main.transform.position = this.origin;
				this.completed = true;
			}
		}
	}
}
