using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LockedDoor : MonoBehaviour
{
	Animator anim;
	public bool isOpen;

	private void Awake()
	{
		anim = GetComponent<Animator>();
		anim.SetBool("doorOpen", false);
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.tag == "Player" && collision.gameObject.GetComponent<SeedCharacterController>().keys > 0)
		{
			collision.gameObject.GetComponent<SeedCharacterController>().keys--;

			anim.SetBool("doorOpen", true);

			//Open door if time;
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{

		if (collision.tag == "Player" && isOpen)
		{
			SceneManager.LoadScene("EndCredits");
		}
		else if (collision.tag == "Player" && !isOpen && collision.gameObject.GetComponent<SeedCharacterController>().keys > 0)
		{
			anim.SetBool("doorOpen", true);
		}
	}
}
