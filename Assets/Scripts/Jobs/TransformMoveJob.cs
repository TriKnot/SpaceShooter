using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

namespace Jobs
{
    [BurstCompile]
    public struct TransformMoveJob : IJobParallelForTransform
    {
        [ReadOnly] private readonly float _deltaTime;
        [ReadOnly] private NativeArray<MoveData> _moveDataArray;

        public TransformMoveJob(float deltaTime, NativeArray<MoveData> moveDataArray)
        {
            _deltaTime = deltaTime;
            _moveDataArray = moveDataArray;
        }
        
        public void Execute(int index, TransformAccess transform)
        {
            if (!_moveDataArray[index].IsActive)
            {
                return;
            }

            MoveAsteroid(transform, _moveDataArray[index]);
            RotateAsteroid(transform, _moveDataArray[index]);
        }

        private void MoveAsteroid(TransformAccess transform, MoveData moveData)
        {
            Vector3 pos = transform.position;
            pos += moveData.Velocity * _deltaTime;
            transform.position = pos;
        }

        private void RotateAsteroid(TransformAccess transform, MoveData moveData)
        {
            quaternion deltaRotation = Quaternion.Euler(moveData.AngularVelocity * _deltaTime);
            quaternion rot = transform.rotation;
            rot = math.mul(rot, deltaRotation);
            transform.rotation = rot;
        }
    }
}