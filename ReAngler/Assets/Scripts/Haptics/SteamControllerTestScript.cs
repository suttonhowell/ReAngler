using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class SteamControllerTestScript : MonoBehaviour
{
	void Start()
	{
		// Just a test to make sure the steamworks API is functioning correctly
		if (SteamManager.Initialized)
		{
			string name = SteamFriends.GetPersonaName();
			Debug.Log("username: " + name);
		}
	}

	void Update()
	{
		// Get the controllers
		InputHandle_t[] inputHandles = new InputHandle_t[Constants.STEAM_INPUT_MAX_COUNT];
		SteamInput.GetConnectedControllers(inputHandles);

		// Simple test to make sure the controller and haptics are recognized
		if (Input.GetKeyDown(KeyCode.JoystickButton0))
		{
			SteamInput.Legacy_TriggerHapticPulse(inputHandles[0], ESteamControllerPad.k_ESteamControllerPad_Left, 10000);
			Debug.Log("gamepad");
		}
	}
}
