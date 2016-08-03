using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour 
{
	[SerializeField] private MatchController mMatchController;
	[SerializeField] private int mCurrentPosition;

	[SerializeField] private bool mFacingRight;

	[SerializeField] private PlayerController mOtherPlayer;

	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	//Called when Charge or Uppercut is successful ~Adam
	public void MoveForward()
	{
		if(mFacingRight)
		{
			mCurrentPosition++;
		}
		else
		{
			mCurrentPosition--;
		}
		StartCoroutine(MoveToNewPosition());
	}

	//Called when pushed back by Charge or Uppercut ~Adam
	public void MoveBackward()
	{
		if(mFacingRight)
		{
			mCurrentPosition--;
		}
		else
		{
			mCurrentPosition++;
		}
		StartCoroutine(MoveToNewPosition());
	}

	//Called when losing to Counter ~Adam
	public void GetCountered()
	{
		//Debug.Log (gameObject.name + "Got countered and other player is at "+ mOtherPlayer.GetCurrentPosition());

		if(mFacingRight)
		{
			mCurrentPosition = mOtherPlayer.GetCurrentPosition()+1;
		}
		else
		{
			mCurrentPosition = mOtherPlayer.GetCurrentPosition()-1;
		}
		StartCoroutine(MoveToNewPosition());
	}

	//Flip the direction the player is facing after a succesfull Counter occurs for either player ~Adam
	public void CounterHappened()
	{
		mFacingRight = !mFacingRight;
		transform.Rotate(0,180,0);
	}


	public int GetCurrentPosition()
	{
		//Debug.Log(gameObject.name + " is at position " + mCurrentPosition);
		return mCurrentPosition;
	}

	IEnumerator MoveToNewPosition()
	{
		yield return new WaitForSeconds(0.1f);
		while(Vector3.Distance(transform.position, mMatchController.mPositionSlots[mCurrentPosition].position)<0.1f)
		{
			transform.Translate(Vector3.Normalize(transform.position-mMatchController.mPositionSlots[mCurrentPosition].position)*Time.deltaTime);
			yield return new WaitForSeconds(Time.deltaTime);
		}
		transform.position = mMatchController.mPositionSlots[mCurrentPosition].position;
	}



}
