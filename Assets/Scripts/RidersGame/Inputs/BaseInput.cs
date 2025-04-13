using UnityEngine;
using RidersCore.Data;

namespace RidersCore
{
    public interface IInput
    {
        public ActorInputData GrabCurrentFrameInputs();
    }

    public abstract class BaseInput : MonoBehaviour, IInput
    {
        // 3 Major components will be using this
        // StuntSystem, VehicleSystem and Analytics.
        // The AI Will have it's own "AIInput" class that will implement this interface.
        public abstract ActorInputData GrabCurrentFrameInputs();
    }
}