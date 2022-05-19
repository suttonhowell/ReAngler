using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class HapticsManager : MonoBehaviour
{
	private InputHandle_t controllerHandle;
	public int HAPTICS_CALM_PARAMS = 0; // Use this to control which haptics settings we're testing at the moment for the calm phase

	void Start()
	{
		FindController();
	}

	void Update()
	{
		FindController(); // TODO: replace this with a coroutine in start that checks until found. controllers take a few frames to be detected
	}

	private void FindController()
	{
		InputHandle_t[] inputHandles = new InputHandle_t[Constants.STEAM_INPUT_MAX_COUNT];
		if (SteamInput.GetConnectedControllers(inputHandles) == 0)
		{
			throw new MissingReferenceException("No controllers found"); // TODO: not sure if this is the right type
		}

		controllerHandle = inputHandles[0];
	}

	// TODO: test TriggerVibration(), this is another function available, but it's for rumble
	public void HapticsCalm(bool leftPad) // TODO: rename, support both at once?
	{

		ESteamControllerPad targetPad = leftPad ? ESteamControllerPad.k_ESteamControllerPad_Left : ESteamControllerPad.k_ESteamControllerPad_Right;
		ushort durationOn = 0, durationOff = 0, repetitions = 0;

		// Allows us to control which parameter settings we're testing and switch easily
		switch (HAPTICS_CALM_PARAMS)
		{
			case 0:
				durationOn = 1000;
				durationOff = 1000;
				repetitions = 10;
				break;
			case 1:
				break;
		}

		SteamInput.Legacy_TriggerRepeatedHapticPulse(controllerHandle, targetPad, durationOn, durationOff, repetitions, 0);
	}

	public void HapticsHectic() // TODO: Is this ok? Or make it repeat forever until pressed? It goes a pretty long time rn before stopping
	{
		ushort durationOn = 10000, durationOff = 1000, repetitions = 1000;
		SteamInput.Legacy_TriggerRepeatedHapticPulse(controllerHandle, ESteamControllerPad.k_ESteamControllerPad_Left, durationOn, durationOff, repetitions, 0);
		SteamInput.Legacy_TriggerRepeatedHapticPulse(controllerHandle, ESteamControllerPad.k_ESteamControllerPad_Right, durationOn, durationOff, repetitions, 0);
	}

	public void CancelHaptics()
	{
		SteamInput.Legacy_TriggerRepeatedHapticPulse(controllerHandle, ESteamControllerPad.k_ESteamControllerPad_Left, 0, 0, 0, 0);
		SteamInput.Legacy_TriggerRepeatedHapticPulse(controllerHandle, ESteamControllerPad.k_ESteamControllerPad_Right, 0, 0, 0, 0);
	}

	// TODO: needed?
	public void HapticsEarlyPress()
	{
		ushort durationOn = 0, durationOff = 0, repetitions = 0;
		SteamInput.Legacy_TriggerRepeatedHapticPulse(controllerHandle, ESteamControllerPad.k_ESteamControllerPad_Left, durationOn, durationOff, repetitions, 0);
		SteamInput.Legacy_TriggerRepeatedHapticPulse(controllerHandle, ESteamControllerPad.k_ESteamControllerPad_Right, durationOn, durationOff, repetitions, 0);
	}
}
