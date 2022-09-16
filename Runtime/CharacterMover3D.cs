using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterMover3D : MonoBehaviour, CharacterMover
{
    private CharacterController characterController;

	public Vector3 velocity
    {
        get => characterController.velocity;
    }

	public void Move(Vector3 movement)
	{
        characterController.Move(movement);
	}

	private void Awake()
	{
		characterController = GetComponent<CharacterController>();
	}
}
