using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardSettings : MonoBehaviour
{
	public bool curse;
	public bool amulet;
	public bool root;
	public bool right;
	public bool left;
	[Range(0, 3)]
	public int stars = 1;
	[Range(0, 3)]
	public int sevens = 0;
	public int count = 4;

	[Space(20)]
	public bool pushUpdates;
	public List<StarSevenToggle> starToggles = new List<StarSevenToggle>();

	public HalfStarToggle rightHalf;
	public HalfStarToggle leftHalf;
	public HalfStarToggle curseBackground;
	public HalfStarToggle amuletStar;
	public HalfStarToggle amuletBG;
	public TMP_InputField countField;

	void Start()
    {
		PushSettings();
    }


    void Update()
    {
		if (pushUpdates)
			PushSettings();
		else
			PullSettings();
	}

	void PushSettings()
	{
		rightHalf.SetState(right);
		leftHalf.SetState(left);
		if (curseBackground != null)
			curseBackground.SetState(curse);

		if (amuletStar != null)
			amuletStar.SetState(amulet);
		if (amuletBG != null)
			amuletBG.SetState(amulet || root);

		if (stars + sevens > starToggles.Count)
			Debug.LogError("Not enough seven/star toggles!");

		for (int i = 0; i < starToggles.Count; i++)
		{
			if(i < stars)
				starToggles[i].SetState(StarSevenState.STAR);
			else if (i < stars + sevens)
				starToggles[i].SetState(StarSevenState.SEVEN);
			else
				starToggles[i].SetState(StarSevenState.OFF);
		}

		countField.text = count.ToString();
	}

	void PullSettings()
	{
		right = rightHalf.state;
		left = leftHalf.state;
		if (curseBackground != null)
			curse = curseBackground.state;
		if (amuletStar != null)
			amulet = amuletStar.state;
		stars = 0;
		sevens = 0;


		for (int i = 0; i < starToggles.Count; i++)
		{
			switch(starToggles[i].state)
			{
				case StarSevenState.SEVEN:
					sevens++;
					break;
				case StarSevenState.STAR:
					stars++;
					break;
			}
		}

		count = System.Convert.ToInt32(countField.text);
	}

	public List<MonteCard> GetMonteCards()
	{
		List<MonteCard> retVal = new List<MonteCard>();

		for (int i = 0; i < count; i++)
		{
			retVal.Add(new MonteCard(stars, sevens, left, right, curse, amulet, root));
		}

		return retVal;
	}
}
