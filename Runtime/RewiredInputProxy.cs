using UnityEngine;
using Rewired;

public class RewiredInputProxy : PlayerInputProxy
{
	#region Editor Fields

	[SerializeField] private string horizontalAxisName = "Horizontal";
	[SerializeField] private string verticalAxisName = "Vertical";
	[SerializeField] private string horizontalLookAxisName = "HorizontalLook";
	[SerializeField] private string verticalLookAxisName = "VerticalLook";
	[SerializeField] private string jumpAxisName = "Jump";
	[SerializeField] private float inputBufferTime = 0.1f;

	#endregion // Editor Fields

	#region Properties

	public Player player { get; set; }

	#endregion // Properties

	#region Unity Functions

	new protected void Start()
	{
		base.Start();

		if (player == null)
		{
			player = ReInput.players.GetPlayer(0);
		}
	}

	new protected void Update()
	{
		if (player == null)
		{
			return;
		}

		UpdateButtonValue(jumpAxisName, ref jumpPressed, ref jumpBufferTimeDelta);
	}

	#endregion // Unity Functions

	#region Public Functions

	public override bool JumpHeld()
	{
		if (player == null)
		{
			return false;
		}
		return player.GetButton(jumpAxisName);
	}

	public override Vector2 Movement()
	{
		if (player == null)
		{
			return Vector2.zero;
		}

		return player.GetAxis2DRaw(horizontalAxisName, verticalAxisName);
	}

	public override Vector2 Look()
	{
		if (player == null)
		{
			return Vector2.zero;
		}

		return player.GetAxis2DRaw(horizontalLookAxisName, verticalLookAxisName);
	}

	#endregion // Public Functions

	#region Private Functions

	protected void UpdateButtonValue(string axisName, ref bool pressed, ref float bufferTimeDelta)
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
			pressed = player.GetButtonDown(axisName);
		}
	}

	#endregion // Private Functions
}
