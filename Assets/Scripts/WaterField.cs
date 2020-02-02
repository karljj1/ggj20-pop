using UnityEngine;

public class WaterField : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.tag == "Player")
		{
            collision.GetComponent<SeedCharacterController>().Heal();
            Destroy(gameObject);
		}
	}
}
