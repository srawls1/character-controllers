using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterMover2D : MonoBehaviour, CharacterMover
{
    private Rigidbody2D body;

	public Vector3 velocity
	{
		get => body.velocity;
	}

	public void Move(Vector3 movement)
	{
		body.MovePosition(movement);
	}

	private void Awake()
	{
		body = GetComponent<Rigidbody2D>();
	}
}
