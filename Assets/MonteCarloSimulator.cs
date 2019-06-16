using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

public class MonteCard
{
	public int stars;
	public int sevens;

	public bool left;
	public bool right;

	public bool curse;
	public bool amulet;
	public bool root;

	System.Random random;

	public MonteCard(int stars, int sevens, bool left, bool right, bool curse, bool amulet, bool root)
	{
		this.stars = stars;
		this.sevens = sevens;
		this.left = left;
		this.right = right;
		this.curse = curse;
		this.amulet = amulet;
		this.root = root;
	}
}

public class StarCount
{
	public List<int> StarsByDepth = new List<int>(); //StarsByDepth[i] is how many stars you'd get if you drew i cards

	public List<int> RequiredByDifficulty = new List<int>(); //RequiredByDifficulty[i] is how many cards you'd need to draw to pass a check of difficulty i;

	public int RequiredByInfinity = -1; //If a check can only be succeeded by things that succed it automatically, it has infinite difficulty.

	public List<bool> SafetyByDepth = new List<bool>(); //StarsByDepth[i] is how many stars you'd get if you drew i cards


	public StarCount(List<int> starsByDepth, List<int> requiredByDifficulty, int requiredByInfinity, List<bool> safetyByDepth)
	{
		StarsByDepth = starsByDepth;
		RequiredByDifficulty = requiredByDifficulty;
		RequiredByInfinity = requiredByInfinity;
		SafetyByDepth = safetyByDepth;
	}

	public bool PassesCheck(int draws, int difficulty)
	{
		if (difficulty >= RequiredByDifficulty.Count)
			return false;

		if (draws > SafetyByDepth.Count)
			return false;

		if (SafetyByDepth[draws] == false)
			return false;


		if (RequiredByDifficulty[difficulty] <= draws)
			return true;

		return false;
	}

	public int DeckSize
	{
		get { return StarsByDepth.Count; }
	}

	public int MaxSuccesses
	{
		get { return RequiredByDifficulty.Count; }
	}
}


public enum CurseBehavior
{
	FAIL,
	MINUS,
	NORMAL,
	STAR,
	SUCCEED
}

public class MonteCarloSimulator : MonoBehaviour
{

	public List<CardSettings> cardPiles = new List<CardSettings>();

	public List<MonteCard> masterDeck = new List<MonteCard>();

	public List<List<MonteCard>> simulations = new List<List<MonteCard>>();

	public List<StarCount> starCounts;
	public List<float> averageStarsByDepth;


	public int sevenStars;
	public int extraStars;
	public CurseBehavior curseBehavior = CurseBehavior.NORMAL;
	public bool amuletAutoSucceed;
	public bool rootAutoFail;

	System.Random chaos;
	public string debug = "";

	public float[,] percentages;

	public DrawOddsGrid drawOddsGrid;

	public int simulatedShuffles = 100000;

	void Start()
    {
		chaos = new System.Random();
		
		/*for (int i = 0; i < starCounts[0].StarsByDepth.Count; i++)
		{
			debug += starCounts[0].StarsByDepth[i].ToString();
			debug += ", ";
		}*/

	}

	[Button]
	void Simulate()
	{
		simulations.Clear();

		RunSimulations(simulatedShuffles);
		Debug.Log(simulations.Count);
		starCounts = CountStars(sevenStars, extraStars, curseBehavior);

		percentages = CalculateAverages(starCounts);
		for (int i = 0; i < percentages.GetLength(0); i++)
		{
			string line = "";
			for (int j = 0; j < percentages.GetLength(1); j++)
			{
				line += percentages[i, j].ToString("0") + " ";
			}
			debug += line + "\n";
		}

		//Debug.Log(percentages[0, 0]);
		//Debug.Log(percentages[0, 1]);
		//Debug.Log(percentages[0, 2]);
		drawOddsGrid.DisplayData(percentages);
	}

    void Update()
    {
        
    }

	void GenerateMasterDeck()
	{
		masterDeck.Clear();
		foreach(CardSettings pile in cardPiles)
		{
			masterDeck.AddRange(pile.GetMonteCards());
		}
		ShuffleMasterDeck();
	}

	void ShuffleMasterDeck()
	{
		List<MonteCard> masterCopy = new List<MonteCard>();
		masterCopy.AddRange(masterDeck);

		List<MonteCard> newMaster = new List<MonteCard>();
		while (masterCopy.Count > 0)
		{
			int randomIndex = chaos.Next(masterCopy.Count);
			newMaster.Add(masterCopy[randomIndex]);
			masterCopy.RemoveAt(randomIndex);
		}

		masterDeck = newMaster;
	}

