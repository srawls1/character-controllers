using UnityEngine;

#if !ENABLE_INPUT_SYSTEM
public class StandardPlayerInputProxy : PlayerInputProxy
{
	[SerializeField] private string jumpAxisName = "Jump";
	[SerializeField] private string horizontalAxisName = "Horizontal";
	[SerializeField] private string verticalAxisName = "Vertical";
	[SerializeField] private string lookHorizontalAxisName = "LookHorizontal";
	[SerializeField] private string lookVerticalAxisName = "LookVertical";

	public override bool JumpHeld()
	{
		return Input.GetButton(jumpAxisName);
	}

	public override Vector2 Look()
	{
		return GetVectorValue(lookHorizontalAxisName, lookVerticalAxisName);
	}

	public override Vector2 Movement()
	{
		return GetVectorValue(horizontalAxisName, verticalAxisName);
	}

	protected float GetAxisValue(string axisName)
	{
		return Input.GetAxisRaw(axisName);
	}

	protected Vector2 GetVectorValue(string xAxis, string yAxis)
	{
		return new Vector2(GetAxisValue(xAxis), GetAxisValue(yAxis));
	}

	new protected void Update()
	{
		base.Update();
		if (!jumpPressed)
		{
            jumpPressed = Input.GetButtonDown(jumpAxisName);
		}
	}
}

#endif
