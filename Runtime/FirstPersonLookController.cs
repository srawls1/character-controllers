using UnityEngine;

public class FirstPersonLookController : MonoBehaviour
{
    [SerializeField] private float sensitivity = 30f;
	[SerializeField] private float minPitch = -89;
	[SerializeField] private float maxPitch = 89;
	[SerializeField] private Transform cameraTransform;

	private float pitch;

	private PlayerInputProxy inputProxy;
	private MyCharacterController characterController;

	private void Awake()
	{
		inputProxy = GetComponent<PlayerInputProxy>();
		characterController = GetComponent<MyCharacterController>();
	}

	private void Update()
    {
		Vector2 lookInput = inputProxy.Look();
		characterController.targetRotation += lookInput.x * sensitivity;

		pitch = Mathf.Clamp(pitch - lookInput.y * sensitivity, minPitch, maxPitch);
		cameraTransform.localRotation = Quaternion.Euler(pitch, 0, 0);
    }
}