	void RunSimulations(int count)
	{
		if (chaos == null)
			chaos = new System.Random();
		GenerateMasterDeck();
		for (int i = 0; i < count; i++)
		{
			List<MonteCard> simulation = new List<MonteCard>();
			simulation.AddRange(masterDeck);
			simulations.Add(simulation);
			ShuffleMasterDeck();
		}
	}

	public StarCount CountStars(List<MonteCard> simulation, int sevenStars, int extraStars, CurseBehavior curseBehavior)
	{
		List<int> StarsByDepth = new List<int>();
		List<int> RequiredByDifficulty = new List<int>();
		List<bool> SafetyByDepth = new List<bool>();

		int totalStars = extraStars;

		int left = 0;
		int right = 0;
		bool hasAHalf = false;
		int successIndex = -1;
		bool failed = false;

		for (int i = 0; i < simulation.Count; i++)
		{
			MonteCard card = simulation[i];
			totalStars += card.stars;
			totalStars += card.sevens * sevenStars;



			if(card.curse)
			{
				switch(curseBehavior)
				{
					case CurseBehavior.FAIL:
						failed = true;
						break;
					case CurseBehavior.MINUS:
						totalStars--;
						break;
					case CurseBehavior.STAR:
						totalStars++;
						break;
					case CurseBehavior.SUCCEED:
						if (successIndex == -1)
							successIndex = i;
						break;
				}
			}

			if(card.root && rootAutoFail)
			{
				failed = true;
			}

			if (card.amulet)
			{
				totalStars++;
				if (amuletAutoSucceed && successIndex == -1)
				{
					successIndex = i;
				}
			}



			if (card.left)
				left++;

			if (card.right)
				right++;

			if (!(card.right && card.left))
				hasAHalf = true;


			int halfTotal = Math.Min(left, right);
			if (!hasAHalf)
				halfTotal--;
			if (halfTotal < 0)
				halfTotal = 0;

			int totalAtDepthI = halfTotal + totalStars;

			StarsByDepth.Add(totalAtDepthI);
			SafetyByDepth.Add(!failed || successIndex > -1);
			if(successIndex > -1)
			{
				while(RequiredByDifficulty.Count < totalAtDepthI)
				{
					RequiredByDifficulty.Add(successIndex);
				}
			}
			else if(!failed)
			{
				while (RequiredByDifficulty.Count < totalAtDepthI)
				{
					RequiredByDifficulty.Add(i);
				}
			}

			
		}

		StarCount count = new StarCount(StarsByDepth, RequiredByDifficulty, successIndex, SafetyByDepth);
		return count;
	}

	public List<StarCount> CountStars(int sevenStars, int extraStars, CurseBehavior curseBehavior)
	{
		List<StarCount> retVal = new List<StarCount>() ;
		for (int i = 0; i < simulations.Count; i++)
		{
			retVal.Add(CountStars(simulations[i], sevenStars, extraStars, curseBehavior));
		}
		return retVal;
	}

	int GetMaxSuccesses(List<StarCount> starCounts)
	{
		int retVal = 0;
		foreach (StarCount count in starCounts)
			if (count.MaxSuccesses > retVal)
				retVal = count.MaxSuccesses;
		return retVal;
	}

	float[,] CalculateAverages(List<StarCount> starCounts)
	{
		List<float> totalStarsByDepth = new List<float>();

		int maxSuccesses = GetMaxSuccesses(starCounts);

		float[,] passPercentages = new float[starCounts[0].DeckSize, maxSuccesses]; //[x, y] is the chance of success at draws, difficulty

		float threefive = 0;

		foreach(StarCount count in starCounts)
		{
			while (totalStarsByDepth.Count < count.StarsByDepth.Count)
				totalStarsByDepth.Add(0);

			for (int i = 0; i < count.StarsByDepth.Count; i++)
			{
				totalStarsByDepth[i] += count.StarsByDepth[i];
			}

			for (int i = 0; i < count.DeckSize; i++)
			{
				for (int j = 0; j < count.MaxSuccesses; j++)
				{
					if (count.PassesCheck(i, j))
						passPercentages[i, j]++;
				}
			}

			threefive += count.PassesCheck(0, 0) ? 1 : 0;
		}

		for (int i = 0; i < totalStarsByDepth.Count; i++)
		{
			totalStarsByDepth[i] /= starCounts.Count;
		}

		if(true)
		{

			for (int i = 0; i < passPercentages.GetLength(0); i++)
			{

				for (int j = 0; j < passPercentages.GetLength(1); j++)
				{
					passPercentages[i, j] /= starCounts.Count;
					passPercentages[i, j] *= 100;
				}
			}

		}
		threefive /= starCounts.Count;
		threefive *= 100;

		Debug.Log(threefive);

		return passPercentages;
	}
}
