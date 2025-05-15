using System;
using RidersRuntime.VehicleSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.Users;
using UnityEngine.SceneManagement;

namespace RidersRuntime.GameSystems
{
    public class EventSystemSpawner : MonoBehaviour
    {
        // This class is responsible for spawning the event system and setting up the input system
        // It will be used to manage the event system for the character selection screen

        public GameObject eventSystemPrefab;

        void Start()
        {
            SceneManager.sceneLoaded += OnSceneEntered;
        }

        private void OnSceneEntered(Scene scene, LoadSceneMode sceneMode)
        {
            // Certain More or less most scenes will need to have their own event systems implemented

            // Nuke all the event systems
            EventSystem[] eventSystems = FindObjectsByType<EventSystem>(FindObjectsSortMode.InstanceID);

            foreach (var eventSystem in eventSystems)
            {
                Destroy(eventSystem.gameObject);
            }

            // Check each player
            foreach (var user in InputUser.all)
            {
                EventSystem es = new GameObject("Event System for " + user.index).AddComponent<MultiplayerEventSystem>();
                PlayerInput.GetPlayerByIndex(user.index).uiInputModule = es.GetComponent<InputSystemUIInputModule>();
            }
        }

        public static EventSystem CreateNewPlayerEventSystem(int playerIndex)
        {
            EventSystem es = new GameObject("Event System for " + playerIndex, new Type[] { typeof(MultiplayerEventSystem), typeof(InputSystemUIInputModule) }).GetComponent<MultiplayerEventSystem>();
            PlayerInput.GetPlayerByIndex(playerIndex).uiInputModule = es.GetComponent<InputSystemUIInputModule>();
            return es;
        }
    }
}