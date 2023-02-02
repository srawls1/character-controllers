using UnityEngine;

public abstract class PlayerInputProxy : MonoBehaviour
{
	#region Editor Fields

	[SerializeField] protected float jumpBufferTime;

	#endregion // Editor Fields

	#region Private Fields

	protected bool jumpPressed;
    protected float jumpBufferTimeDelta;

	#endregion // Private Fields

	#region Unity Functions

	protected void Start()
    {
        jumpBufferTimeDelta = jumpBufferTime;
        jumpPressed = false;
    }

    protected void Update()
    {
        if (jumpPressed)
        {
            jumpBufferTimeDelta -= Time.deltaTime;
            if (jumpBufferTimeDelta <= 0f)
            {
                jumpBufferTimeDelta = 0f;
                jumpPressed = false;
            }
        }
        else
		{
            jumpBufferTimeDelta = jumpBufferTime;
		}
    }

	#endregion // Unity Functions

	#region Public Functions

	public bool Jump()
	{
        return jumpPressed;
    }

    public void ResetJump()
	{
        jumpPressed = false;
	}

	#endregion // Public Functions

	#region Abstract Functions

	public abstract bool JumpHeld();
    public abstract Vector2 Movement();
    public abstract Vector2 Look();

	#endregion // Abstract Functions
}
