using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

// Provides a mechanism to set the different phases of the haptic motors
public class HapticsManager : MonoBehaviour
{
	private InputHandle_t controllerHandle; // Necessary reference for the steam API to ID a controller
	private bool enableHaptics = false;

	void Start()
	{
		StartCoroutine(FindController());
	}

	void Update()
	{
		// TODO: Check periodically for controller still connected?
	}

	public void SetHaptics(bool hapticsOn)
	{
		enableHaptics = hapticsOn;
	}

	// Search for the controller until found. Just checks with the steam API, not Unity
	private IEnumerator FindController()
	{
		// if (!enableHaptics) yield break; // Don't need the handle if no haptics

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
		if (!enableHaptics) return;

		// ESteamControllerPad targetPad = leftPad ? ESteamControllerPad.k_ESteamControllerPad_Left : ESteamControllerPad.k_ESteamControllerPad_Right;
		ushort durationOn = 500, durationOff = 65535, repetitions = 10000;

		SteamInput.Legacy_TriggerRepeatedHapticPulse(controllerHandle, ESteamControllerPad.k_ESteamControllerPad_Left, durationOn, durationOff, repetitions, 0);
		SteamInput.Legacy_TriggerRepeatedHapticPulse(controllerHandle, ESteamControllerPad.k_ESteamControllerPad_Right, durationOn, durationOff, repetitions, 0);
	}

	// Perform the more hectic vibrate to indicate it's time to press
	public void HapticsHectic() // TODO: Is this ok? Or make it repeat forever until pressed? It goes a pretty long time rn before stopping
	{
		if (!enableHaptics) return;

		ushort durationOn = 3600, durationOff = 400, repetitions = 41; // = .164 sec of 250 Hz
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
		if (!enableHaptics) return;

		ushort durationOn = 0;
		SteamInput.Legacy_TriggerHapticPulse(controllerHandle, ESteamControllerPad.k_ESteamControllerPad_Left, durationOn);
		SteamInput.Legacy_TriggerHapticPulse(controllerHandle, ESteamControllerPad.k_ESteamControllerPad_Right, durationOn);
	}
}
