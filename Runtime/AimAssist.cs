using System.Collections.Generic;
using UnityEngine;

public abstract class AimAssist : MonoBehaviour
{
	[SerializeField] private AnimationCurve correctionFunction;

	public float MapInputAngleToAimAngle(float range, float maxCorrectionAngle, float input)
	{
		Rect bounds = new Rect(GetLocation() - new Vector2(range, range), new Vector2(2 * range, 2 * range));
		List<Targetable> enemies = TargetableRegistry.instance.GetTargetablesWithinBounds(bounds);
		Targetable targetEnemy = null;
		float closestDiffFromInput = maxCorrectionAngle;
		float closestAngle = 0f;
		for (int i = 0; i < enemies.Count; ++i)
		{
			Targetable enemy = enemies[i];

			Vector2 diff = enemy.GetLocation() - GetLocation();
			if (diff.sqrMagnitude > range * range) continue;

			float angle = Mathf.Atan2(diff.x, diff.y) * Mathf.Rad2Deg;
			float diffFromInput = normalizeAngleClosestToZero(angle - input);

			if (Mathf.Abs(diffFromInput) > maxCorrectionAngle) continue;

			if (Mathf.Abs(diffFromInput) < Mathf.Abs(closestDiffFromInput))
			{
				closestDiffFromInput = diffFromInput;
				targetEnemy = enemy;
				closestAngle = angle;
			}
		}

		if (targetEnemy == null)
		{
			return input;
		}

		if (closestDiffFromInput < 0f)
		{
			float correctionFactor = correctionFunction.Evaluate(-closestDiffFromInput / maxCorrectionAngle);
			return Mathf.LerpAngle(closestAngle, closestAngle + maxCorrectionAngle, correctionFactor);
		}
		else
		{
			float correctionFactor = correctionFunction.Evaluate(closestDiffFromInput / maxCorrectionAngle);
			return Mathf.LerpAngle(closestAngle, closestAngle - maxCorrectionAngle, correctionFactor);
		}
	}

	private float normalizeAngleClosestToZero(float angle)
	{
		if (angle > 360f)
		{
			float rotations = Mathf.Floor(angle / 360f);
			angle -= rotations * 360f;
		}
		if (angle > 180f)
		{
			angle -= 360f;
		}
		if (angle < -360f)
		{
			float rotations = Mathf.Floor(-angle / 360f);
			angle += rotations * 360f;
		}
		if (angle < -180f)
		{
			angle += 360f;
		}

		return angle;
	}

	protected abstract Vector2 GetLocation();
	protected abstract float GetRotation();
}
