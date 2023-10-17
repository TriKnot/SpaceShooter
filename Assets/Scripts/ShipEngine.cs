
using System;
using UnityEngine;
using UnityEngine.Serialization;

public class ShipEngine : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Transform _transform;
    
    [Header("Engine Values")] 
    [SerializeField] private float _thrustForce = 10f;
    [SerializeField] private float _pitchForce = 10f;
    [SerializeField] private float _yawForce = 10f;
    [SerializeField] private float _rollForce = 10f;

    [FormerlySerializedAs("_currentThrust")]
    [Header("Current Engine Values")] 
    [SerializeField] [Range(-1, 1)] private float _currentThrustInput = 0f;
    [FormerlySerializedAs("_currentPitch")] [SerializeField] [Range(-1, 1)] private float _currentPitchInput = 0f;
    [FormerlySerializedAs("_currentYaw")] [SerializeField] [Range(-1, 1)] private float _currentYawInput = 0f;
    [FormerlySerializedAs("_currentRoll")] [SerializeField] [Range(-1, 1)] private float _currentRollInput = 0f;

    [Header("Thrusters")]
    [SerializeField] private ParticleSystem _leftBackThruster;
    [SerializeField] private ParticleSystem _rightBackThruster;
    [SerializeField] private ParticleSystem _leftFrontThruster;
    [SerializeField] private ParticleSystem _rightFrontThruster;
    [SerializeField] private float _engineThrustEmissionRateMax = 25.0f;
    [SerializeField] private float _engineThrustEmissionLifeTimeMax = 3.5f;
    
    private Vector3 _velocity;
    private Vector3 _angularVelocity; // x = pitch, y = yaw, z = roll
    
    private bool _shouldStop = false;
    
    public bool ShouldDampenCurrentValues
    {
        set => _shouldStop = value;
    }

    private void Awake()
    {
        // Reset the ship
        _currentThrustInput = 0f;
        _currentPitchInput = 0f;
        _currentYawInput = 0f;
        _currentRollInput = 0f;
    }

    private void FixedUpdate()
    {
        UpdateVelocity();

        // Update the particle systems
        UpdateEngineParticles();
        
        // Move the ship
        MoveShip();
    }

    private void UpdateVelocity()
    {

        if (_shouldStop)
        {
            if(_velocity.magnitude < 0.1f)
            {
                _velocity = Vector3.zero;
                _angularVelocity = Vector3.zero;
                return;
            }
            
            // Calculate the target rotation based on the input values
            var targetRotation = Quaternion.LookRotation(_velocity, Vector3.up);
            targetRotation *= Quaternion.Euler(_currentPitchInput, _currentYawInput, _currentRollInput);

            // Calculate the rotation step based on your desired rotation speed
            var rotationStep = Time.deltaTime; // You need to define rotationSpeed

            // Interpolate between the current rotation and the target rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationStep);
            
            // Dampen the angular velocity
            _angularVelocity = Vector3.Lerp(_angularVelocity, Vector3.zero, Time.deltaTime);
            
            if(Quaternion.Angle(_transform.rotation, targetRotation) < 0.1f)
            {
                // Dampen the velocity
                _currentThrustInput = -1f;
            }
            
        }
        
        // Calculate the current angular velocity
        _angularVelocity = new Vector3(-_pitchForce * _currentPitchInput, _yawForce * _currentYawInput, -_rollForce * _currentRollInput);

        // Calculate the current velocity
        _velocity += _transform.forward * (_thrustForce * _currentThrustInput);
        
    }

    private void MoveShip()
    {
        // Rotate the ship
        transform.Rotate(_angularVelocity * Time.deltaTime, Space.Self);
        
        // Move the ship
        _transform.position += _velocity * Time.deltaTime;
        
        Debug.Log($" _velocity: {_velocity} ");
        Debug.Log($" _angularVelocity: {_angularVelocity} ");
    }

    private void UpdateEngineParticles()
    {
        // Calculate the emission rate and lifetime based on the current yaw
        // emissionRate += Mathf.Lerp(0, _engineThrustEmissionRateMax, Mathf.Abs(_currentYaw));
        // emissionLifeTime += Mathf.Lerp(0, _engineThrustEmissionLifeTimeMax, Mathf.Abs(_currentYaw));

        // Calculate the emission rate and lifetime based on the current thrust
        var emissionRate = Mathf.Lerp(0, _engineThrustEmissionRateMax, Mathf.Abs(_currentThrustInput)); // Use the absolute value of _currentThrust
        var emissionLifeTime = Mathf.Lerp(0, _engineThrustEmissionLifeTimeMax, Mathf.Abs(_currentThrustInput));

        // Determine the thrust direction (1 for positive, -1 for negative)
        var thrustDirection = Mathf.Sign(_currentThrustInput);
        
        // Calculate yaw emission rate and lifetime
        var yawEmissionRate = Mathf.Lerp(0, _engineThrustEmissionRateMax, Mathf.Abs(_currentYawInput));
        var yawEmissionLifeTime = Mathf.Lerp(0, _engineThrustEmissionLifeTimeMax, Mathf.Abs(_currentYawInput));
        
        // yawDirection is 1 if _currentYaw is positive, -1 if _currentYaw is negative
        var yawDirection = Mathf.Sign(_currentYawInput);

        // Update the particle systems
        // For the right thrusters
        _rightFrontThruster.emissionRate = Mathf.Clamp(emissionRate * -thrustDirection, 0, _engineThrustEmissionRateMax * 0.7f) + Mathf.Clamp(yawEmissionRate * yawDirection, 0, _engineThrustEmissionRateMax * 0.3f); 
        _rightFrontThruster.startLifetime = Mathf.Clamp(emissionLifeTime * -thrustDirection, 0, _engineThrustEmissionLifeTimeMax * 0.7f) + Mathf.Clamp(yawEmissionLifeTime * yawDirection, 0, _engineThrustEmissionLifeTimeMax * 0.3f); 
        _rightBackThruster.emissionRate = Mathf.Clamp(emissionRate * thrustDirection, 0, _engineThrustEmissionRateMax * 0.7f) + Mathf.Clamp(yawEmissionRate * -yawDirection, 0, _engineThrustEmissionRateMax * 0.3f);
        _rightBackThruster.startLifetime = Mathf.Clamp(emissionLifeTime * thrustDirection, 0, _engineThrustEmissionLifeTimeMax * 0.7f) + Mathf.Clamp(yawEmissionLifeTime * -yawDirection, 0, _engineThrustEmissionLifeTimeMax * 0.3f);

        // For the left thrusters
        _leftFrontThruster.emissionRate = Mathf.Clamp(emissionRate * -thrustDirection, 0, _engineThrustEmissionRateMax * 0.7f) + Mathf.Clamp(yawEmissionRate * -yawDirection, 0, _engineThrustEmissionRateMax * 0.3f); 
        _leftFrontThruster.startLifetime = Mathf.Clamp(emissionLifeTime * -thrustDirection, 0, _engineThrustEmissionLifeTimeMax * 0.7f) + Mathf.Clamp(yawEmissionLifeTime * -yawDirection, 0, _engineThrustEmissionLifeTimeMax * 0.3f); 
        _leftBackThruster.emissionRate = Mathf.Clamp(emissionRate * thrustDirection, 0, _engineThrustEmissionRateMax * 0.7f) + Mathf.Clamp(yawEmissionRate * yawDirection, 0, _engineThrustEmissionRateMax * 0.3f);
        _leftBackThruster.startLifetime = Mathf.Clamp(emissionLifeTime * thrustDirection, 0, _engineThrustEmissionLifeTimeMax * 0.7f) + Mathf.Clamp(yawEmissionLifeTime * yawDirection, 0, _engineThrustEmissionLifeTimeMax * 0.3f);

    }
    
    public void SetThrust(float thrust)
    {
        _currentThrustInput = thrust;
    }
    
    public void SetPitch(float pitch)
    {
        _currentPitchInput = pitch;
    }
    
    public void SetYaw(float yaw)
    {
        _currentYawInput = yaw;
    }
    
    public void SetRoll(float roll)
    {
        _currentRollInput = roll;
    }
}
