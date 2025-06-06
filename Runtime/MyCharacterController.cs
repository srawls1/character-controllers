using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator), typeof(RelativeTime))]
public class MyCharacterController : MonoBehaviour
{
	#region Editor Fields

	[SerializeField] private Camera m_camera;

	[Header("Movement")]
	[SerializeField] private float m_maxSpeed = 6f;
	[SerializeField] private float maxStrafeSpeed = 4f;
	[SerializeField] private float maxBackupSpeed = 3f;
	[SerializeField] private float timeToFullSpeed = 0.2f;
	[SerializeField] private float timeToStop = 0.1f;
	[SerializeField] private float orthogonalAccelerationTime = 0.5f;
	[SerializeField] private float rotationSmoothTime = 0.05f;
	[SerializeField] private bool autoRotation = true;

	[Header("Jumping")]
	[SerializeField] private float minJumpHeight = 2f;
	[SerializeField] private float maxJumpHeight = 5f;
	[SerializeField] private float horizontalDistanceBeforeMaxHeight = 5f;
	[SerializeField] private float horizontalDistanceAfterMaxHeight = 3f;
	[SerializeField] private float midairTimeToFullSpeed = 0.5f;
	[SerializeField] private float midairTimeToStop = 1f;
	[SerializeField] private float midairOrthogonalAccelerationTime = 2f;
	[SerializeField] private float coyoteTime = 0.2f;
	[SerializeField] private float maxAirJumpHeight = 3f;
	[SerializeField] private int m_numAirJumps = 0;
	[SerializeField] private float fallTimeout = 0.15f;
	[SerializeField] private float terminalVelocity = 53.0f;

	[Tooltip("Called upon leaving the ground on a jump. Intended for sound queues, particle effects, etc.")]
	[SerializeField] private UnityEvent onJump;
	[Tooltip("Called upon landing on the ground. Intended for sound queues, particle effects, etc.")]
	[SerializeField] private UnityEvent onLand;

	#endregion // Editor Fields

	#region Swizzlin

	public enum MovementAxis
	{
		X, Y, Z, None
	}

	[Header("Swizzling")]
	[SerializeField] private MovementAxis horizontalAxis1 = MovementAxis.X;
	[SerializeField] private MovementAxis horizontalAxis2 = MovementAxis.Z;
	[SerializeField] private MovementAxis verticalAxis = MovementAxis.Y;

	#endregion // Swizzlin

	#region Private Fields

	private float rotationVelocity = 0f;
	private float currentGravity;

	private float groundAcceleration;
	private float groundDeceleration;
	private float groundOrthogonalAcceleration;
	private float airAcceleration;
	private float airDeceleration;
	private float airOrthogonalAcceleration;
	private float jumpSpeed;
	private float airJumpSpeed;
	private float holdingJumpButtonGravity;
	private float releasedJumpButtonGravity;
	private float downwardFallingGravity;

	private float numAirJumpsDelta;
	private float fallTimeoutDelta;
	private float coyoteTimeDelta;

	private int animIDForwardSpeed;
	private int animIDStrafeSpeed;
	private int animIDUnsignedForwardSpeed;
	private int animIDUnsignedStrafeSpeed;
	private int animIDGrounded;
	private int animIDJump;
	private int animIDFreeFall;

	private PlayerInputProxy inputProxy;
	private RelativeTime relativeTime;
	private Animator animator;
	private CharacterMover mover;
	private GroundChecker groundChecker;

	#endregion // Private Fields

	#region Properties

	public Vector3 velocity
	{
		get
		{
			Vector3 horizontalComponent1 = GetSwizzledComponent(horizontalVelocity.x, horizontalAxis1);
			Vector3 horizontalComponent2 = GetSwizzledComponent(horizontalVelocity.y, horizontalAxis2);
			Vector3 verticalComponent = GetSwizzledComponent(verticalVelocity, verticalAxis);
			return horizontalComponent1 + horizontalComponent2 + verticalComponent;
		}
	}

	private Vector3 GetSwizzledComponent(float component, MovementAxis axis)
	{
		switch (axis)
		{
			case MovementAxis.X: return component * Vector3.right;
			case MovementAxis.Y: return component * Vector3.up;
			case MovementAxis.Z: return component * Vector3.forward;
			default: return Vector3.zero;
		}
	}

	private Vector2 m_horizontalVelocity;
	public Vector2 horizontalVelocity
	{
		get
		{
			return m_horizontalVelocity;
		}
		set
		{
			m_horizontalVelocity = value;
		}
	}

	private float m_verticalVelocity;
	public float verticalVelocity
	{
		get
		{
			return m_verticalVelocity;
		}
		set
		{
			m_verticalVelocity = value;
			if (!grounded)
			{
				if (value < 0)
				{
					currentGravity = downwardFallingGravity;
					animator.SetBool(animIDFreeFall, true);
				}
				else
				{
					animator.SetBool(animIDFreeFall, false);
				}
			}
		}
	}

