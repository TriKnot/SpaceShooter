using Jobs;
using UnityEngine;

namespace Asteroids
{
    public static class CollisionManager
    {
        public static void CalculateCollision(AsteroidMovement asteroid1, AsteroidMovement asteroid2, Vector3 contactPoint)
        {
            // Extract mass and initial velocities
            float mass1 = asteroid1.Mass;
            float mass2 = asteroid2.Mass;
            Vector3 initialVelocity1 = asteroid1.AsteroidMoveData.Velocity;
            Vector3 initialVelocity2 = asteroid2.AsteroidMoveData.Velocity;

            // Calculate relative velocity and collision normal
            Vector3 relativeVelocity = initialVelocity1 - initialVelocity2;
            Vector3 collisionNormal = (contactPoint - asteroid1.transform.position).normalized;

            // Calculate the change in linear velocity (reflect off the collision plane)
            float relativeSpeed = Vector3.Dot(relativeVelocity, collisionNormal);
            Vector3 linearVelocityChange = -2 * relativeSpeed * collisionNormal;

            // Calculate final velocities
            Vector3 finalVelocity1 = initialVelocity1 + (linearVelocityChange * (mass2 / (mass1 + mass2)));
            Vector3 finalVelocity2 = initialVelocity2 - (linearVelocityChange * (mass1 / (mass1 + mass2)));

            // Update AsteroidMoveData for both asteroids
            UpdateAsteroidMoveData(asteroid1, finalVelocity1);
            UpdateAsteroidMoveData(asteroid2, finalVelocity2);
        }

        private static void UpdateAsteroidMoveData(AsteroidMovement asteroid, Vector3 finalVelocity)
        {
            MoveData asteroidMoveData = new MoveData()
            {
                IsActive = asteroid.AsteroidMoveData.IsActive,
                Velocity = finalVelocity,
                AngularVelocity = asteroid.AsteroidMoveData.AngularVelocity
            };
            asteroid.AsteroidMoveData = asteroidMoveData;
        }
    }
}