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

            // Calculate the final velocities using the equations for elastic collision
            Vector3 finalVelocity1 = (initialVelocity1 * (mass1 - mass2) + 2 * mass2 * initialVelocity2) / (mass1 + mass2);
            Vector3 finalVelocity2 = (initialVelocity2 * (mass2 - mass1) + 2 * mass1 * initialVelocity1) / (mass1 + mass2);

            asteroid1.Velocity = finalVelocity1;
            asteroid2.Velocity = finalVelocity2;

        }
    }
}