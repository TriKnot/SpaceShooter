using Jobs;
using UnityEngine;

namespace Asteroids
{

    public static class CollisionManager
    {
        public static void CalculateCollision(AsteroidMovement asteroid1, AsteroidMovement asteroid2, Vector3 contactPoint)
        {
            // Define the masses and initial velocities of two bodies
            float mass1 = asteroid1.Mass;
            float mass2 = asteroid2.Mass;
    
            Vector3 initialVelocity1 = asteroid1.AsteroidMoveData.Velocity;
            Vector3 initialVelocity2 = asteroid2.AsteroidMoveData.Velocity;

            // Calculate the relative velocity
            Vector3 relativeVelocity = initialVelocity1 - initialVelocity2;

            // Calculate the collision normal (direction along which the collision occurs)
            Vector3 collisionNormal = (contactPoint - asteroid1.transform.position).normalized;

            // Calculate the change in linear velocity (reflect off the collision plane)
            float relativeSpeed = Vector3.Dot(relativeVelocity, collisionNormal);
            Vector3 linearVelocityChange = -2 * relativeSpeed * collisionNormal;

            Vector3 finalVelocity1 = initialVelocity1 + (linearVelocityChange * (mass2 / (mass1 + mass2)));
            Vector3 finalVelocity2 = initialVelocity2 - (linearVelocityChange * (mass1 / (mass1 + mass2)));


            MoveData asteroidMoveData = new MoveData()
            {
                Velocity = finalVelocity1,
                AngularVelocity = asteroid1.AsteroidMoveData.AngularVelocity
            };
            asteroid1.AsteroidMoveData = asteroidMoveData;
            
            asteroidMoveData = new MoveData()
            {
                Velocity = finalVelocity2,
                AngularVelocity = asteroid2.AsteroidMoveData.AngularVelocity
            };

            asteroid2.AsteroidMoveData = asteroidMoveData;

        }


    }
}