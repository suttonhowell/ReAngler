using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

// Provides a mechanism to set the different phases of the haptic motors
public class HapticsManager : MonoBehaviour
{
	private InputHandle_t controllerHandle; // Necessary reference for the steam API to ID a controller

	void Start()
	{
		StartCoroutine(FindController());
	}

	void Update()
	{
		// TODO: Check periodically for controller still connected?
	}

	// Search for the controller until found. Just checks with the steam API, not Unity
	private IEnumerator FindController()
	{
		InputHandle_t[] inputHandles = new InputHandle_t[Constants.STEAM_INPUT_MAX_COUNT];

		// Do the actual searching and checking
		while (SteamInput.GetConnectedControllers(inputHandles) == 0)
		{
			// throw new MissingReferenceException("No controllers found"); // TODO: not sure if this is the right type
			Debug.Log("Looking for controllers");
			yield return new WaitForSeconds(1);
		}

		Debug.Log("Controller found");
		controllerHandle = inputHandles[0];
	}

	// Perform a calmer vibrate before the button-press cue
	public void HapticsCalm() // TODO: rename, support just left or right at a time?
	{

		// ESteamControllerPad targetPad = leftPad ? ESteamControllerPad.k_ESteamControllerPad_Left : ESteamControllerPad.k_ESteamControllerPad_Right;
		ushort durationOn = 500, durationOff = 65535, repetitions = 10000;

		SteamInput.Legacy_TriggerRepeatedHapticPulse(controllerHandle, ESteamControllerPad.k_ESteamControllerPad_Left, durationOn, durationOff, repetitions, 0);
		SteamInput.Legacy_TriggerRepeatedHapticPulse(controllerHandle, ESteamControllerPad.k_ESteamControllerPad_Right, durationOn, durationOff, repetitions, 0);
	}

	// Perform the more hectic vibrate to indicate it's time to press
	public void HapticsHectic() // TODO: Is this ok? Or make it repeat forever until pressed? It goes a pretty long time rn before stopping
	{
		ushort durationOn = 10000, durationOff = 1000, repetitions = 10000;
		SteamInput.Legacy_TriggerRepeatedHapticPulse(controllerHandle, ESteamControllerPad.k_ESteamControllerPad_Left, durationOn, durationOff, repetitions, 0);
		SteamInput.Legacy_TriggerRepeatedHapticPulse(controllerHandle, ESteamControllerPad.k_ESteamControllerPad_Right, durationOn, durationOff, repetitions, 0);
	}

	// Called after button is pressed
	public void CancelHaptics()
	{
		SteamInput.Legacy_TriggerRepeatedHapticPulse(controllerHandle, ESteamControllerPad.k_ESteamControllerPad_Left, 0, 0, 0, 0);
		SteamInput.Legacy_TriggerRepeatedHapticPulse(controllerHandle, ESteamControllerPad.k_ESteamControllerPad_Right, 0, 0, 0, 0);
	}

	// TODO: needed?
	// The idea is to give an error vibration
	public void HapticsEarlyPress()
	{
		ushort durationOn = 0;
		SteamInput.Legacy_TriggerHapticPulse(controllerHandle, ESteamControllerPad.k_ESteamControllerPad_Left, durationOn);
		SteamInput.Legacy_TriggerHapticPulse(controllerHandle, ESteamControllerPad.k_ESteamControllerPad_Right, durationOn);
	}
}
