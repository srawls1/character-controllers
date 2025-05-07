#if REWIRED

using UnityEngine;
using Rewired;

// Note: This file is surrounded in an 'if' preprocessor to prevent it from breaking your
// compilation if you are not using Rewired for your input. If you wish to use rewired,
// go to Project Settings > Player > Script Compilation > Scripting Define Symbols, and
// add 'REWIRED' to the list. Be sure to click 'Apply'.

public class RewiredInputProxy : PlayerInputProxy
{
#region Editor Fields

	[SerializeField] private string horizontalAxisName = "Horizontal";
	[SerializeField] private string verticalAxisName = "Vertical";
	[SerializeField] private string horizontalLookAxisName = "HorizontalLook";
	[SerializeField] private string verticalLookAxisName = "VerticalLook";

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

#endregion // Unity Functions

#region Public Functions

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

	protected override bool ButtonDown(string buttonName)
	{
		return player.GetButtonDown(buttonName);
	}

	protected override bool ButtonHeld(string buttonName)
	{
		return player.GetButton(buttonName);
	}

	protected override bool ButtonReleased(string buttonName)
	{
		return player.GetButtonUp(buttonName);
	}

#endregion // Private Functions
}

#endif
