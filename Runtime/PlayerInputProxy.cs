using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerInputProxy : MonoBehaviour
{
    [SerializeField] protected float jumpBufferTime;

    protected bool jumpPressed;
    protected float jumpBufferTimeDelta;

    private void Start()
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

    public bool Jump()
	{
        return jumpPressed;
    }

    public void ResetJump()
	{
        jumpPressed = false;
	}

    public abstract bool JumpHeld();
    public abstract Vector2 Movement();
}
