using UnityEngine;

public abstract class PlayerInputProxy : MonoBehaviour
{
	#region Editor Fields

	[SerializeField] protected float inputBufferTime = 0.1f;
	[SerializeField] private string jumpAxisName = "Jump";

	#endregion // Editor Fields

	#region Private Fields

	protected bool jumpPressed;
    protected float jumpBufferTimeDelta;

	#endregion // Private Fields

	#region Unity Functions

	protected void Start()
    {
        jumpBufferTimeDelta = inputBufferTime;
        jumpPressed = false;
    }

    protected void Update()
    {
		UpdateButtonValue(jumpAxisName, ref jumpPressed, ref jumpBufferTimeDelta);
    }

	#endregion // Unity Functions

	#region Public Functions

	public bool Jump()
	{
        return jumpPressed;
    }

	public bool JumpHeld()
	{
		return ButtonHeld(jumpAxisName);
	}

    public void ResetJump()
	{
        jumpPressed = false;
	}

	#endregion // Public Functions

	#region Abstract Functions

    public abstract Vector2 Movement();
    public abstract Vector2 Look();

	protected abstract bool ButtonDown(string buttonName);
    protected abstract bool ButtonHeld(string buttonName);
    protected abstract bool ButtonReleased(string buttonName);

	protected void UpdateButtonValue(string buttonName, ref bool pressed, ref float bufferTimeDelta)
	{
		if (pressed)
		{
			bufferTimeDelta -= Time.deltaTime;
			if (bufferTimeDelta <= 0f)
			{
				bufferTimeDelta = 0f;
				pressed = false;
			}
		}
		else
		{
			bufferTimeDelta = inputBufferTime;
			pressed = ButtonDown(buttonName);
		}
	}

	#endregion // Abstract Functions
}
