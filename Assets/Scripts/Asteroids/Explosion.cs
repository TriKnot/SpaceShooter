using System;
using UnityEngine;
using Util;
using Utils;

namespace Asteroids
{
    public class Explosion : MonoBehaviour, IPoolObject<Explosion>
    {
        [SerializeField] private ParticleSystem _particleSystems;
        [SerializeField] private Light _light;
        private ObjectPool<Explosion> _pool;

        private float _scaleMultiplier;

        public void Explode(float scaleMultiplier)
        {
            _scaleMultiplier = scaleMultiplier;
            UpdateParticleSystem(scaleMultiplier);
            UpdateLight(scaleMultiplier);
            SetRandomRotation();
            Invoke(nameof(ReturnToPool), 2.0f);
        }

        private void UpdateParticleSystem(float scaleMultiplier)
        {
            if (_particleSystems)
            {
                ParticleSystem.MainModule main = _particleSystems.main;
                main.startSizeMultiplier *= scaleMultiplier * 2;

                float duration = Mathf.Lerp(0.25f, 2.5f, Mathf.InverseLerp(10, 100, scaleMultiplier));
                main.duration = duration;
                main.startLifetimeMultiplier = duration;

                _particleSystems.Play();
            }
        }

        private void UpdateLight(float scaleMultiplier)
        {
            if (_light)
            {
                _light.range *= scaleMultiplier;
                _light.intensity *= scaleMultiplier;
            }
        }

        private void SetRandomRotation()
        {
            transform.rotation = Quaternion.Euler(UnityEngine.Random.insideUnitSphere * 360);
        }

        private void LateUpdate()
        {
            float step = _scaleMultiplier * Time.deltaTime * 10;
            _light.intensity -= step;
            _light.range -= step;
        }

        public void InitializePoolObject(ObjectPool<Explosion> pool)
        {
            _pool = pool;
        }

        public void ReturnToPool()
        {
            _pool.Return(this);
        }
    }
}