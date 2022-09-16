using UnityEngine;

public interface CharacterMover
{
    Vector3 velocity { get; }
    void Move(Vector3 movement);
}
