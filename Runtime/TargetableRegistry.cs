using System.Collections.Generic;
using System.Linq;
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
		return targetables
			.Where((target) => bounds.Contains(target.GetLocation()))
			.ToList();
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawWireCube(new Vector3(bounds.center.x, bounds.center.y, bounds.center.y),
			new Vector3(bounds.size.x, bounds.size.y, bounds.size.y));
	}
}
