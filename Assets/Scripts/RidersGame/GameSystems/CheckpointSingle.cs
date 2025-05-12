using System;
using UnityEngine;

namespace RidersRuntine.GameSystems
{
    public class CheckpointSingle : MonoBehaviour
    {
        Action<CheckpointSingle, GameObject> onCheckpointPassed;

        public void Setup(Action<CheckpointSingle, GameObject> onCheckpointPassed)
        {
            this.onCheckpointPassed = onCheckpointPassed;
        }

        void OnTriggerEnter(Collider other)
        {
            // Let the track checkpoints manage this
            onCheckpointPassed?.Invoke(this, other.gameObject);
        }
    }
}