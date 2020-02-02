using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedDoor : MonoBehaviour
{
	Animator anim;

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
}
