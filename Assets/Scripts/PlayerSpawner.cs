using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{

	public GameObject _playerPrefab;

    // Start is called before the first frame update
    void Start()
    {
		SpawnPlayer();
	}

    // Update is called once per frame
    void Update()
    {
		if (Input.GetKeyDown(KeyCode.L))
		{
			SpawnPlayer();
		}
    }

	public void SpawnPlayer()
	{
		if (!GameObject.FindGameObjectWithTag("Player"))
			Instantiate(_playerPrefab, transform.position, Quaternion.identity);
	}

	public void PopPlayer()
	{
		Destroy(GameObject.FindGameObjectWithTag("Player"));
		StartCoroutine("RespawnPlayer");
	}

	IEnumerator RespawnPlayer()
	{
		yield return new WaitForSeconds(1.5f);
		SpawnPlayer();
	}


	private void OnDrawGizmos()
	{
		Gizmos.color = Color.magenta;
		Gizmos.DrawWireCube(transform.position, transform.localScale);
	}
}
