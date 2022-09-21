using UnityEngine;

public class TopDownAimAssist : AimAssist
{
	protected override Vector2 GetLocation()
	{
		return new Vector2(transform.position.x, transform.position.z);
	}

	protected override float GetRotation()
	{
		return transform.eulerAngles.y;
	}
}
