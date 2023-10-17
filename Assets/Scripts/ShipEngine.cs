
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

    [Header("Current Engine Values")] 
    [SerializeField] [Range(-1, 1)] private float _currentThrust = 0f;
    [SerializeField] [Range(-1, 1)] private float _currentPitch = 0f;
    [SerializeField] [Range(-1, 1)] private float _currentYaw = 0f;
    [SerializeField] [Range(-1, 1)] private float _currentRoll = 0f;

    [Header("Thrusters")]
    [SerializeField] private ParticleSystem _leftBackThruster;
    [SerializeField] private ParticleSystem _rightBackThruster;
    [SerializeField] private ParticleSystem _leftFrontThruster;
    [SerializeField] private ParticleSystem _rightFrontThruster;
    [SerializeField] private float _engineThrustEmissionRateMax = 25.0f;
    [SerializeField] private float _engineThrustEmissionLifeTimeMax = 3.5f;
    
    private Vector3 _velocity;
    private Vector3 _angularVelocity; // x = pitch, y = yaw, z = roll

    private void Awake()
    {
        // Reset the ship
        _currentThrust = 0f;
        _currentPitch = 0f;
        _currentYaw = 0f;
        _currentRoll = 0f;
    }

    private void FixedUpdate()
    {
        // Move the ship
        MoveShip();
        
        // Update the particle systems
        UpdateEngineParticles();

    }

    private void MoveShip()
    {
        // Calculate the current angular velocity
        _angularVelocity = new Vector3(-_pitchForce * _currentPitch, _yawForce * _currentYaw, -_rollForce * _currentRoll);
        
        // Rotate the ship
        transform.Rotate(_angularVelocity * Time.deltaTime, Space.Self);
        
        // Calculate the current velocity
        _velocity += _transform.forward * (_thrustForce * _currentThrust);
        
        // Move the ship
        _transform.position += _velocity * Time.deltaTime;

    }

    private void UpdateEngineParticles()
    {
        // Calculate the emission rate and lifetime based on the current yaw
        // emissionRate += Mathf.Lerp(0, _engineThrustEmissionRateMax, Mathf.Abs(_currentYaw));
        // emissionLifeTime += Mathf.Lerp(0, _engineThrustEmissionLifeTimeMax, Mathf.Abs(_currentYaw));

        // Calculate the emission rate and lifetime based on the current thrust
        var emissionRate = Mathf.Lerp(0, _engineThrustEmissionRateMax, Mathf.Abs(_currentThrust)); // Use the absolute value of _currentThrust
        var emissionLifeTime = Mathf.Lerp(0, _engineThrustEmissionLifeTimeMax, Mathf.Abs(_currentThrust));

        // Determine the thrust direction (1 for positive, -1 for negative)
        var thrustDirection = Mathf.Sign(_currentThrust);
        
        // Calculate yaw emission rate and lifetime
        var yawEmissionRate = Mathf.Lerp(0, _engineThrustEmissionRateMax, Mathf.Abs(_currentYaw));
        var yawEmissionLifeTime = Mathf.Lerp(0, _engineThrustEmissionLifeTimeMax, Mathf.Abs(_currentYaw));
        
        // yawDirection is 1 if _currentYaw is positive, -1 if _currentYaw is negative
        var yawDirection = Mathf.Sign(_currentYaw);

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
}
