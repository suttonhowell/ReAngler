using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public AudioClip stillWater;
	public AudioClip fishCaught;
	public AudioClip fishOnLine;

	public enum clipName
	{
		stillWater,
		fishCaught,
		fishOnLine
	};

	Dictionary<clipName, AudioSource> audioSources;

	void Start()
	{
		audioSources = new Dictionary<clipName, AudioSource>();
		audioSources[clipName.stillWater] = gameObject.AddComponent<AudioSource>();
		audioSources[clipName.stillWater].clip = stillWater;
		audioSources[clipName.fishCaught] = gameObject.AddComponent<AudioSource>();
		audioSources[clipName.fishCaught].clip = fishCaught;
		audioSources[clipName.fishOnLine] = gameObject.AddComponent<AudioSource>();
		audioSources[clipName.fishOnLine].clip = fishOnLine;
	}

	void Update()
	{

	}

	public void Play(clipName name)
	{
		audioSources[name].Play();
	}

	public void Stop(clipName name)
	{
		audioSources[name].Stop();
	}
}
