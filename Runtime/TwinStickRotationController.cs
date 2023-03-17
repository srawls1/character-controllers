using UnityEngine;

public class TwinStickRotationController : MonoBehaviour
{
	[SerializeField] private float deadzone = 0.5f;
    [SerializeField] private float aimRange = 20f;
    [SerializeField] private float maxCorrectionAngle = 30f;
    [SerializeField] private Camera m_camera;

    private float sqrDeadzone;

    private PlayerInputProxy inputProxy;
    private MyCharacterController characterController;
    private AimAssist aimAssist;

    new public Camera camera
	{
        get { return m_camera; }
        set
		{
            m_camera = value;
		}
	}

	private void Awake()
    {
        sqrDeadzone = deadzone * deadzone;

        inputProxy = GetComponent<PlayerInputProxy>();
		characterController = GetComponent<MyCharacterController>();
        aimAssist = GetComponent<AimAssist>();
	}

	void Update()
    {
        Vector2 lookInput = inputProxy.Look();
        if (lookInput.sqrMagnitude < sqrDeadzone)
		{
            return;
		}

        Camera cam = camera ?? Camera.main;
        Vector3 cameraRight = cam != null ? cam.transform.right : Vector3.right;
        Vector3 cameraForward = Vector3.Cross(cameraRight, Vector3.up);
        Vector3 inputDirection =  (lookInput.x * cameraRight) + (lookInput.y * cameraForward);
        float inputRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
        float correctedRotation = aimAssist.MapInputAngleToAimAngle(aimRange, maxCorrectionAngle, inputRotation);
        characterController.targetRotation = correctedRotation;
    }
}
