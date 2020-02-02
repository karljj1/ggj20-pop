using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.InputSystem.InputAction;

public class CharacterController : MonoBehaviour
{
	#region References
	public Transform cameraFollowTarget;
	public SpriteRenderer spriteRenderer;
	private Rigidbody2D rb;
	public Animator anim;
	Collider2D _collider;
	LayerMask groundMask;
	LayerMask platformMask;
	public GameObject seedObject;
	public GameObject budObject;
	public GameObject bloomObject;
	List<Collider2D> playerColliders = new List<Collider2D>();
	#endregion

	#region Public variables
	public float moveSpeed = 10f;
	public float fallSpeedMultiplier;
	public float jumpForce;
	public float bloomToBudWaitTime;
	public float budToSeedWaitTime;
	public float seedToPop;
	public int startingBloomState;

	//public float cameraHorizontalFacingOffset;
	//public float cameraHorizontalSpeedOffset;
	//public float cameraVerticalInputOffset;
	//public float maxHorizontalDeltaDampTime;
	//public float maxVerticalDeltaDampTime;
	//public float verticalCameraOffsetDelay;
	#endregion

	#region Private variables
	InputSystem m_Controls;
	float m_MoveDir;
	bool canJump;
	Collider2D currentPassedPlatformCollider;
	bool passingThrough;
	IEnumerator passingNowCoruitne;

	Vector2 SeedColliderOffset;
	Vector2 seedColliderHeight;
	#endregion

	void Awake()
	{
		m_Controls = new InputSystem();
	}

	public void OnEnable()
	{
		rb = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		m_Controls.Enable();
		playerColliders.AddRange(GetComponentsInChildren<Collider2D>());
		//_collider = GetComponent<Collider2D>();
		groundMask = LayerMask.GetMask("Ground");
		platformMask = LayerMask.GetMask("Platform");
	}

	void OnDisable()
	{
		m_Controls.Disable();
	}

	private void Start()
	{
		//anim.SetInteger("FlowerForm", startingBloomState);

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

		BloomStateOneFrameChecks();

		SeedColliderOffset = new Vector2 (0, 0);
		seedColliderHeight = new Vector2(1, 1.5f);
	}

	private void Update()
	{
		CheckIfGrounded();
		CheckPassThroughPlatform();
		DebugFlowerState();
	}

	private void FixedUpdate()
	{
		MotionPhysics();
	}

	public void Move(CallbackContext evt)
	{
		m_MoveDir = evt.ReadValue<float>();
	}



	public void Jump(CallbackContext evt)
	{
		if (evt.phase == UnityEngine.InputSystem.InputActionPhase.Started && canJump)
		{
			rb.velocity = Vector2.up * jumpForce;
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

	IEnumerator PassingThroughPlatform()
	{
		//_collider.isTrigger = true;

		foreach (Collider2D collider in playerColliders)
		{
			collider.isTrigger = true;
		}




		yield return new WaitForSeconds(.2f);


		//while (passingThrough)
		//{
		//	yield return null;
		//}

		foreach (Collider2D collider in playerColliders)
		{
			collider.isTrigger = false;
		}

		//_collider.isTrigger = false;
		StopCoroutine(passingNowCoruitne);
	}
	

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

	void CheckIfGrounded()
	{
		if (Physics2D.Raycast(transform.position, Vector2.down, transform.localScale.y / 2 + 0.1f, groundMask))
			canJump = true;
		else
			canJump = false;

		//if (Physics2D.CheckCapsule(_collider.bounds.center, new Vector3(_collider.bounds.center.x, _collider.bounds.min.y - 0.1f, _collider.bounds.center.z), 0.18f))
		//{
		//	canJump = false;
		//}
		//else
		//	canJump = true;
	}

	public bool CheckPassThroughPlatform()
	{
		return (Physics2D.Raycast(transform.position, Vector2.down, transform.localScale.y / 2 + 0.1f, platformMask));
	}

	//public void PassThroughPlatformsCheck()
	//{
	//	if (Physics2D.Raycast(transform.position, Vector2.down, transform.localScale.y / 2 + 0.1f, platformMask))
	//		canPassThroughPlatform = true;
	//	else
	//		canPassThroughPlatform = false;
	//}



	private void OnTriggerEnter(Collider other)
	{
		//if (other.GetComponent<LayerMask>() == platformMask)
		//{
		//	passingThrough = true;
		//}
	}

	//private void OnTriggerExit2D(Collider2D collision)
	//{
	//	if (collision.GetComponent<LayerMask>() == platformMask)
	//	{
	//		passingThrough = false;
	//	}
	//}


	IEnumerator BloomingToBudding()
	{
		yield return new WaitForSeconds(bloomToBudWaitTime);
		bloomState = BloomStates.Bud;
		BloomStateOneFrameChecks();
	}

	IEnumerator BuddingTosSeedling()
	{
		yield return new WaitForSeconds(budToSeedWaitTime);
		bloomState = BloomStates.Seed;
		BloomStateOneFrameChecks();
	}

	IEnumerator SeedlingToPop()
	{
		yield return new WaitForSeconds(budToSeedWaitTime);
		//Death
	}


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
	}

	public enum BloomStates { Bloom, Bud, Seed, Misc}

	public BloomStates bloomState;
}