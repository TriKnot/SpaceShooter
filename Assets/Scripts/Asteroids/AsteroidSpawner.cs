using UnityEngine;
using Random = UnityEngine.Random;
using Util;

namespace Asteroids
{
    public static class AsteroidSpawner
    {
        
        public static Asteroid SpawnAsteroid(Asteroid[] asteroidPrefabs, Vector3 spawnPos)
        {
            int randAsteroidIndex = Random.Range(0, asteroidPrefabs.Length);
            Asteroid asteroid = Object.Instantiate(asteroidPrefabs[randAsteroidIndex]);
            asteroid.gameObject.SetActive(false);
            asteroid.Activate(spawnPos, Random.rotation);

            return asteroid;
        }

        public static Asteroid SpawnAsteroid(ObjectPool<Asteroid> asteroidPool, Vector3 spawnPos)
        {
            Asteroid asteroid = asteroidPool.Get();
            asteroid.Activate(spawnPos, Random.rotation);

            return asteroid;
        }

        public static bool IsValidSpawnPosition(Vector3 spawnPosition, Camera camera = null)
        {
            if(camera == null )
                camera = Camera.main;
            return !PointInCameraView(spawnPosition, camera);
        }


        private static bool PointInCameraView(Vector3 point, Camera camera)
        {
            if (camera == null)
            {
                return false;
            }

            Vector3 viewport = camera.WorldToViewportPoint(point);
            if(!(viewport.z > 0))
                return false;
            if(!(Is01(viewport.x) && Is01(viewport.y)))
                return false;
            return !CheckObjectBlockingPoint(point, camera.transform.position);
        }

        private static bool Is01(float a)
        {
            return a is > 0 and < 1;
        }

        private static bool CheckObjectBlockingPoint(Vector3 point, Vector3 cameraPosition)
        {
            bool objectBlockingPoint = false;
            Vector3 directionBetween = (point - cameraPosition).normalized;
            float distance = Vector3.Distance(cameraPosition, point);

            if (Physics.Raycast(cameraPosition, directionBetween, out var depthCheck, distance + 0.05f))
            {
                if (depthCheck.point != point)
                {
                    objectBlockingPoint = true;
                }
            }

            return objectBlockingPoint;
        }
    }
}
