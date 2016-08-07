using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MatchController : MonoBehaviour 
{
	public enum eMatchState {MATCHSTART, PLAYERINPUT, INPUTRESOLVE, GAMEOVER};
	enum ePlayerMove {CHARGE, COUNTER, UPPERCUT};

	[SerializeField] private List<ePlayerMove> mP1Moves = new List<ePlayerMove>();
	[SerializeField] private List<ePlayerMove> mP2Moves = new List<ePlayerMove>();

	[SerializeField] private eMatchState mCurrentMatchState = eMatchState.MATCHSTART;

	[SerializeField] PlayerController mPlayer1;
	[SerializeField] PlayerController mPlayer2;

	Animator mP1Animator;
	Animator mP2Animator;

	[SerializeField] private AudioSource mSFXSource;
	[SerializeField] private AudioClip mUpperCutHit;
	[SerializeField] private AudioClip mChargeHit;
	[SerializeField] private AudioClip mCounterThrow;
	[SerializeField] private AudioClip mPlayerInput;

	[SerializeField] private AudioSource mBGMSource;
	[SerializeField] private AudioClip[] mBGMTracks;

	[SerializeField] private AudioSource mEndMatchJingleSource;

	[SerializeField] private AudioSource mP1InputSFXSource;
	[SerializeField] private AudioSource mP2InputSFXSource;

	public Transform[] mPositionSlots;

	Transform mLosingPlayerTransform;

	[SerializeField] GameObject mRematchMessage;

	// Use this for initialization
	void Start () 
	{
		#region Make the BGM change every match ~Adam
		Debug.Log("Current Track is: " + PlayerPrefs.GetInt("NextBGMTrack"));
		if(PlayerPrefs.GetInt("NextBGMTrack") < mBGMTracks.Length && mBGMTracks.Length > 0)
		{
			mBGMSource.clip = mBGMTracks[PlayerPrefs.GetInt("NextBGMTrack")];
			PlayerPrefs.SetInt("NextBGMTrack", PlayerPrefs.GetInt("NextBGMTrack")+1);
			if(PlayerPrefs.GetInt("NextBGMTrack") >= mBGMTracks.Length)
			{
				PlayerPrefs.SetInt("NextBGMTrack", 0);
			}
		}
		else
		{
			PlayerPrefs.SetInt("NextBGMTrack", 0);
		}
		mBGMSource.Play();
		#endregion

		StartCoroutine(StartupSequence());
		mP1Animator = mPlayer1.GetComponent<Animator>();
		mP2Animator = mPlayer2.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(mCurrentMatchState == eMatchState.PLAYERINPUT)
		{
			if(!(mP1Moves.Count >=3 && mP2Moves.Count >=3))
			{
				TakeP1Input();
				TakeP2Input();
			}
			else
			{
				mCurrentMatchState = eMatchState.INPUTRESOLVE;
				StartCoroutine(InputResolveSequence());
			}
		}

		if(mCurrentMatchState != eMatchState.GAMEOVER)
		{
			float playerMidpoint = (mPlayer1.transform.position.x+mPlayer2.transform.position.x)/2f;
			playerMidpoint = Mathf.Clamp(playerMidpoint,-7.5f, 7.5f);
			Camera.main.transform.position = new Vector3(playerMidpoint, 1.1f, -9.5f);
		}
		else if (mLosingPlayerTransform != null)
		{
			//if(Camera.main.transform.position.y > -2.45f)
			//{
				Vector3 cameraTarget = new Vector3(mLosingPlayerTransform.position.x, mLosingPlayerTransform.position.y, -9.5f);
				Camera.main.transform.Translate((cameraTarget-Camera.main.transform.position)*Time.deltaTime);
			//}
		}

	}

	IEnumerator StartupSequence()
	{
		yield return new WaitForSeconds(1f);
		mCurrentMatchState = eMatchState.PLAYERINPUT;
	}

	IEnumerator InputResolveSequence()
	{
		yield return new WaitForSeconds(1f);
		while(mP1Moves.Count >0 && mP2Moves.Count >0 && mCurrentMatchState==eMatchState.INPUTRESOLVE)
		{
			ComparePlayerMoves();
			yield return new WaitForSeconds(1.2f);
			if(PlayerOutCheck())
			{
				mCurrentMatchState = eMatchState.GAMEOVER;
				StartCoroutine(GameOverSequence());

			}
		}
		if(!PlayerOutCheck())
		{
			yield return new WaitForSeconds(1f);
			mCurrentMatchState = eMatchState.PLAYERINPUT;
		}
	}

	IEnumerator GameOverSequence()
	{
		Debug.Log("Game Over!");
		yield return new WaitForSeconds(3f);
		//Application.LoadLevel(Application.loadedLevel);
		mRematchMessage.SetActive(true);
	}
	void ComparePlayerMoves()
	{
		if(mP1Moves.Count >0)
		{
			switch(mP1Moves[0])
			{
			#region resolutions if Player 1 Charges
			case ePlayerMove.CHARGE:
				switch (mP2Moves[0])
				{
				case ePlayerMove.CHARGE:
					//nothing on a tie ~Adam
					mP1Animator.Play("PlayerCharge");
					mP2Animator.Play("PlayerCharge");
					mSFXSource.PlayOneShot(mChargeHit);
					break;
				case ePlayerMove.COUNTER:
					mP1Animator.Play("PlayerCharge");
					mP2Animator.Play("PlayerCounter");
					mSFXSource.PlayOneShot(mCounterThrow);
					mPlayer1.GetCountered();
					mPlayer1.CounterHappened();
					mPlayer2.CounterHappened();
					break;
				case ePlayerMove.UPPERCUT:
					mP1Animator.Play("PlayerCharge");
					mP2Animator.Play("PlayerKnockback");
					mSFXSource.PlayOneShot(mUpperCutHit);
					mPlayer1.MoveForward();
					mPlayer2.MoveBackward();
					break;
				}
				break;
				#endregion
				#region resolutions if Player 1 Counters
			case ePlayerMove.COUNTER:
				switch (mP2Moves[0])
				{
				case ePlayerMove.CHARGE:
					mP1Animator.Play("PlayerCounter");
					mP2Animator.Play("PlayerCharge");
					mSFXSource.PlayOneShot(mCounterThrow);
					mPlayer2.GetCountered();
					mPlayer1.CounterHappened();
					mPlayer2.CounterHappened();
					break;
				case ePlayerMove.COUNTER:
					//nothing on a tie ~Adam
					mP1Animator.Play("PlayerCounter");
					mP2Animator.Play("PlayerCounter");
					mSFXSource.PlayOneShot(mCounterThrow);
					break;
				case ePlayerMove.UPPERCUT:
					mP1Animator.Play("PlayerKnockback");
					mP2Animator.Play("PlayerUppercut");
					mSFXSource.PlayOneShot(mUpperCutHit);
					mPlayer1.MoveBackward();
					mPlayer2.MoveForward();
					break;
				}
				break;
				#endregion
				#region resolutions if Player 1 Uppercuts
			case ePlayerMove.UPPERCUT:
				switch (mP2Moves[0])
				{
				case ePlayerMove.CHARGE:
					mP1Animator.Play("PlayerKnockback");
					mP2Animator.Play("PlayerCharge");
					mSFXSource.PlayOneShot(mChargeHit);
					mPlayer1.MoveBackward();
					mPlayer2.MoveForward();
					break;
				case ePlayerMove.COUNTER:
					mP1Animator.Play("PlayerUppercut");
					mP2Animator.Play("PlayerKnockback");
					mSFXSource.PlayOneShot(mUpperCutHit);
					mPlayer1.MoveForward();
					mPlayer2.MoveBackward();
					break;
				case ePlayerMove.UPPERCUT:
					//nothing on a tie ~Adam
					mP1Animator.Play("PlayerUppercut");
					mP2Animator.Play("PlayerUppercut");
					mSFXSource.PlayOneShot(mUpperCutHit);
					break;
				}
				break;
				#endregion
			}
			//Clean out the move that was just performed from the queue ~Adam
			mP1Moves.RemoveAt(0);
			mP2Moves.RemoveAt(0);
		}

	}

	bool PlayerOutCheck()
	{
		bool playerOut = false;
		if(mPlayer1.GetCurrentPosition() ==0 || mPlayer1.GetCurrentPosition() == 9 ||
			mPlayer2.GetCurrentPosition() ==0 || mPlayer2.GetCurrentPosition() == 9)
		{
			playerOut = true;
			if(mBGMSource.isPlaying)
			{
				mBGMSource.Stop();
				mEndMatchJingleSource.Play();
			}
		}
		return playerOut;
	}

	void TakeP1Input()
	{
		//Don't take input if there are already three moves in the queue ~Adam
		if(mP1Moves.Count<3)
		{
			if(Input.GetButtonDown("ChargeP1"))
			{
				mP1Moves.Add(ePlayerMove.CHARGE);
				mP1InputSFXSource.Play();
			}
			else if(Input.GetButtonDown("CounterP1"))
			{
				mP1Moves.Add(ePlayerMove.COUNTER);
				mP1InputSFXSource.Play();
			}
			if(Input.GetButtonDown("UppercutP1"))
			{
				mP1Moves.Add(ePlayerMove.UPPERCUT);
				mP1InputSFXSource.Play();
			}
		}
	}

	void TakeP2Input()
	{
		//Don't take input if there are already three moves in the queue ~Adam
		if(mP2Moves.Count<3)
		{
			if(Input.GetButtonDown("ChargeP2"))
			{
				mP2Moves.Add(ePlayerMove.CHARGE);
				mP2InputSFXSource.Play();
			}
			else if(Input.GetButtonDown("CounterP2"))
			{
				mP2Moves.Add(ePlayerMove.COUNTER);
				mP2InputSFXSource.Play();
			}
			if(Input.GetButtonDown("UppercutP2"))
			{
				mP2Moves.Add(ePlayerMove.UPPERCUT);
				mP2InputSFXSource.Play();
			}

		}
	}

	public eMatchState GetMatchState()
	{
		return mCurrentMatchState;
	}

	//Return how many moves the players have in their queues ~Adam
	public int GetP1MoveCount()
	{
		return mP1Moves.Count;
	}
	public int GetP2MoveCount()
	{
		return mP2Moves.Count;
	}

	//Get the position of the losing player so the camera can follow their fall ~Adam
	public void SetLoserTransform(Transform loserTransform)
	{
		mLosingPlayerTransform = loserTransform;
	}
}
