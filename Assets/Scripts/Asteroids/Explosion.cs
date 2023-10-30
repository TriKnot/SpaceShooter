using ScriptableObjects.Variables;
using UnityEngine;
using Util;
using Utils;

namespace Asteroids
{
    public class Explosion : MonoBehaviour, IPoolObject<Explosion>
    {
        [Header("Dependencies")]
        [SerializeField] private BoolVariableSO _usePoolingSO;
        [SerializeField] private ParticleSystem _particleSystems;
        [SerializeField] private Light _light;
        private ObjectPool<Explosion> _pool;
        private Transform _transform;
        
        [Header("Settings")]
        [SerializeField] private float _minimumSize = 15f;
        [SerializeField] private float _maximumSize = 300f;
        [SerializeField] private float _minimumTime = 0.25f;
        [SerializeField] private float _maximumTime = 1f;
        
        private float _duration;
        private float _startSizeMultiplier;

        private void Awake()
        {
            _transform = transform;
        }

        public void Explode(float scaleMultiplier, Vector3 position, Quaternion rotation)
        {
            _duration = Mathf.Lerp(_minimumTime, _maximumTime, Mathf.InverseLerp(1, 100, scaleMultiplier));
            _startSizeMultiplier = Mathf.Lerp(_minimumSize, _maximumSize, Mathf.InverseLerp(1, 100, scaleMultiplier));
            _transform.position = position;
            _transform.rotation = rotation;

            ActivateParticleSystem(scaleMultiplier);
            ActivateLight(scaleMultiplier);
            SetRandomRotation();
            
            Invoke(nameof(Despawn), _duration * 1.5f);
        }

        private void ActivateParticleSystem(float scaleMultiplier)
        {
            if (!_particleSystems) return;
            
            ParticleSystem.MainModule main = _particleSystems.main;
            main.startSizeMultiplier = _startSizeMultiplier;

            main.duration = _duration;
            main.startLifetimeMultiplier = _duration;

            _particleSystems.Play();
        }

        private void ActivateLight(float scaleMultiplier)
        {
            if (!_light) return;
            _light.range = 0;
            _light.intensity = 0;
        }
        
        private void UpdateLight()
        {
            if (!_light) return;
            
            float t = Mathf.PingPong(Time.time, _duration) / _duration;
            _light.range = Mathf.Lerp(0, _startSizeMultiplier, t);
            _light.intensity = Mathf.Lerp(0, 8, t);
            
            if(t >= 1)
                ReturnToPool();
        }

        private void SetRandomRotation()
        {
            transform.rotation = Quaternion.Euler(UnityEngine.Random.insideUnitSphere * 360);
        }

        private void LateUpdate()
        {
            UpdateLight();
        }

        public void InitializePoolObject(ObjectPool<Explosion> pool)
        {
            _pool = pool;
        }
        
        private void Despawn()
        {
            if(_usePoolingSO.Value) 
                ReturnToPool();
            else
                Destroy(gameObject);
        }

        public void ReturnToPool()
        {
            _pool.Return(this);
        }
    }
}