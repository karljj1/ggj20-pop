using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
	public Slider slider;
    static Healthbar s_Instance;

    [Range(0.01F, 0.2F)]
	public float lossRate = 0.1f;

    [Range(0, 1)]
    public float startHealth = 1;

    public static float CurrentHealth
    {
        get => s_Instance.slider.value;
        set => s_Instance.slider.value = value;
    }

    private void Start()
    {
        s_Instance = this;
        slider.value = startHealth;
    }

    void Update()
	{
        slider.value = Mathf.MoveTowards(slider.value, 0, lossRate * Time.deltaTime);
	}
}
