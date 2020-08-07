using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

	public Slider slider;
	public Gradient gradient;
	public Color poisioned = new Color();
	public bool isPoisioned;
	public Image fill;

	public void SetMaxHealth(int health)
	{
		poisioned.a = 1f;
		slider.maxValue = health;
		slider.value = health;

		if (isPoisioned)
		{
			fill.color = poisioned;
		}
		else
		{
			fill.color = gradient.Evaluate(1f);
		}
		
	}

    public void SetHealth(int health)
	{
		poisioned.a = 1f;
		slider.value = health * 1f;

		if (isPoisioned)
		{
			fill.color = poisioned;
		}
		else
		{
			fill.color = gradient.Evaluate(slider.normalizedValue);
		}
		
	}

}
