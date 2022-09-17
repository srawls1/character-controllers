using UnityEngine;

public class TwinStickRotationController : MonoBehaviour
{
	[SerializeField] private float deadzone = 0.5f;

    private float sqrDeadzone;

    private PlayerInputProxy inputProxy;
    private MyCharacterController characterController;
    private Transform mainCamera;

	private void Awake()
    {
        sqrDeadzone = deadzone * deadzone;

        inputProxy = GetComponent<PlayerInputProxy>();
		characterController = GetComponent<MyCharacterController>();
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
        float desiredRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
        characterController.targetRotation = desiredRotation;
    }
}
