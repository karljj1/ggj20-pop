using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.InputSystem.InputAction;

public class SeedCharacterController : MonoBehaviour
{
    [System.Serializable]
    public class SeedState
    {
        public GameObject visuals;
        public float timeStart;
        public float blinkStart;
        public float jumpForce;
    }

    public List<SeedState> m_States;
    int m_CurrentState;
    SeedState m_ActiveState;
    public GameObject popEffect;

    #region References
    public Transform cameraFollowTarget;
	public SpriteRenderer spriteRenderer;
	private Rigidbody2D rb;
	public Animator anim;
	LayerMask groundMask;
	LayerMask platformMask;
	LayerMask waterMask;
	public GameObject seedObject;
	public GameObject budObject;
	public GameObject bloomObject;
	List<Collider2D> playerColliders = new List<Collider2D>();
	List<LockedDoor> doorCollection = new List<LockedDoor>();
	List<Key> keyCollection = new List<Key>();
	List<WaterField> waterCollection = new List<WaterField>();
	PlayerSpawner playerSpawner;
    #endregion

    #region Public variables
	public float moveSpeed = 10f;
	public float fallSpeedMultiplier;
	public float downgradeWaitTime1;
	public float downgradeWaitTime2;
	public int startingBloomState;
	public int keys;
	#endregion
	
	#region Private variables
	public float m_MoveDir;
	bool canJump;
	Collider2D currentPassedPlatformCollider;
	bool passingThrough;
	IEnumerator passingNowCoruitne;

	Vector2 SeedColliderOffset;
	Vector2 seedColliderHeight;
	#endregion

	#region methods
	#region starters
	public void OnEnable()
	{
		rb = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		playerSpawner = FindObjectOfType<PlayerSpawner>();
		playerColliders.AddRange(GetComponentsInChildren<Collider2D>());
		doorCollection.AddRange(FindObjectsOfType<LockedDoor>());
		keyCollection.AddRange(FindObjectsOfType<Key>());
		waterCollection.AddRange(FindObjectsOfType<WaterField>());
		//_collider = GetComponent<Collider2D>();
		groundMask = LayerMask.GetMask("Ground");
		platformMask = LayerMask.GetMask("Platform");
		waterMask = LayerMask.GetMask("Water");
	}

	private void Start()
	{
        for(int i = 0; i < transform.childCount; ++i)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }


        //anim.SetInteger("FlowerForm", startingBloomState);
        anim = GetComponentInChildren<Animator>();
        m_CurrentState = 0;
        m_ActiveState = null;
        ActivateState(m_CurrentState);

        switch (startingBloomState)
		{
			case 0:
				bloomState = BloomStates.Seed;
				break;
			case 1:
				bloomState = BloomStates.Bud;
				break;
			case 2:
				bloomState = BloomStates.Bloom;
				break;
			default:
				break;
		}

		//BloomStateOneFrameChecks();

