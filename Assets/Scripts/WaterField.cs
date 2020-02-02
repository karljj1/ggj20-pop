using UnityEngine;

public class WaterField : MonoBehaviour
{
    public int healAmount = 1;
	private void OnTriggerEnter2D(Collider2D collision)
	{
        Debug.Log("Enter water");
		if (collision.tag == "Player")
		{
            for(int i = 0; i < healAmount; ++i)
                collision.GetComponent<SeedCharacterController>().Heal();
            Destroy(gameObject);
		}
	}
}
