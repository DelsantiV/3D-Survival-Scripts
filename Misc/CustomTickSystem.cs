using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GoTF.Content
{
    public static class CustomTickSystem
    {
        public static event Action OnTick;
        public static event Action OnLargeTick;
        public static event Action OnGameHourTick;
        public static event Action OnSpoilageTick;
        public delegate void SubscriberDelegate();
        public static List<SubscriberDelegate> Subscribers = new List<SubscriberDelegate>();

        private const float tickTimerMax = .2f; // So tick is 0.2s
        private const int largeTickTimerMultiplier = 5; // So large tick is 1s
        private const int numberOfSecondsInGameHour = 720;
        private const int numberOfTicksForSpoilageUpdate = 500;


        private static GameObject tickSystemObject;

        public static void SubscribeToTick(Delegate method, int numberOfTicks)
        {

        }
        public static void InitializeTickSystem()
        {
            if (tickSystemObject == null)
            {
                tickSystemObject = new GameObject("TickSystem");
                tickSystemObject.AddComponent<CustomTickSystemObject>();
            }
        }

        private class CustomTickSystemObject : MonoBehaviour
        {
            private float timer;
            private int tickCount;
            private int tickCountSpoilage;
            private int secondsCount;

            private void Awake()
            {
                tickCount = 0;
                secondsCount = 0;
            }

            // Update is called once per frame
            void Update()
            {
                timer += Time.deltaTime;
                if (timer >= tickTimerMax)
                {
                    timer -= tickTimerMax;
                    OnTick?.Invoke();
                    tickCount++;
                    tickCountSpoilage++;
                    Debug.Log("tick !");
                }

                if (tickCount == largeTickTimerMultiplier)
                {
                    tickCount = 0;
                    secondsCount++;
                    OnLargeTick?.Invoke();
                    Debug.Log("Large tick !");
                }

                if (tickCountSpoilage == numberOfTicksForSpoilageUpdate)
                {
                    tickCountSpoilage = 0;
                    OnSpoilageTick?.Invoke();
                    Debug.Log("Spoilage Tick !");
                }

                if (secondsCount == numberOfSecondsInGameHour)
                {
                    secondsCount = 0;
                    OnGameHourTick?.Invoke();
                    Debug.Log("Game Hour Tick !");
                }
            }
        }
    }
}
