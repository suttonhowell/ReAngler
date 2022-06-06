using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SimpleReactionManager : MonoBehaviour
{
	//this is just for the instructions to disappear
	bool started;
	bool locked;
	List<Stage> stages;
	ArrayList userScores;
	GameObject target;
	GameObject overlay;
	Round currRound;
	int currStage;
	//sprites are set in the inspector within unity
	public Sprite[] overlaySprites = new Sprite[5];
	public Sprite[] targetSprites = new Sprite[5];

	private HapticsManager hapticsManager;
	private AudioManager audioManager;
	private DataHandler dataHandler;

	// Start is called before the first frame update
	void Start()
	{
		dataHandler = GetComponent<DataHandler>();
		if (dataHandler == null) throw new Exception("data handler creation error");

		started = false;
		locked = true;
		stages = GetStageInfo();
		userScores = new ArrayList();
		//find the overlay
		overlay = GameObject.Find("overlay");
		//verify target found
		if (overlay == null) throw new Exception("overlay could not be found");

		//find the target
		target = GameObject.Find("centerTarget");
		//verify target found
		if (target == null) throw new Exception("Target could not be found");
		currStage = 0;

		hapticsManager = GetComponent<HapticsManager>();
		//verify haptics manager
		if (hapticsManager == null) throw new Exception("haptics manager could not be found");

		audioManager = GetComponent<AudioManager>();
		//verify audio manager
		if (audioManager == null) throw new Exception("audio manager could not be found");

	}

	// Update is called once per frame
	void Update()
	{
		//only allow user to play once the researchers have unlocked the program
		if ((Input.GetKeyDown(KeyCode.U))){
			locked = false;
		}
		//remove instructions and begin when user presses space 
		if (!locked && !started && (Input.GetKeyDown("space") || Input.GetKeyUp(KeyCode.JoystickButton0)))
		{
			print("Space key was pressed, starting reaction experiment");
			ChangeOverlay(0);
			started = true;
			NextRound();
		}

		//if a round is currently executing...
		if (currRound != null && started)
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
					audioManager.Stop(AudioManager.clipName.fishOnLine);
					audioManager.Play(AudioManager.clipName.fishCaught);
					//save the round as-is to the user scores, so all data is saved with stage info
					userScores.Add(currRound);
					//move to next stage and round
					currStage += 1;
					//positive feedback (with 3 second cooldown)
					//check for end of experiment
					//end of experiment
					if (currStage >= stages.Count)
					{
						ChangeSprite(0);
						currRound = null;
						//save scores
						DataHandler.SaveResults(userScores);
						ChangeOverlay(3);
						return;
					}
					//check for new round
					else if (currStage-1 > 0 && stages[currStage-1].round != stages[currStage].round){
						StartCoroutine(RoundOver(3));
					} 
					else {
						StartCoroutine(WaitForNextRound(3));
					}
					
				}//end of user reacted
			}//end of if current round is active
			return;
		}//end of if currRound is not null

	}


	// Import list of reaction settings for each round
	// hard coded for now until integration
	private List<Stage> GetStageInfo()
	{
		//get info from data handler
		List<Dictionary<string, object>> stageImport = dataHandler.StageReader();
		if (stageImport == null) throw new Exception("Data Handler stage passing failed");

		// create stage objects and populate list
		List<Stage> stages = new List<Stage>();

		//timing between game start and cue in s
		float timing = 0f;
		int audio = 0;
		int haptics = 0;
		int roundNum = 0;

		for (int i=0; i < stageImport.Count; i++){
			timing = Convert.ToSingle(stageImport[i]["delay"]);
			audio = Convert.ToInt32(stageImport[i]["audioEnabled"]);
			audio = Convert.ToInt32(stageImport[i]["hapticsEnabled"]);
			roundNum = Convert.ToInt32(stageImport[i]["round"]);
			stages.Add(new Stage(timing, audio, haptics, roundNum));
			Debug.Log( "Added stage " + stageImport[i]["stageNum"] + " with delay " + timing + " to stage list.\n");
		}

		return stages;
	}



	//changes Stages/rounds
	private void NextRound()
	{

		Debug.Log("Starting round " + currStage + 1);
		currRound = new Round(stages[currStage]);
		
		//update haptics settings
		hapticsManager.SetHaptics(currRound.currStage.hapticsEnabled);
		//update audio settings
		audioManager.SetAudio(currRound.currStage.audioEnabled);

		StartCoroutine(ReactionWait());
	}

	private void ChangeSprite(int spriteNum)
	{
		target.GetComponent<SpriteRenderer>().sprite = targetSprites[spriteNum];
	}

	private void ChangeOverlay(int spriteNum){
		overlay.GetComponent<SpriteRenderer>().sprite = overlaySprites[spriteNum];
	}

	//this will change the image to cue and begin recording time
	IEnumerator ReactionWait()
	{
		//Print the time of when the function is first called.
		Debug.Log("Started ReactionWait Coroutine at timestamp : " + Time.time);
		//change sprite to neutral/no cue
		ChangeSprite(1);
		//no cues before actual cue, so these are disabled for now
		//hapticsManager.HapticsCalm();
		//audioManager.Play(AudioManager.clipName.stillWater);
		//yield on a new YieldInstruction that waits for the set stage time
		yield return new WaitForSeconds(currRound.currStage.time);

		//After we have waited 5 seconds print the time again.
		Debug.Log("Finished ReactionWait Coroutine at timestamp : " + Time.time);

		//active stage and change sprite
		currRound.active = true;
		hapticsManager.HapticsHectic();
		//audioManager.Stop(AudioManager.clipName.stillWater);
		audioManager.Play(AudioManager.clipName.fishOnLine);
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

	IEnumerator RoundOver(int cooldown)
	{
		//Print the time of when the function is first called.
		Debug.Log("Started roundover Coroutine at timestamp : " + Time.time);
		//show user they got it right, wait for cooldown until next round
		ChangeSprite(3);

		//yield on a new YieldInstruction that waits for 5 seconds.
		yield return new WaitForSeconds(cooldown);
		ChangeOverlay(2);
		ChangeSprite(0);
		started = false;
		locked = true;
		//After we have waited 5 seconds print the time again.
		Debug.Log("Finished roundover Coroutine at timestamp : " + Time.time);
	}

}


class Stage
{
	public float time;
	public bool audioEnabled;
	public bool hapticsEnabled;
	public int round;
	public Stage(float time, int audioEnabled, int hapticsEnabled, int round)
	{
		this.time = time;
		this.audioEnabled = Convert.ToBoolean(audioEnabled);
		this.hapticsEnabled = Convert.ToBoolean(hapticsEnabled);
		this.round = round;
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