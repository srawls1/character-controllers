using UnityEngine;

public class GroundChecker2D : MonoBehaviour, GroundChecker
{
    [SerializeField] private float groundedOffset = 0.14f;

    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    [SerializeField] private float groundedRadius = 0.28f;
    [Tooltip("What layers the character uses as ground")]
    [SerializeField] private LayerMask groundLayers;

    public bool IsGrounded()
	{
        Vector2 circlePosition = transform.position + Vector3.down * groundedOffset;
        return Physics2D.OverlapCircle(circlePosition, groundedRadius, groundLayers) != null;
	}

	private void OnDrawGizmosSelected()
	{
		Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
		Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

		if (IsGrounded()) Gizmos.color = transparentGreen;
		else Gizmos.color = transparentRed;

		// when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
		Gizmos.DrawSphere(
			new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z),
			groundedRadius);
	}
}
