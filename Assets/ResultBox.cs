using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ResultBox : MonoBehaviour
{
	public float min = 0;
	public float max = 1;
	public Gradient gradient;

	[Space(30)]
	public float value;
	[Space(30)]

	public TMP_Text text;
	public Image background;

	void Start()
    {
        
    }

    void Update()
    {
		SetValue(value, "0");
    }

	public void SetValue(float value, string format = "0")
	{
		this.value = value;
		text.text = value.ToString(format);
		float percent = (value - min) / (max - min);
		background.color = gradient.Evaluate(percent);
	}
}
