using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MoveInputUIController : MonoBehaviour 
{
	[SerializeField] private MatchController mMatchController;
	[SerializeField] private Image[] mP1Inputs;
	[SerializeField] private Image[] mP2Inputs;

	[SerializeField] private Image mInputLockP1;
	[SerializeField] private Image mInputLockP2;

	[SerializeField] private Image mFightMessage;

	[SerializeField] private Image mMatchOverMessage;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		//Turn on "Ready" message and remove input lock UI during player input phase ~Adam
		if(mMatchController.GetMatchState() == MatchController.eMatchState.PLAYERINPUT)
		{
			mFightMessage.enabled = true;
			mInputLockP1.enabled = (mMatchController.GetP1MoveCount()>=3);
			mInputLockP2.enabled = (mMatchController.GetP2MoveCount()>=3);
		}
		//Hide the "Ready" message and show the UI locks duing the input resolve phase ~Adam
		else if (mMatchController.GetMatchState() == MatchController.eMatchState.INPUTRESOLVE)
		{
			mFightMessage.enabled = false;
			mInputLockP1.enabled = true;
			mInputLockP2.enabled = true;
		}
		//Hide input UI during match startup and game over ~Adam
		else
		{
			mFightMessage.enabled = false;
			mInputLockP1.enabled = false;
			mInputLockP2.enabled = false;
		}
		//Display the match end message duing the Game OVer phase ~Adam
		if(mMatchController.GetMatchState() == MatchController.eMatchState.GAMEOVER)
		{
			mMatchOverMessage.enabled = true;
		}
		DisplayInputCounters();

	}

	void DisplayInputCounters()
	{
		for(int i = 0; i < mP1Inputs.Length; i++)
		{
			if(mMatchController.GetP1MoveCount() > i)
			{
				mP1Inputs[i].enabled = true;
			}
			else
			{
				mP1Inputs[i].enabled = false;
			}
			if(mMatchController.GetP2MoveCount() > i)
			{
				mP2Inputs[i].enabled = true;
			}
			else
			{
				mP2Inputs[i].enabled = false;
			}
		}
	}
}
