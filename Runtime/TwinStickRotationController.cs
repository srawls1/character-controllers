using UnityEngine;

public class TwinStickRotationController : MonoBehaviour
{
	[SerializeField] private float deadzone = 0.5f;
    [SerializeField] private float aimRange = 20f;
    [SerializeField] private float maxCorrectionAngle = 30f;

    private float sqrDeadzone;

    private PlayerInputProxy inputProxy;
    private MyCharacterController characterController;
    private AimAssist aimAssist;
    private Transform mainCamera;

	private void Awake()
    {
        sqrDeadzone = deadzone * deadzone;

        inputProxy = GetComponent<PlayerInputProxy>();
		characterController = GetComponent<MyCharacterController>();
        aimAssist = GetComponent<AimAssist>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").transform;
	}

	void Update()
    {
        Vector2 lookInput = inputProxy.Look();
        if (lookInput.sqrMagnitude < sqrDeadzone)
		{
            return;
		}

        Vector3 cameraRight = mainCamera.right;
        Vector3 cameraForward = Vector3.Cross(cameraRight, Vector3.up);
        Vector3 inputDirection =  (lookInput.x * cameraRight) + (lookInput.y * cameraForward);
        float inputRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
        float correctedRotation = aimAssist.MapInputAngleToAimAngle(aimRange, maxCorrectionAngle, inputRotation);
        characterController.targetRotation = correctedRotation;
    }
}
