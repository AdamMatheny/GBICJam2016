using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MatchController : MonoBehaviour 
{
	enum eMatchState {MATCHSTART, PLAYERINPUT, INPUTRESOLVE, GAMEOVER};
	enum ePlayerMove {CHARGE, COUNTER, UPPERCUT};

	[SerializeField] private List<ePlayerMove> mP1Moves = new List<ePlayerMove>();
	[SerializeField] private List<ePlayerMove> mP2Moves = new List<ePlayerMove>();

	[SerializeField] private eMatchState mCurrentMatchState = eMatchState.MATCHSTART;

	[SerializeField] PlayerController mPlayer1;
	[SerializeField] PlayerController mPlayer2;

	Animator mP1Animator;
	Animator mP2Animator;

	public Transform[] mPositionSlots;


	// Use this for initialization
	void Start () 
	{
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
		Application.LoadLevel(Application.loadedLevel);
	}
	void ComparePlayerMoves()
	{
		if(mP1Moves.Count >0)
		{
			switch(mP1Moves[0])
			{
			case ePlayerMove.CHARGE:
				switch (mP2Moves[0])
				{
				case ePlayerMove.CHARGE:
					//nothing on a tie ~Adam
					mP1Animator.Play("PlayerCharge");
					mP2Animator.Play("PlayerCharge");
					break;
				case ePlayerMove.COUNTER:
					mP1Animator.Play("PlayerCharge");
					mP2Animator.Play("PlayerCounter");
					mPlayer1.GetCountered();
					mPlayer1.CounterHappened();
					mPlayer2.CounterHappened();
					break;
				case ePlayerMove.UPPERCUT:
					mP1Animator.Play("PlayerCharge");
					mP2Animator.Play("PlayerKnockback");
					mPlayer1.MoveForward();
					mPlayer2.MoveBackward();
					break;
				}
				break;
			case ePlayerMove.COUNTER:
				switch (mP2Moves[0])
				{
				case ePlayerMove.CHARGE:
					mP1Animator.Play("PlayerCounter");
					mP2Animator.Play("PlayerCharge");
					mPlayer2.GetCountered();
					mPlayer1.CounterHappened();
					mPlayer2.CounterHappened();
					break;
				case ePlayerMove.COUNTER:
					//nothing on a tie ~Adam
					mP1Animator.Play("PlayerCounter");
					mP2Animator.Play("PlayerCounter");
					break;
				case ePlayerMove.UPPERCUT:
					mP1Animator.Play("PlayerKnockback");
					mP2Animator.Play("PlayerUppercut");
					mPlayer1.MoveBackward();
					mPlayer2.MoveForward();
					break;
				}
				break;
			case ePlayerMove.UPPERCUT:
				switch (mP2Moves[0])
				{
				case ePlayerMove.CHARGE:
					mP1Animator.Play("PlayerKnockback");
					mP2Animator.Play("PlayerCharge");
					mPlayer1.MoveBackward();
					mPlayer2.MoveForward();
					break;
				case ePlayerMove.COUNTER:
					mP1Animator.Play("PlayerUppercut");
					mP2Animator.Play("PlayerKnockback");
					mPlayer1.MoveForward();
					mPlayer2.MoveBackward();
					break;
				case ePlayerMove.UPPERCUT:
					//nothing on a tie ~Adam
					mP1Animator.Play("PlayerUppercut");
					mP2Animator.Play("PlayerUppercut");
					break;
				}
				break;
			}
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
		}
		return playerOut;
	}

	void TakeP1Input()
	{
		if(mP1Moves.Count<3)
		{
			if(Input.GetButtonDown("ChargeP1"))
			{
				mP1Moves.Add(ePlayerMove.CHARGE);
			}
			else if(Input.GetButtonDown("CounterP1"))
			{
				mP1Moves.Add(ePlayerMove.COUNTER);
			}
			if(Input.GetButtonDown("UppercutP1"))
			{
				mP1Moves.Add(ePlayerMove.UPPERCUT);
			}
		}
	}

	void TakeP2Input()
	{
		if(mP2Moves.Count<3)
		{
			if(Input.GetButtonDown("ChargeP2"))
			{
				mP2Moves.Add(ePlayerMove.CHARGE);
			}
			else if(Input.GetButtonDown("CounterP2"))
			{
				mP2Moves.Add(ePlayerMove.COUNTER);
			}
			if(Input.GetButtonDown("UppercutP2"))
			{
				mP2Moves.Add(ePlayerMove.UPPERCUT);
			}
		}
	}
}
