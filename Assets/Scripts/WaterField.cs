using UnityEngine;

public class WaterField : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D collision)
	{
        Debug.Log("Enter water");
		if (collision.tag == "Player")
		{
            collision.GetComponent<SeedCharacterController>().Heal();
            Destroy(gameObject);
		}
	}
}
