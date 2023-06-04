using SuspiciousGames.SellMe.Utility.Exceptions;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SuspiciousGames.SellMe.Utility.Sensors
{

    public static class StepSensorReader
    {
        public static bool SensorActive => StepCounter.current.enabled;

        public static int GetStepCount()
        {
            try
            {
                return StepCounter.current.stepCounter.ReadValue();
            }
            catch (SensorNotConnectedException e)
            {
                Debug.LogException(e);
                if (SensorActive)
                    return StepCounter.current.stepCounter.ReadValue();
                throw e;
            }
        }

        public static void Activate()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                Debug.Log("Trying to connect to sensor...");
                if (!StepCounter.current.enabled)
                {
                    InputSystem.EnableDevice(StepCounter.current);
                    Debug.Log("StepCounter sensor was enabled:" + StepCounter.current.enabled);
                }
                else
                {
                    Debug.Log("StepCounter sensor was already enabled");
                }
            }
        }

        public static IEnumerator ConnectToSensor()
        {
            while (!SensorActive)
            {
                Activate();
                yield return new WaitForEndOfFrame();
            }
        }

        public static bool DisconnectFromSensor()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                InputSystem.DisableDevice(StepCounter.current);
                Debug.Log("StepCounter sensor was disabled:" + !StepCounter.current.enabled);
                return !StepCounter.current.enabled;
            }
            return true;
        }
    }
}
