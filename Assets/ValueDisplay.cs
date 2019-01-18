using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ValueDisplay : MonoBehaviour
{
	[Space(30)]
	public float value;
	[Space(30)]

	public TMP_Text text;

	void Update()
	{
		SetValue(value, "0");
	}

	public void SetValue(float value, string format = "0")
	{
		this.value = value;
		text.text = value.ToString(format);
	}
}
