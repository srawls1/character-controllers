using System.Collections.Generic;
using UnityEngine;

public class TargetableRegistry : Singleton<TargetableRegistry>
{
	[SerializeField] private Rect bounds;

	private List<Targetable> targetables;

	protected override TargetableRegistry GetThis()
	{
		return this;
	}

	protected override void Init()
	{
		targetables = new List<Targetable>();
	}

	public void RegisterTargetable(Targetable targetable)
	{
		targetables.Add(targetable);
	}

	public void UnregisterTargetable(Targetable targetable)
	{
		targetables.Remove(targetable);
	}

	public List<Targetable> GetTargetablesWithinBounds(Rect bounds)
	{
		List<Targetable> results = new List<Targetable>();
		GetTargetablesWithinBounds(bounds, results);
		return results;
	}

	public void GetTargetablesWithinBounds(Rect bounds, List<Targetable> results)
	{
		for (int i = 0; i < targetables.Count; ++i)
		{
			if (bounds.Contains(targetables[i].GetLocation()))
			{
				results.Add(targetables[i]);
			}
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawWireCube(new Vector3(bounds.center.x, bounds.center.y, bounds.center.y),
			new Vector3(bounds.size.x, bounds.size.y, bounds.size.y));
	}
}