		SeedColliderOffset = new Vector2 (0, 0);
		seedColliderHeight = new Vector2(1, 1.5f);
	}
	#endregion

	private void Update()
	{
        UpdateGrowingState();
		CheckContacts();
		//DebugFlowerState();
	}

	private void FixedUpdate()
	{
		MotionPhysics();
	}
	#endregion

	#region Inputs

	public void Move(CallbackContext evt)
	{
		m_MoveDir = evt.ReadValue<float>();
	}

	public void Jump(CallbackContext evt)
	{
		if (evt.phase == UnityEngine.InputSystem.InputActionPhase.Started && canJump)
		{
			rb.velocity = Vector2.up * m_ActiveState.jumpForce;
		}
	}

	public void Dance(CallbackContext evt)
	{
		if (evt.phase == UnityEngine.InputSystem.InputActionPhase.Started)
		{
			print("Dancing");
		}
	}

	public void StartPassThroughPlatform(CallbackContext evt)
	{
		var canPassThroughPlatform = CheckPassThroughPlatform();
		passingNowCoruitne = PassingThroughPlatform();
		if (evt.phase == UnityEngine.InputSystem.InputActionPhase.Started && canPassThroughPlatform)
		{
			StartCoroutine(passingNowCoruitne);
		}
	}

	public void UpdateFaceDirection(CallbackContext evt)
	{
		if (evt.phase == UnityEngine.InputSystem.InputActionPhase.Performed)
		{
			transform.localScale = new Vector3(m_MoveDir, transform.localScale.y, transform.localScale.z);
		}
	}
	#endregion

	IEnumerator PassingThroughPlatform()
	{
		foreach (Collider2D collider in playerColliders)
		{
			collider.isTrigger = true;
		}

		yield return new WaitForSeconds(.2f);

		foreach (Collider2D collider in playerColliders)
		{
			collider.isTrigger = false;
		}
		StopCoroutine(passingNowCoruitne);
	}


	#region Physics
	void MovementPhysics()
	{
		rb.velocity = new Vector2(m_MoveDir * moveSpeed, rb.velocity.y);
	}

	void MotionPhysics()
	{
		MovementPhysics();
		FallPhysics();
	}

	void FallPhysics()
	{
		if (rb.velocity.y < 0)
			rb.velocity += Vector2.up * Physics2D.gravity.y * (fallSpeedMultiplier - 1) * Time.deltaTime;
	}
	#endregion

	#region Contact Checks

	void CheckContacts()
	{
		CheckIfGrounded();
		CheckPassThroughPlatform();
	}

	void CheckIfGrounded()
	{
		if (Physics2D.Raycast(transform.position, Vector2.down, transform.localScale.y / 2 + 0.1f, groundMask))
			canJump = true;
		else if (Physics2D.Raycast(transform.position, Vector2.down, transform.localScale.y / 2 + 0.1f, platformMask))
            canJump = true;
        else
            canJump = false;
	}

    public void Heal()
    {
        m_CurrentState = Mathf.Max(0, m_CurrentState - 1);
        ActivateState(m_CurrentState);
        Healthbar.CurrentHealth = m_ActiveState.timeStart;
    }

    void UpdateGrowingState()
    {
        if (m_CurrentState < m_States.Count - 1)
        {
            var nextState = m_States[m_CurrentState + 1];
            if (Healthbar.CurrentHealth <= nextState.timeStart)
            {
                anim.SetInteger("BlinkState", 0);
                m_CurrentState++;
                ActivateState(m_CurrentState);
            }
            else if (Healthbar.CurrentHealth <= nextState.blinkStart)
            {
                anim.SetInteger("BlinkState", 2);
            }
        }
        else
        {
            if (Healthbar.CurrentHealth <= 0)
            {
                m_ActiveState.visuals.SetActive(false);
                Instantiate(popEffect, m_ActiveState.visuals.transform.position, Quaternion.identity);
                enabled = false;
            }
        }
    }

    void ActivateState(int state)
    {
       // Disable the old state if we have one
       if (m_ActiveState != null)
        {
            m_ActiveState.visuals.SetActive(false);
        }

        m_ActiveState = m_States[state];
        anim = m_ActiveState.visuals.GetComponent<Animator>();
        m_ActiveState.visuals.SetActive(true);
    }
    /*
	public IEnumerator GrowingTransition()
	{
		GetAnimator();
		anim.SetInteger("BlinkState", 1);
		yield return new WaitForSeconds(0.3f);
		anim.SetInteger("BlinkState", 0);
		RepairPoppy();
		WaterField foundWaterfield;
		foundWaterfield = Physics2D.Raycast(transform.position, Vector2.down, transform.localScale.y / 2 + 0.1f, waterMask).collider.GetComponent<WaterField>();
		foundWaterfield.gameObject.SetActive(false);
	}
    */

	public bool CheckPassThroughPlatform()
	{
		return (Physics2D.Raycast(transform.position, Vector2.down, transform.localScale.y / 2 + 0.1f, platformMask));
	}
	#endregion
      /*
	#region BloomingTransitions

	IEnumerator BloomingToBudding()
	{
		GetAnimator();
		anim.SetInteger("BlinkState", 0);
		yield return new WaitForSeconds(downgradeWaitTime1);
		//GetAnimator();
		anim.SetInteger("BlinkState", 2);
		yield return new WaitForSeconds(downgradeWaitTime2);
		bloomState = BloomStates.Bud;
		BloomStateOneFrameChecks();
	}

	IEnumerator BuddingTosSeedling()
	{
		GetAnimator();
		anim.SetInteger("BlinkState", 0);
		yield return new WaitForSeconds(downgradeWaitTime1);
		anim.SetInteger("BlinkState", 2);
		yield return new WaitForSeconds(downgradeWaitTime2);
		bloomState = BloomStates.Seed;
		BloomStateOneFrameChecks();
	}
    
	IEnumerator SeedlingToPop()
	{
		yield return new WaitForSeconds(downgradeWaitTime1);
		yield return new WaitForSeconds(downgradeWaitTime2);
		//Death
	}
	#endregion
    
	void BloomStateOneFrameChecks()
	{
		StopAllCoroutines();
		foreach  (Collider2D collider in playerColliders)
		{
			collider.isTrigger = false;
		}

		//_collider.isTrigger = false;
		seedObject.SetActive(false);
		budObject.SetActive(false);
		bloomObject.SetActive(false);
		switch (bloomState)
		{
			case BloomStates.Bloom:
				bloomObject.SetActive(true);
				StartCoroutine("BloomingToBudding");
				//anim.SetInteger("FlowerForm", 2);

				break;
			case BloomStates.Bud:
				budObject.SetActive(true);
				StartCoroutine("BuddingTosSeedling");
				//anim.SetInteger("FlowerForm", 1);
				//_collider.offset = new Vector2(1, .5f);
				//_collider.transform.localScale = new Vector2(1, 2);
				break;
			case BloomStates.Seed:
				seedObject.SetActive(true);
				StartCoroutine("SeedlingToPop");
				//anim.SetInteger("FlowerForm", 0);
				//_collider.offset = SeedColliderOffset;
				//_collider.transform.localScale = seedColliderHeight;
				break;
			case BloomStates.Misc:
				break;
			default:
				break;
		}
	}
    */
    /*
	void RepairPoppy()
	{
		switch (bloomState)
		{
			case BloomStates.Bloom:
				bloomState = BloomStates.Bloom;
				break;
			case BloomStates.Bud:
				bloomState = BloomStates.Bloom;
				break;
			case BloomStates.Seed:
				bloomState = BloomStates.Bud;
				break;
			case BloomStates.Misc:
				break;
			default:
				break;
		}

		BloomStateOneFrameChecks();
	}
    */
    /*
	void DebugFlowerState()
	{
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			bloomState = BloomStates.Seed;
			BloomStateOneFrameChecks();
		}
		else if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			bloomState = BloomStates.Bud;
			BloomStateOneFrameChecks();
		}
		else if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			bloomState = BloomStates.Bloom;
			BloomStateOneFrameChecks();
		}

		if (Input.GetKeyDown(KeyCode.O))
		{
			Death();
		}
	}
    */
	void Death()
	{
		foreach (WaterField item in waterCollection)
		{
			item.gameObject.SetActive(true);
		}

		foreach (LockedDoor item in doorCollection)
		{
			item.isOpen = false;
			anim.SetBool("doorOpen", false);
		}

		foreach (Key item in keyCollection)
		{
			item.gameObject.SetActive(true);
		}

		playerSpawner.PopPlayer();
	}
    
	public enum BloomStates { Bloom, Bud, Seed, Misc}

	public BloomStates bloomState;
}
