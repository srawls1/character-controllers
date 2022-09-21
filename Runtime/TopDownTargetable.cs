using UnityEngine;

public class TopDownTargetable : Targetable
{
	public override Vector2 GetLocation()
	{
		return new Vector2(transform.position.x, transform.position.z);
	}
}