	private bool m_grounded;
	public bool grounded
	{
		get
		{
			return m_grounded;
		}
		set
		{
			bool previouslyGrounded = m_grounded;
			m_grounded = value;
			if (m_grounded && !previouslyGrounded)
			{
				onLand.Invoke();
				animator.SetBool(animIDFreeFall, false);
			}

			if (m_grounded != previouslyGrounded)
			{
				animator.SetBool(animIDGrounded, grounded);
			}
		}
	}

	public float targetRotation
	{
		get; set;
	}

	public float maxSpeed
	{
		get { return m_maxSpeed; }
		set
		{
			m_maxSpeed = value;
			groundAcceleration = maxSpeed / timeToFullSpeed;
			groundDeceleration = maxSpeed / timeToStop;
			groundOrthogonalAcceleration = maxSpeed / orthogonalAccelerationTime;
			airAcceleration = maxSpeed / midairTimeToFullSpeed;
			airDeceleration = maxSpeed / midairTimeToStop;
			airOrthogonalAcceleration = maxSpeed / midairOrthogonalAccelerationTime;
		}
	}

	public int numAirJumps
	{
		get { return m_numAirJumps; }
		set
		{
			m_numAirJumps = value;
		}
	}

	new public Camera camera
	{
		get { return m_camera; }
		set
		{
			m_camera = value;
		}
	}

	#endregion // Properties

	#region Unity Functions

	private void Awake()
	{
		GetReferences();
		AssignAnimationIDs();
		DeriveMovementParameters();
		ResetCounters();
	}

	private void FixedUpdate()
	{
		VerticalUpdate();
		HorizontalUpdate();
		mover.Move(velocity * relativeTime.fixedDeltaTime);
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawRay(transform.position, transform.forward);
	}

	#endregion // Unity Functions

	#region Public Functions



	#endregion // Public Functions

	#region Private Functions

	private void GetReferences()
	{
		inputProxy = GetComponent<PlayerInputProxy>();
		animator = GetComponent<Animator>();
		mover = GetComponent<CharacterMover>();
		groundChecker = GetComponent<GroundChecker>();
		relativeTime = GetComponent<RelativeTime>();
	}

	private void AssignAnimationIDs()
	{
		animIDForwardSpeed = Animator.StringToHash("ForwardSpeed");
		animIDStrafeSpeed = Animator.StringToHash("StrafeSpeed");
		animIDUnsignedForwardSpeed = Animator.StringToHash("UnsignedForwardSpeed");
		animIDUnsignedStrafeSpeed = Animator.StringToHash("UnsignedStrafeSpeed");
		animIDGrounded = Animator.StringToHash("Grounded");
		animIDJump = Animator.StringToHash("Jump");
		animIDFreeFall = Animator.StringToHash("FreeFall");
	}

	private void DeriveMovementParameters()
	{
		groundAcceleration = maxSpeed / timeToFullSpeed;
		groundDeceleration = maxSpeed / timeToStop;
		groundOrthogonalAcceleration = maxSpeed / orthogonalAccelerationTime;
		airAcceleration = maxSpeed / midairTimeToFullSpeed;
		airDeceleration = maxSpeed / midairTimeToStop;
		airOrthogonalAcceleration = maxSpeed / midairOrthogonalAccelerationTime;

		float timeToMaxJumpHeight = horizontalDistanceBeforeMaxHeight / maxSpeed;
		jumpSpeed = 2 * maxJumpHeight / timeToMaxJumpHeight;
		holdingJumpButtonGravity = jumpSpeed / timeToMaxJumpHeight;

		float timeToMinJumpHeight = 2 * minJumpHeight / jumpSpeed;
		releasedJumpButtonGravity = jumpSpeed / timeToMinJumpHeight;

		float fallTime = horizontalDistanceAfterMaxHeight / maxSpeed;
		float fallingSpeed = 2 * maxJumpHeight / fallTime;
		downwardFallingGravity = fallingSpeed / fallTime;

		airJumpSpeed = Mathf.Sqrt(2 * maxAirJumpHeight * holdingJumpButtonGravity);

		currentGravity = downwardFallingGravity;
	}

	private void ResetCounters()
	{
		numAirJumpsDelta = numAirJumps;
		fallTimeoutDelta = fallTimeout;
		coyoteTimeDelta = coyoteTime;
	}

