using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour 
{
	[SerializeField] private MatchController mMatchController;
	[SerializeField] private int mCurrentPosition;

	[SerializeField] private bool mFacingRight;

	[SerializeField] private PlayerController mOtherPlayer;


	[SerializeField] GameObject mWindowShardsRight;
	[SerializeField] GameObject mWindowShardsLeft;
	[SerializeField] SpriteRenderer mWindowLeft;
	[SerializeField] SpriteRenderer mWindowRight;
	[SerializeField] Sprite mBrokenWindowSprite;

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
		Vector3 targetPosition = new Vector3(mMatchController.mPositionSlots[mCurrentPosition].position.x, transform.position.y, transform.position.z);
		yield return new WaitForSeconds(0.1f);
		while(Vector3.Distance(transform.position, mMatchController.mPositionSlots[mCurrentPosition].position)>0.2f)
		{
			targetPosition = new Vector3(mMatchController.mPositionSlots[mCurrentPosition].position.x, transform.position.y, transform.position.z);
			transform.Translate((targetPosition-transform.position)*Time.deltaTime);
			#region When the player crosses the window, break it and turn on its particle effects, and start falling animation
			if(transform.position.x > 7.8f)
			{
				mMatchController.SetLoserTransform(this.transform);
				if(mCurrentPosition == 0 || mCurrentPosition == 9)
				{
					GetComponent<Animator>().Play("PlayerFall");
				}
				mWindowRight.sprite = mBrokenWindowSprite;
				mWindowShardsRight.SetActive(true);
			}
			else if (transform.position.x < -7.8f)
			{
				mMatchController.SetLoserTransform(this.transform);
				if(mCurrentPosition == 0 || mCurrentPosition == 9)
				{
					GetComponent<Animator>().Play("PlayerFall");
				}
				mWindowLeft.sprite = mBrokenWindowSprite;
				mWindowShardsLeft.SetActive(true);
			}
			#endregion
			//start actually falling ~Adam
			if((transform.position.x<-9f||transform.position.x>9f)&&transform.position.y > -1f)
			{
				GetComponent<Rigidbody2D>().gravityScale=0.5f;
			}
			yield return new WaitForSeconds(Time.deltaTime);
		}
		transform.position = targetPosition;


	}



}
