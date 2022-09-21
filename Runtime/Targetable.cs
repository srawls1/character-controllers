using UnityEngine;

public abstract class Targetable : MonoBehaviour
{
	[SerializeField] private bool m_positionDynamic;

	public bool positionDynamic
	{
		get { return m_positionDynamic; }
	}

	private void OnEnable()
	{
		TargetableRegistry.instance.RegisterTargetable(this);
	}

	private void OnDisable()
	{
		TargetableRegistry.instance.UnregisterTargetable(this);
	}

	public abstract Vector2 GetLocation();
}
