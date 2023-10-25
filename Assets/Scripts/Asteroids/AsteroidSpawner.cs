using System.Collections;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Random = UnityEngine.Random;
using Util;

namespace Asteroids
{
    [BurstCompile]
    public class AsteroidSpawner
    {
        public static Asteroid SpawnAsteroid(
            Asteroid[] asteroidPrefabs,
            float minSpawnDistance,
            float maxSpawnRadius,
            bool usePooling,
            Transform playerTransform = null,
            ObjectPool<Asteroid> asteroidPool = null)
        {
            Vector3 centerPosOffset = playerTransform ? playerTransform.position : Vector3.zero;
            Vector3 spawnPos = GetRandomSpawnPosition(centerPosOffset, minSpawnDistance, maxSpawnRadius);

            Asteroid asteroid = usePooling || asteroidPool == null ? SpawnAsteroidFromPool(asteroidPool) : SpawnAsteroidFromPrefab(asteroidPrefabs);
            asteroid.Init();
            asteroid.Activate(spawnPos, Random.rotation);
            return asteroid;
        }

        private static Vector3 GetRandomSpawnPosition(Vector3 centerPosOffset, float minSpawnDistance, float maxSpawnRadius)
        {
            Vector3 spawnPos;
            Camera mainCamera = Camera.main; // Kind of a hack for the time being
            int attempts = 0; // Safety measure to prevent infinite loop

            do
            {
                spawnPos = Random.insideUnitSphere * Random.Range(minSpawnDistance, maxSpawnRadius) + centerPosOffset;
            } while (PointInCameraView(spawnPos, mainCamera) && attempts++ < 100);

            return spawnPos;
        }

        private static Asteroid SpawnAsteroidFromPrefab(Asteroid[] asteroidPrefabs)
        {
            int randAsteroidIndex = Random.Range(0, asteroidPrefabs.Length);
            Asteroid asteroid = Object.Instantiate(asteroidPrefabs[randAsteroidIndex]);
            asteroid.gameObject.SetActive(false);
            return asteroid;
        }

        private static Asteroid SpawnAsteroidFromPool(ObjectPool<Asteroid> asteroidPool)
        {
            return asteroidPool.Get();
        }

        private static bool PointInCameraView(Vector3 point, Camera camera)
        {
            if (camera == null)
            {
                return false;
            }

            Vector3 viewport = camera.WorldToViewportPoint(point);
            bool inCameraFrustum = Is01(viewport.x) && Is01(viewport.y);
            bool inFrontOfCamera = viewport.z > 0;
            bool objectBlockingPoint = CheckObjectBlockingPoint(point, camera.transform.position);

            return inCameraFrustum && inFrontOfCamera && !objectBlockingPoint;
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
