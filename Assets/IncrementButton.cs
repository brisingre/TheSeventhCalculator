using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class IncrementButton : MonoBehaviour
{
	public TMP_InputField field;
	// Start is called before the first frame update

	public void Add()
	{
		string text = field.text;
		int count = System.Convert.ToInt32(text);
		count++;
		field.text = count.ToString();
	}

	public void Subtract()
	{
		string text = field.text;
		int count = System.Convert.ToInt32(text);
		if(count > 0)
			count--;
		field.text = count.ToString();
	}
}
