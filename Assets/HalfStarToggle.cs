using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalfStarToggle : MonoBehaviour
{

	public bool state;
	[Space(10)]
	public GameObject sprite;

	bool cachedState;

	void Start()
	{
		cachedState = state;
		SetState(state);
	}

	private void Update()
	{
		if (cachedState != state)
		{
			SetState(state);
		}
	}

	public void SetState(bool state)
	{
		this.state = state;
		this.cachedState = state;

		sprite.SetActive(state);
	}
}
