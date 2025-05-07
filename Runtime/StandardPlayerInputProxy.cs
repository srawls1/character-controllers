using UnityEngine;

#if !ENABLE_INPUT_SYSTEM
public class StandardPlayerInputProxy : PlayerInputProxy
{
	#region Editor Fields

	[SerializeField] private string horizontalAxisName = "Horizontal";
	[SerializeField] private string verticalAxisName = "Vertical";
	[SerializeField] private string lookHorizontalAxisName = "LookHorizontal";
	[SerializeField] private string lookVerticalAxisName = "LookVertical";

	#endregion // Editor Fields

	#region Public Functions

	public override Vector2 Look()
	{
		return GetVectorValue(lookHorizontalAxisName, lookVerticalAxisName);
	}

	public override Vector2 Movement()
	{
		return GetVectorValue(horizontalAxisName, verticalAxisName);
	}

	#endregion // Public Functions

	#region Private Functions

	protected float GetAxisValue(string axisName)
	{
		return Input.GetAxisRaw(axisName);
	}

	protected Vector2 GetVectorValue(string xAxis, string yAxis)
	{
		return new Vector2(GetAxisValue(xAxis), GetAxisValue(yAxis));
	}

	protected override bool ButtonDown(string buttonName)
	{
		return Input.GetButtonDown(buttonName);
	}

	protected override bool ButtonHeld(string buttonName)
	{
		return Input.GetButton(buttonName);
	}

	protected override bool ButtonReleased(string buttonName)
	{
		return Input.GetButtonUp(buttonName);
	}

	#endregion // Private Functions
}

#endif
