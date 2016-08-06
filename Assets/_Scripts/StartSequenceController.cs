using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StartSequenceController : MonoBehaviour 
{
	[SerializeField] private Image mTitle;
	[SerializeField] private Image mSubtitle;
	[SerializeField] private Image mCredits;
	[SerializeField] private Image mInstructions;
	[SerializeField] private GameObject mControlsScreen;

	[SerializeField] private GameObject mDefenestratedMan;
	[SerializeField] private ParticleSystem mWindowShards;
	[SerializeField] private SpriteRenderer mBrokenWindow;
	[SerializeField] private Sprite mBrokenWindowSprite;

	[SerializeField] Vector3 mDefenstratedManTargetPos;
	[SerializeField] Vector2 mDefenestratedManVelocity2D;
	[SerializeField] float mDefenestratedManGravity;

	[SerializeField] Vector3 mCameraTargetPos;
	[SerializeField] float mCameraRiseTime = 3f;
	[SerializeField] float mTimeToFreeze = 5f;
	[SerializeField] float mTitleTime = 1f;
	[SerializeField] float mSubtitleTime=1f;
	[SerializeField] float mCreditsTime=1f;
	[SerializeField] float mInstructionsTime=1f;

	bool mCanStartGame = false;
	bool mShowingControlsScreen = false;

	// Use this for initialization
	void Start () 
	{
		StartCoroutine(StartupSequence());
	}
	
	// Update is called once per frame
	void Update () 
	{
		Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position,mCameraTargetPos,Time.deltaTime*0.5f);
		if(mCanStartGame && ! mShowingControlsScreen)
		{
			if(Input.GetKeyDown(KeyCode.Return))
			{
				//Application.LoadLevel(1);
				mControlsScreen.SetActive(true);
				StartCoroutine(ShowControlsScreen());
			}
			if(Input.GetKeyDown(KeyCode.Escape))
			{
				Application.Quit();
			}
		}
		if(mCanStartGame && mShowingControlsScreen)
		{
			if(Input.anyKeyDown)
			{
				Application.LoadLevel(1);
			}
		}
	}

	IEnumerator StartupSequence()
	{
		//Camera is rising ~Adam
		yield return new WaitForSeconds(mCameraRiseTime);
		//Man thrown from window~Adam
		mDefenestratedMan.GetComponent<Rigidbody2D>().velocity = mDefenestratedManVelocity2D;
		mDefenestratedMan.GetComponent<Rigidbody2D>().gravityScale = mDefenestratedManGravity;
		mBrokenWindow.sprite = mBrokenWindowSprite;
		mWindowShards.gameObject.SetActive(true);
		yield return new WaitForSeconds(mTimeToFreeze);
		//Freeze falling man and window shards and show the title;
		mDefenestratedMan.GetComponent<Rigidbody2D>().gravityScale = 0f;
		mDefenestratedMan.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
		mWindowShards.Pause();
		yield return new WaitForSeconds(mTitleTime);
		//Display the UI elements ~Adam
		mTitle.enabled = true;
		yield return new WaitForSeconds(mSubtitleTime);
		mSubtitle.enabled = true;
		yield return new WaitForSeconds(mCreditsTime);
		mCredits.enabled = true;
		yield return new WaitForSeconds(mInstructionsTime);
		mInstructions.enabled = true;
		mCanStartGame = true;
	}

	IEnumerator ShowControlsScreen()
	{
		yield return new WaitForSeconds(1f);
		mShowingControlsScreen = true;
	}
}
