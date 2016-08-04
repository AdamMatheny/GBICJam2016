using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour 
{
	[SerializeField] private MatchController mMatchController;
	[SerializeField] private int mCurrentPosition;

	[SerializeField] private bool mFacingRight;

	[SerializeField] private PlayerController mOtherPlayer;


	[SerializeField] GameObject mWindowShardsRight;
	[SerializeField] GameObject mWindowShardsLeft;

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
		GetComponent<SpriteRenderer>().flipX = !mFacingRight;
	}


	public int GetCurrentPosition()
	{
		//Debug.Log(gameObject.name + " is at position " + mCurrentPosition);
		return mCurrentPosition;
	}

	IEnumerator MoveToNewPosition()
	{
		yield return new WaitForSeconds(0.1f);
		while(Vector3.Distance(transform.position, mMatchController.mPositionSlots[mCurrentPosition].position)>0.2f)
		{
			Debug.Log("Moving " + gameObject.name + " over to position " + mCurrentPosition);
			transform.Translate((mMatchController.mPositionSlots[mCurrentPosition].position-transform.position)*Time.deltaTime);
			if(transform.position.x > 7.8f)
			{
				if(mCurrentPosition == 0 || mCurrentPosition == 9)
				{
					GetComponent<Animator>().Play("PlayerFall");
				}
				mWindowShardsRight.SetActive(true);
			}
			else if (transform.position.x < -7.8f)
			{
				if(mCurrentPosition == 0 || mCurrentPosition == 9)
				{
					GetComponent<Animator>().Play("PlayerFall");
				}
				mWindowShardsLeft.SetActive(true);
			}
			if(transform.position.x<-9f||transform.position.x>9f)
			{
				GetComponent<Rigidbody2D>().gravityScale=0.225f;
			}
			yield return new WaitForSeconds(Time.deltaTime);
		}
		transform.position = mMatchController.mPositionSlots[mCurrentPosition].position;

	}



}
