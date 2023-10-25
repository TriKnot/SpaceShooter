using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Ship
{
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
        [SerializeField] [Range(-1, 1)] private float _currentPitchInput = 0f;
        [SerializeField] [Range(-1, 1)] private float _currentYawInput = 0f;
        [SerializeField] [Range(-1, 1)] private float _currentRollInput = 0f;

        [Header("Thrusters")]
        [SerializeField] private ParticleSystem _leftBackThruster;
        [SerializeField] private ParticleSystem _rightBackThruster;
        [SerializeField] private ParticleSystem _leftFrontThruster;
        [SerializeField] private ParticleSystem _rightFrontThruster;
        [SerializeField] private Light _leftBackThrusterLight;
        [SerializeField] private Light _rightBackThrusterLight;
        [SerializeField] private Light _leftFrontThrusterLight;
        [SerializeField] private Light _rightFrontThrusterLight;        [SerializeField] private float _engineThrustEmissionRateMax = 25.0f;
        [SerializeField] private float _engineThrustEmissionLifeTimeMax = 3.5f;
        [SerializeField] private float _engineLightIntensityMax = 10.0f;
    
        private Vector3 _velocity;
        private Vector3 _angularVelocity;
    
        public bool ShouldDampenCurrentValues { set; get; }

        private void Awake()
        {
            ResetCurrentInputs();
        }

        private void FixedUpdate()
        {
            UpdateVelocity();
            UpdateEngineParticles();
        }

        private void Update()
        {
            MoveShip();
        }

        private void UpdateVelocity()
        {
            if (ShouldDampenCurrentValues)
            {
                DampenVelocity();
            }
        
            UpdateCurrentInputs();
        
            _angularVelocity = new Vector3(-_pitchForce * _currentPitchInput, _yawForce * _currentYawInput, -_rollForce * _currentRollInput);
        
            _velocity += _transform.forward * (_thrustForce * _currentThrustInput);
        }

        private void MoveShip()
        {
            if (_angularVelocity.magnitude > 0.1f)
            {
                // Rotate the ship
                _transform.Rotate(_angularVelocity * Time.deltaTime, Space.Self);
            }
        
            _transform.position += _velocity * Time.deltaTime;
        }
        
        private void UpdateEngineParticles()
        {
            // Calculate the emission rate and lifetime based on the current thrust
            float emissionRate = Mathf.Lerp(0, _engineThrustEmissionRateMax, Mathf.Abs(_currentThrustInput)); // Use the absolute value of _currentThrust
            float emissionLifeTime = Mathf.Lerp(0, _engineThrustEmissionLifeTimeMax, Mathf.Abs(_currentThrustInput));
            float emissionIntensity = Mathf.Lerp(0, _engineLightIntensityMax, Mathf.Abs(_currentThrustInput));

            // Determine the thrust direction (1 for positive, -1 for negative)
            float thrustDirection = Mathf.Sign(_currentThrustInput);
        
            // Calculate yaw emission rate and lifetime
            float yawEmissionRate = Mathf.Lerp(0, _engineThrustEmissionRateMax, Mathf.Abs(_currentYawInput));
            float yawEmissionLifeTime = Mathf.Lerp(0, _engineThrustEmissionLifeTimeMax, Mathf.Abs(_currentYawInput));
            float yawEmissionIntensity = Mathf.Lerp(0, _engineLightIntensityMax, Mathf.Abs(_currentYawInput));
        
            // yawDirection is 1 if _currentYaw is positive, -1 if _currentYaw is negative
            float yawDirection = Mathf.Sign(_currentYawInput);

            // Update the particle systems
            ParticleSystem.MainModule mainModule;
            ParticleSystem.EmissionModule emissionModule;
            // For the right thrusters
            mainModule = _rightFrontThruster.main;
            emissionModule = _rightFrontThruster.emission;
            emissionModule.rateOverTime = Mathf.Clamp(emissionRate * -thrustDirection, 0, _engineThrustEmissionRateMax * 0.7f) + Mathf.Clamp(yawEmissionRate * yawDirection, 0, _engineThrustEmissionRateMax * 0.3f); 
            mainModule.startLifetime = Mathf.Clamp(emissionLifeTime * -thrustDirection, 0, _engineThrustEmissionLifeTimeMax * 0.7f) + Mathf.Clamp(yawEmissionLifeTime * yawDirection, 0, _engineThrustEmissionLifeTimeMax * 0.3f); 
            _rightFrontThrusterLight.intensity = Mathf.Clamp(emissionIntensity * -thrustDirection, 0, _engineLightIntensityMax * 0.7f) + Mathf.Clamp(yawEmissionIntensity * yawDirection, 0, _engineLightIntensityMax * 0.3f);
            
            mainModule = _rightBackThruster.main;
            emissionModule = _rightBackThruster.emission;
            emissionModule.rateOverTime = Mathf.Clamp(emissionRate * thrustDirection, 0, _engineThrustEmissionRateMax * 0.7f) + Mathf.Clamp(yawEmissionRate * -yawDirection, 0, _engineThrustEmissionRateMax * 0.3f);
            mainModule.startLifetime = Mathf.Clamp(emissionLifeTime * thrustDirection, 0, _engineThrustEmissionLifeTimeMax * 0.7f) + Mathf.Clamp(yawEmissionLifeTime * -yawDirection, 0, _engineThrustEmissionLifeTimeMax * 0.3f);
            _rightBackThrusterLight.intensity = Mathf.Clamp(emissionIntensity * thrustDirection, 0, _engineLightIntensityMax * 0.7f) + Mathf.Clamp(yawEmissionIntensity * -yawDirection, 0, _engineLightIntensityMax * 0.3f);
            
            // For the left thrusters
            mainModule = _leftFrontThruster.main;
            emissionModule = _leftFrontThruster.emission;
            emissionModule.rateOverTime = Mathf.Clamp(emissionRate * -thrustDirection, 0, _engineThrustEmissionRateMax * 0.7f) + Mathf.Clamp(yawEmissionRate * -yawDirection, 0, _engineThrustEmissionRateMax * 0.3f); 
            mainModule.startLifetime = Mathf.Clamp(emissionLifeTime * -thrustDirection, 0, _engineThrustEmissionLifeTimeMax * 0.7f) + Mathf.Clamp(yawEmissionLifeTime * -yawDirection, 0, _engineThrustEmissionLifeTimeMax * 0.3f);
            _leftFrontThrusterLight.intensity = Mathf.Clamp(emissionIntensity * -thrustDirection, 0, _engineLightIntensityMax * 0.7f) + Mathf.Clamp(yawEmissionIntensity * -yawDirection, 0, _engineLightIntensityMax * 0.3f);
            
            mainModule = _leftBackThruster.main;
            emissionModule = _leftBackThruster.emission;
            emissionModule.rateOverTime = Mathf.Clamp(emissionRate * thrustDirection, 0, _engineThrustEmissionRateMax * 0.7f) + Mathf.Clamp(yawEmissionRate * yawDirection, 0, _engineThrustEmissionRateMax * 0.3f);
            mainModule.startLifetime = Mathf.Clamp(emissionLifeTime * thrustDirection, 0, _engineThrustEmissionLifeTimeMax * 0.7f) + Mathf.Clamp(yawEmissionLifeTime * yawDirection, 0, _engineThrustEmissionLifeTimeMax * 0.3f);
            _leftBackThrusterLight.intensity = Mathf.Clamp(emissionIntensity * thrustDirection, 0, _engineLightIntensityMax * 0.7f) + Mathf.Clamp(yawEmissionIntensity * yawDirection, 0, _engineLightIntensityMax * 0.3f);

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

        private void ResetCurrentInputs()
        {
            _currentThrustInput = 0f;
            _currentPitchInput = 0f;
            _currentYawInput = 0f;
            _currentRollInput = 0f;
        }

        private void UpdateCurrentInputs()
        {
            if (ShouldDampenCurrentValues)
            {
                if (_velocity.magnitude < 10.0f)
                {
                    ResetCurrentInputs();
                    return;
                }
            
                Quaternion targetRotation = Quaternion.LookRotation(_velocity, Vector3.up);
                Quaternion currentRotation = _transform.rotation;
                float forwardAngle = Quaternion.Angle(currentRotation, targetRotation);
                float backwardAngle = Quaternion.Angle(currentRotation, targetRotation * Quaternion.Euler(0, 180, 0));
                float thrustDirection = -1f;
                if (backwardAngle < forwardAngle)
                {
                    targetRotation *= Quaternion.Euler(0, 180, 0);
                    thrustDirection = 1f;
                }
                
                targetRotation *= Quaternion.Euler(_currentPitchInput, _currentYawInput, _currentRollInput);

                var rotationStep = Time.deltaTime;
                _transform.rotation = Quaternion.Slerp(_transform.rotation, targetRotation, rotationStep);
            
                _angularVelocity = Vector3.Lerp(_angularVelocity, Vector3.zero, Time.deltaTime);
            
                if (Quaternion.Angle(_transform.rotation, targetRotation) < 15.0f)
                {
                    thrustDirection *= Mathf.InverseLerp(0, 1000, _velocity.magnitude);
                    _currentThrustInput = thrustDirection;
                }
            }
        }

        private void DampenVelocity()
        {
            if (_velocity.magnitude < 10.0f)
            {
                _velocity = Vector3.zero;
                _angularVelocity = Vector3.zero;
                ResetCurrentInputs();
            }
        }
    }
}