	private void VerticalUpdate()
	{
		grounded = groundChecker.IsGrounded();

		float deltaTime = relativeTime.fixedDeltaTime;

		if (grounded)
		{
			ResetCounters();

			// stop our velocity dropping infinitely when grounded
			if (verticalVelocity < 0f)
			{
				verticalVelocity = -1f;
			}

			// Jump
			if (inputProxy.Jump())
			{
				DoAJump(jumpSpeed);
			}
		}
		else
		{
			if (fallTimeoutDelta >= 0f)
			{
				fallTimeoutDelta -= deltaTime;
			}
			if (coyoteTimeDelta >= 0f)
			{
				coyoteTimeDelta -= deltaTime;

				if (inputProxy.Jump())
				{
					DoAJump(jumpSpeed);
				}
			}

			if (inputProxy.Jump() && numAirJumpsDelta > 0 && verticalVelocity < airJumpSpeed)
			{
				--numAirJumpsDelta;
				DoAJump(airJumpSpeed);
			}
		}

		if (verticalVelocity > -terminalVelocity)
		{
			verticalVelocity -= currentGravity * deltaTime;
		}
		else
		{
			verticalVelocity = -terminalVelocity;
		}
	}

	private void HorizontalUpdate()
	{
		Vector2 currentVelocity = horizontalVelocity;
		Vector2 targetVelocity = GetTargetVelocity();
		Vector2 parallelDiff, orthogonalDiff;

		if (currentVelocity == Vector2.zero)
		{
			parallelDiff = targetVelocity;
			orthogonalDiff = Vector2.zero;
		}
		else
		{
			Vector2 velocityDiff = targetVelocity - currentVelocity;
			parallelDiff = Vector2.Dot(velocityDiff, currentVelocity) * currentVelocity / currentVelocity.sqrMagnitude;
			orthogonalDiff = velocityDiff - parallelDiff;
		}

		float orthogonalSpeedChangeRate = grounded ?
			groundOrthogonalAcceleration :
			airOrthogonalAcceleration;
		float speedChangeRate = grounded ?
			(Vector2.Dot(parallelDiff, currentVelocity) > 0f ?
				groundAcceleration :
				groundDeceleration) :
			(Vector2.Dot(parallelDiff, currentVelocity) > 0f ?
				airAcceleration :
				airDeceleration);

		Vector2 delta = Vector2.MoveTowards(Vector2.zero, parallelDiff, speedChangeRate * relativeTime.fixedDeltaTime) +
			Vector2.MoveTowards(Vector2.zero, orthogonalDiff, orthogonalSpeedChangeRate * relativeTime.fixedDeltaTime);
		currentVelocity += delta;

		if (autoRotation && targetVelocity != Vector2.zero)
		{
			targetRotation = Mathf.Atan2(targetVelocity.x, targetVelocity.y) * Mathf.Rad2Deg;
		}

		float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity,
			rotationSmoothTime);
		transform.rotation = Quaternion.Euler(0f, rotation, 0f);

		horizontalVelocity = currentVelocity;

		Vector2 transformForward = new Vector2(transform.forward.x, transform.forward.z);
		Vector2 transformRight = new Vector2(transform.right.x, transform.right.z);
		float forwardSpeed = Vector3.Dot(currentVelocity, transformForward) / maxSpeed;
		float strafeSpeed = Vector3.Dot(currentVelocity, transformRight) / maxStrafeSpeed;
		animator.SetFloat(animIDForwardSpeed, forwardSpeed);
		animator.SetFloat(animIDStrafeSpeed, strafeSpeed);
		animator.SetFloat(animIDUnsignedForwardSpeed, Mathf.Abs(forwardSpeed));
		animator.SetFloat(animIDUnsignedStrafeSpeed, Mathf.Abs(strafeSpeed));
	}

	private void DoAJump(float speed)
	{
		animator.SetTrigger(animIDJump);
		inputProxy.ResetJump();
		verticalVelocity = Mathf.Max(speed, verticalVelocity);
		onJump.Invoke();
		StartCoroutine(JumpRoutine());
	}

	private IEnumerator JumpRoutine()
	{
		while (inputProxy.JumpHeld() && verticalVelocity > 0f)
		{
			currentGravity = holdingJumpButtonGravity;
			yield return null;
		}
		currentGravity = releasedJumpButtonGravity;
	}

	private Vector2 GetTargetVelocity()
	{
		Vector2 input = inputProxy.Movement();
		if (input.sqrMagnitude > 1f)
		{
			input.Normalize();
		}

		Camera cam = camera ?? Camera.main;
		Vector3 cameraRight = cam != null ? cam.transform.right : Vector3.right;
		Vector3 cameraForward = Vector3.Cross(cameraRight, Vector3.up);
		Vector3 input3D = (cameraRight * input.x) + (cameraForward * input.y);

		Vector3 forwardComponent = Vector3.Project(input3D, transform.forward);
		Vector3 sidewaysComponent = input3D - forwardComponent;

		float forwardSpeed = Vector3.Dot(forwardComponent, transform.forward) > 0 ? maxSpeed : maxBackupSpeed;
		Vector3 targetVel3D = forwardComponent * forwardSpeed + sidewaysComponent * maxStrafeSpeed;
		return new Vector2(targetVel3D.x, targetVel3D.z);
	}

	#endregion // Private Functions
}
