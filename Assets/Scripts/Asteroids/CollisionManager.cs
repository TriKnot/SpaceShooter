using UnityEngine;

namespace Asteroids
{

    public static class CollisionManager
    {
        public static void CalculateCollision(AsteroidMovement asteroid1, AsteroidMovement asteroid2)
        {
            // Define the masses and initial velocities of two bodies
            float mass1 = asteroid1.Mass;
            float mass2 = asteroid2.Mass;
            Vector3 initialVelocity1 = asteroid1.Velocity;
            Vector3 initialVelocity2 = asteroid2.Velocity;

            // Calculate the initial total momentum
            Vector3 initialTotalMomentum = mass1 * initialVelocity1 + mass2 * initialVelocity2;

            // Calculate the relative velocity
            Vector3 relativeVelocity = initialVelocity1 - initialVelocity2;

            // Calculate the final velocities using the equations for elastic collision
            Vector3 finalVelocity1 = initialVelocity1 - (2 * mass2 / (mass1 + mass2)) * (Vector3.Dot(relativeVelocity, initialVelocity1 - initialVelocity2) / Vector3.Dot(initialVelocity1 - initialVelocity2, initialVelocity1 - initialVelocity2)) * (initialVelocity1 - initialVelocity2);
            Vector3 finalVelocity2 = initialVelocity2 - (2 * mass1 / (mass1 + mass2)) * (Vector3.Dot(relativeVelocity, initialVelocity2 - initialVelocity1) / Vector3.Dot(initialVelocity2 - initialVelocity1, initialVelocity2 - initialVelocity1)) * (initialVelocity2 - initialVelocity1);

            asteroid1.Velocity = finalVelocity1;
            asteroid2.Velocity = finalVelocity2;
            
            Debug.Log($"Initial Total Momentum: {initialTotalMomentum} -->> Final Total Momentum: {mass1 * finalVelocity1 + mass2 * finalVelocity2}");

        }
    }
}