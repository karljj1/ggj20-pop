using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
	public Slider slider;

    [Range(0.01F, 0.2F)]
	public float lossRate = 0.1f;

    [Range(0, 1)]
    public float startHealth = 1;

    private void Start()
    {
        slider.value = startHealth;
    }

    void Update()
	{
        slider.value = Mathf.MoveTowards(slider.value, 0, lossRate * Time.deltaTime);
	}
}
