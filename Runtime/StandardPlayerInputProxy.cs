using UnityEngine;

#if !ENABLE_INPUT_SYSTEM
public class StandardPlayerInputProxy : PlayerInputProxy
{
	[SerializeField] private string jumpAxisName = "Jump";
	[SerializeField] private string horizontalAxisName = "Horizontal";
	[SerializeField] private string verticalAxisName = "Vertical";

   

	public override bool Jump()
	{
		return jumpPressed;
	}

	public override bool JumpHeld()
	{
		return Input.GetButton(jumpAxisName);
	}

	public override Vector2 Movement()
	{
		return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
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
