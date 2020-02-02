using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndCredSceneChanger : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine("WaitForEndOfEndCredits");  
    }

    IEnumerator WaitForEndOfEndCredits()
    {
        yield return new WaitForSeconds(14);
        SceneManager.LoadScene(0);
    }

}
