using UnityEngine;

public interface IMovement
{
    void Move();
    Vector3 GetPosition();
    Vector3 GetVelocity();
}