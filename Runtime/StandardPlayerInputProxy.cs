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
		return new Vector2(Input.GetAxisRaw(lookHorizontalAxisName), Input.GetAxisRaw(lookVerticalAxisName));
	}

	public override Vector2 Movement()
	{
		return new Vector2(Input.GetAxisRaw(horizontalAxisName), Input.GetAxisRaw(verticalAxisName));
	}

	new private void Update()
	{
		base.Update();
		if (!jumpPressed)
		{
            jumpPressed = Input.GetButtonDown(jumpAxisName);
		}
	}
}

#endif
