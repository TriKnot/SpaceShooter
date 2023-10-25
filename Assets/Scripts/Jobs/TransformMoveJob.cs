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
        [ReadOnly] private float _deltaTime;
        [ReadOnly] private NativeArray<MoveData> _moveDataArray;

        public TransformMoveJob(float deltaTime, NativeArray<MoveData> moveDataArray)
        {
            _deltaTime = deltaTime;
            _moveDataArray = moveDataArray;
        }
        
        public void Execute(int index, TransformAccess transform)
        {
            // Check if active
            if (!_moveDataArray[index].IsActive)
            {
                return;
            }
            // Move the asteroid
            Vector3 pos = transform.position;
            pos += _moveDataArray[index].Velocity * _deltaTime;
            transform.position = pos;
            // Rotate the asteroid
            quaternion deltaRotation = Quaternion.Euler(_moveDataArray[index].AngularVelocity * _deltaTime);
            quaternion rot = transform.rotation;
            rot = math.mul(rot, deltaRotation);
            transform.rotation = rot;
        }
    }
}