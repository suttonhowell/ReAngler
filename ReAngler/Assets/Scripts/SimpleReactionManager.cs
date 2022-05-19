using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SimpleReactionManager : MonoBehaviour
{
	//this is just for the instructions to disappear
	bool started;
	List<Stage> stages;
	ArrayList userScores;
	GameObject target;
	Round currRound;
	int currStage;
	//sprites are set in the inspector within unity
	public Sprite[] targetSprites = new Sprite[5];

	private HapticsManager hapticsManager;

	// Start is called before the first frame update
	void Start()
	{
		started = false;
		stages = GetStageInfo();
		userScores = new ArrayList();
		//find the target
		target = GameObject.Find("centerTarget");
		//verify target found
		if (target == null) throw new Exception("Target could not be found");
		currStage = 0;

		hapticsManager = GetComponent<HapticsManager>();
	}

	// Update is called once per frame
	void Update()
	{
		//if a round is currently executing...
		if (currRound != null)
		{
			//add time to the user reaction timer
			if (currRound.active)
			{
				currRound.userTime += Time.deltaTime;
				//check for reaction
				if (Input.GetKeyDown("space") || Input.GetKey("space") || Input.GetKey(KeyCode.JoystickButton0) || Input.GetKeyDown(KeyCode.JoystickButton0))
				{
					//end the stage
					currRound.active = false;
					hapticsManager.CancelHaptics();
					//save the round as-is to the user scores, so all data is saved with stage info
					userScores.Add(currRound);
					//move to next stage and round
					currStage += 1;
					//positive feedback (with 3 second cooldown)
					StartCoroutine(WaitForNextRound(3));

				}//end of user reacted
			}//end of if current round is active
			return;
		}//end of if currRound is not null

		//Debug.Log("Time since level load: " + Time.timeSinceLevelLoad);
		//remove instructions and begin when user presses space 
		if (!started && (Input.GetKeyUp("space") || Input.GetKeyUp(KeyCode.JoystickButton0)))
		{
			print("Space key was pressed, starting reaction");
			Destroy(GameObject.Find("instructions"));
			started = true;
			NextRound();
		}
	}

	// Import list of reaction settings for each round
	// hard coded for now until integration
	private List<Stage> GetStageInfo()
	{
		List<Stage> stages = new List<Stage>();
		//timing between game start and cue in ms (only value so far before vibrations)
		float timing = 1.5f;

		stages.Add(new Stage(timing));

		// this is just me adding fake data for testing
		// remove when we decide on stages
		stages.Add(new Stage(1.7f));
		stages.Add(new Stage(3f));
		stages.Add(new Stage(1.0f));
		stages.Add(new Stage(2.2f));
		return stages;
	}



	//changes Stages/rounds
	private void NextRound()
	{
		if (currStage >= stages.Count)
		{
			ChangeSprite(0);
			currRound = null;
			return;
		}
		Debug.Log("Starting round " + currStage + 1);
		currRound = new Round(stages[currStage]);
		StartCoroutine(ReactionWait());
	}

	private void ChangeSprite(int spriteNum)
	{
		Debug.Log("changed sprite");
		target.GetComponent<SpriteRenderer>().sprite = targetSprites[spriteNum];
	}

	//this will change the image to cue and begin recording time
	IEnumerator ReactionWait()
	{
		//Print the time of when the function is first called.
		Debug.Log("Started ReactionWait Coroutine at timestamp : " + Time.time);
		//change sprite to neutral/no cue
		ChangeSprite(1);
		hapticsManager.HapticsCalm();
		//yield on a new YieldInstruction that waits for the set stage time
		yield return new WaitForSeconds(currRound.currStage.time);

		//After we have waited 5 seconds print the time again.
		Debug.Log("Finished ReactionWait Coroutine at timestamp : " + Time.time);

		//active stage and change sprite
		currRound.active = true;
		hapticsManager.HapticsHectic();
		ChangeSprite(2);
	}

	//this is just to show the user the recognition that they clicked in time
	IEnumerator WaitForNextRound(int cooldown)
	{
		//Print the time of when the function is first called.
		Debug.Log("Started WaitForNextRound Coroutine at timestamp : " + Time.time);
		//show user they got it right, wait for cooldown until next round
		ChangeSprite(3);

		//yield on a new YieldInstruction that waits for 5 seconds.
		yield return new WaitForSeconds(cooldown);

		//After we have waited 5 seconds print the time again.
		Debug.Log("Finished WaitForNextRound Coroutine at timestamp : " + Time.time);

		NextRound();
	}

}


class Stage
{
	public float time;
	public Stage(float time)
	{
		this.time = time;
	}
}

class Round
{
	//the information about the current reaction settings
	public Stage currStage;
	//the user's reaction time
	public float userTime;
	public bool active;

	public Round(Stage currStage)
	{
		this.currStage = currStage;
		userTime = 0;
		active = false;
	}
}