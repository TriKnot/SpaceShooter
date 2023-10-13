
using UnityEngine;

public class ShipEngine : MonoBehaviour
{
    [Header("Engine Values")] 
    [SerializeField] private float _thrustForce = 10f;
    [SerializeField] private float _pitchForce = 10f;
    [SerializeField] private float _yawForce = 10f;
    [SerializeField] private float _rollForce = 10f;

    [Header("Current Engine Values")] 
    [SerializeField] [Range(-1, 1)] private float _currentThrust = 0f;
    [SerializeField] [Range(-1, 1)] private float _currentPitch = 0f;
    [SerializeField] [Range(-1, 1)] private float _currentYaw = 0f;
    [SerializeField] [Range(-1, 1)] private float _currentRoll = 0f;

    private Vector3 _velocity;
    private Vector3 _angularVelocity; // x = pitch, y = yaw, z = roll

    private void FixedUpdate()
    {
        // Calculate the current angular velocity
        _angularVelocity = new Vector3(-_pitchForce * _currentPitch, _yawForce * _currentYaw, -_rollForce * _currentRoll);
        
        // Rotate the ship
        transform.Rotate(_angularVelocity * Time.deltaTime, Space.Self);
        
        // Calculate the current velocity
        _velocity += transform.forward * (_thrustForce * _currentThrust);
        
        // Move the ship
        transform.position += _velocity * Time.deltaTime;

    }
}
