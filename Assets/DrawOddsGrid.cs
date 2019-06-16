using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawOddsGrid : MonoBehaviour
{
	public ResultBox resultBoxPrefab;
	public ValueDisplay starPrefab;
	public ValueDisplay cardPrefab;

	public int width = 70;
	public int height = 60;

	public Vector2 offset;
	public Vector2 labelsOffset;
	ResultBox[,] resultBoxes;
	List<ValueDisplay> starLabels = new List<ValueDisplay>();
	List<ValueDisplay> cardLabels = new List<ValueDisplay>();
	public bool regenerate;

	RectTransform rectTransform;
    void Start()
    {
		rectTransform = GetComponent<RectTransform>();
		GenerateGrid();
    }

    void Update()
    {
        if(regenerate)
		{
			regenerate = false;
			DestroyGrid();
			GenerateGrid();
		}
    }
	void DestroyGrid()
	{
		foreach (ResultBox box in resultBoxes)
			Destroy(box.gameObject);
	}

	void GenerateGrid()
	{
		resultBoxes = new ResultBox[width, height];
		cardLabels.Clear();
		starLabels.Clear();

		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				ResultBox clone = Instantiate(resultBoxPrefab);

				RectTransform rect = clone.GetComponent<RectTransform>();
				rect.SetParent(rectTransform);
				rect.anchoredPosition = new Vector2(i * offset.x, j * offset.y);
				resultBoxes[i, j] = clone;

				clone.SetValue(10 * j + i - 30, "0");
			}
		}


		for (int i = 0; i < width; i++)
		{
			ValueDisplay clone = Instantiate(cardPrefab);
			cardLabels.Add(cardPrefab);

			RectTransform rect = clone.GetComponent<RectTransform>();
			rect.SetParent(rectTransform);
			rect.anchoredPosition = new Vector2(i * offset.x, labelsOffset.y);

			clone.SetValue(i + 1);
		}


		for (int i = 0; i < height; i++)
		{
			ValueDisplay clone = Instantiate(starPrefab);
			starLabels.Add(starPrefab);
			
			RectTransform rect = clone.GetComponent<RectTransform>();
			rect.SetParent(rectTransform);
			rect.anchoredPosition = new Vector2(labelsOffset.x, i * offset.y);

			clone.SetValue(i + 1);
		}

		
	}

	public void DisplayData(float[,] data, int cardStart = 0, int starStart = 0, bool addOneToAxes = true)
	{
		for (int i = 0; i < width; i++)
		{
			cardLabels[i].SetValue(cardStart + i + (addOneToAxes ? 1 : 0));
		}
		for (int i = 0; i < width; i++)
		{
			cardLabels[i].SetValue(starStart + i + (addOneToAxes ? 1 : 0));
		}

		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				try { 
					resultBoxes[i, j].SetValue(data[i + cardStart, j + starStart]);
				}
				catch
				{
					resultBoxes[i, j].SetValue(-1);
				}
			}
		}
	}
}
