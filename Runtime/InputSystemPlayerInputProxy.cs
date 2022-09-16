using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputSystemPlayerInputProxy : PlayerInputProxy
{
	private bool jumpHeld;
	private Vector2 movement;

	public void OnJump(InputValue value)
	{
		Debug.Log($"OnJump: isPressed={value.isPressed}");
		if (value.isPressed)
		{
			jumpHeld = true;
			jumpPressed = true;
		}
		else
		{
			jumpHeld = false;
		}
	}

	public void OnMove(InputValue value)
	{
		movement = value.Get<Vector2>();
	}

	public override bool JumpHeld()
	{
		return jumpHeld;
	}

	public override Vector2 Movement()
	{
		return movement;
	}
}

#endif
