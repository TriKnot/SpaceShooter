using System;
using UnityEngine;

namespace Asteroids
{
    public class Explosion : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _particleSystems;
        [SerializeField] private Light _light;
        
        private float _scaleMultiplier;
        
        
        public void Explode(float scaleMultiplier)
        {
            _scaleMultiplier = scaleMultiplier;
            // Setup the explosion and play it
            ParticleSystem.MainModule main = _particleSystems.main;
            main.startSizeMultiplier *= scaleMultiplier * 2;
            
            // Set duration and lifetime to scale with the explosion between 0.25s and 5s
            float duration = Mathf.Lerp(0.25f, 2.5f, Mathf.InverseLerp(10, 100, scaleMultiplier));
            main.duration = duration;
            main.startLifetimeMultiplier = duration;
            
            _particleSystems.Play();
            
            _light.range *= scaleMultiplier;
            _light.intensity *= scaleMultiplier;
            
            // Set a random spherical rotation
            transform.rotation = Quaternion.Euler(UnityEngine.Random.insideUnitSphere * 360);
            
            // Despawn the explosion after 2 seconds
            Invoke(nameof(DespawnExplosion), 2.0f);
        }

        private void LateUpdate()
        {
            // Decrease the size of the explosion over time
            float step = _scaleMultiplier * Time.deltaTime * 10;
            _light.intensity -= step;
            _light.range -= step;
        }

        private void DespawnExplosion()
        {
            Destroy(gameObject);
        }
    }
}
