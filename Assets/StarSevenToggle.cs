using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum StarSevenState
{
	OFF,
	STAR,
	SEVEN,
	SEVENSTAR
}
public class StarSevenToggle : MonoBehaviour
{
	public StarSevenState state;
	[Space(10)]
	public GameObject star;
	public GameObject seven;

	StarSevenState cachedState;

    void Start()
	{
		cachedState = state;
		SetState(state);
    }

	private void Update()
	{
		if(cachedState != state)
		{
			SetState(state);
		}
	}

	public void SetState(StarSevenState state)
	{
		this.state = state;
		this.cachedState = state;

		switch(state)
		{
			case StarSevenState.OFF:
				star.SetActive(false);
				seven.SetActive(false);
				break;
			case StarSevenState.STAR:
				star.SetActive(true);
				seven.SetActive(false);
				break;
			case StarSevenState.SEVEN:
				star.SetActive(false);
				seven.SetActive(true);
				break;
			case StarSevenState.SEVENSTAR:
				star.SetActive(true);
				seven.SetActive(true);
				break;
		}
	}
}
