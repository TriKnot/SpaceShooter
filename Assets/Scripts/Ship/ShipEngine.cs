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
        [SerializeField] private Light _rightFrontThrusterLight;
        [SerializeField] private float _engineThrustEmissionRateMax = 25.0f;
        [SerializeField] private float _engineThrustEmissionLifeTimeMax = 3.5f;
        [SerializeField] private float _engineLightIntensityMax = 10.0f;
    
        private Vector3 _velocity;
        private Vector3 _angularVelocity; // x = pitch, y = yaw, z = roll
    
        private bool _shouldStop = false;
    
        public bool ShouldDampenCurrentValues
        {
            set
            {
                _shouldStop = value;
                if (!value)
                    _currentThrustInput = 0;
            }
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
            
                if(_velocity.magnitude < 10.0f)
                {
                    _velocity = Vector3.zero;
                    _angularVelocity = Vector3.zero;
                    _currentThrustInput = 0f;
                    return;
                }
            
                // Calculate the target rotation based on the input values
                var targetRotation = Quaternion.LookRotation(-_velocity, Vector3.up);
                targetRotation *= Quaternion.Euler(_currentPitchInput, _currentYawInput, _currentRollInput);

                // Calculate the rotation step based on your desired rotation speed
                var rotationStep = Time.deltaTime; // You need to define rotationSpeed

                // Interpolate between the current rotation and the target rotation
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationStep);
            
                // Dampen the angular velocity
                _angularVelocity = Vector3.Lerp(_angularVelocity, Vector3.zero, Time.deltaTime);
            
                if(Quaternion.Angle(_transform.rotation, targetRotation) < 5.0f)
                {
                    // Dampen the velocity
                    _currentThrustInput = 1f;
                }
            
            }
        
            // Calculate the current angular velocity
            _angularVelocity = new Vector3(-_pitchForce * _currentPitchInput, _yawForce * _currentYawInput, -_rollForce * _currentRollInput);

            // Calculate the current velocity
            _velocity += _transform.forward * (_thrustForce * _currentThrustInput);
        
        }

        private void MoveShip()
        {
            if(_angularVelocity.magnitude > 0.1f)
            {
                // Rotate the ship
                transform.Rotate(_angularVelocity * Time.deltaTime, Space.Self);
            }
        
            // Move the ship
            _transform.position += _velocity * Time.deltaTime;
        
        }

        private void UpdateEngineParticles()
        {
            // Calculate the emission rate and lifetime based on the current yaw
            // emissionRate += Mathf.Lerp(0, _engineThrustEmissionRateMax, Mathf.Abs(_currentYaw));
            // emissionLifeTime += Mathf.Lerp(0, _engineThrustEmissionLifeTimeMax, Mathf.Abs(_currentYaw));

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
    }
}
