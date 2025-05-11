using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class ScoreManager
{
    private int fireScore = 0;
    private int waterScore = 0;
    private int earthScore = 0;
    private int airScore = 0;

    private Spirit[] fireSpirits;
    private Spirit[] airSpirits;
    private Spirit[] earthSpirits;
    private Spirit[] waterSpirits;

	private int runNumber = 0;

	#nullable enable
	private Spirit? _currentSpirit;
	#nullable disable

	public Spirit CurrentSpirit { 
		get 
		{
			if (_currentSpirit == null) {
				throw new System.InvalidOperationException("CurrentSpirit is not set.");
			}
			return _currentSpirit;
		}
		set
		{
        	_currentSpirit = value;
    	}
 	}
	public string CurrentColor { get; private set; }
	
	public string FrameName {get; private set;}
	
	public void SetColor(string color) 
	{
		CurrentColor = color;
	}

	public string GetColor()
	{
		return CurrentColor;
	}

	public void SetFrame(string frameName)
	{
		FrameName = frameName;
	}

	public string GetFrame()
	{
		return FrameName;
	}

    public void Start()
    {
        LoadSpirits();
		ResetPoints();
    }

    public void AddPoints(int firePoints, int waterPoints, int earthPoints, int airPoints)
    {
        fireScore += firePoints;
        waterScore += waterPoints;
        earthScore += earthPoints;
        airScore += airPoints;
    }

    public void ResetPoints()
    {
        fireScore = 0;
        waterScore = 0;
        earthScore = 0;
        airScore = 0;
		CurrentColor = "";
		FrameName = "";
		CurrentSpirit = null;
    }

    public void CalculateSpirit()
    {
		Debug.Log($"Run number: {runNumber}");
        Debug.Log($"RESULT: Fire: {fireScore}, Water: {waterScore}, Earth: {earthScore}, Air: {airScore}");

        int maxScore = Mathf.Max(fireScore, waterScore, earthScore, airScore);

        // Collect tied elements
		List<SpiritElement> tiedElements = new List<SpiritElement>();

        if (fireScore == maxScore) tiedElements.Add(SpiritElement.Fire);
        if (waterScore == maxScore) tiedElements.Add(SpiritElement.Water);
        if (earthScore == maxScore) tiedElements.Add(SpiritElement.Earth);
        if (airScore == maxScore) tiedElements.Add(SpiritElement.Air);

        // Randomly pick one from ties
        SpiritElement chosenElement = tiedElements.OrderBy(q => UnityEngine.Random.value).First();
        Debug.Log($"Tie resolved randomly � selected: {chosenElement}");

        Spirit[] selectedArray = null;
        int elementScore = 0;

        if (chosenElement == SpiritElement.Fire)
        {
            selectedArray = fireSpirits;
            elementScore = fireScore;
        }
        else if (chosenElement == SpiritElement.Water)
        {
            selectedArray = waterSpirits;
            elementScore = waterScore;
        }
        else if (chosenElement == SpiritElement.Earth)
        {
            selectedArray = earthSpirits;
            elementScore = earthScore;
        }
        else if (chosenElement == SpiritElement.Air)
        {
            selectedArray = airSpirits;
            elementScore = airScore;
        }

        // Find spirits matching the score threshold
        var matchingSpirits = selectedArray
            .Where(spirit => elementScore >= spirit.MinPoints && elementScore <= spirit.MaxPoints)
            .ToArray();

        if (matchingSpirits.Length == 0)
        {
            Debug.LogWarning($"No spirit found for {chosenElement} with score {elementScore}");
            return ;
        }

        int randomIndex = UnityEngine.Random.Range(0, matchingSpirits.Length);
        

		// UNNCOMMENT IF YOU WANT TO GO THROUGH ALL SPIRITS
		// First go through all spirits -- comment out if you don't want to run in test mode

		Debug.Log("Running test run...");
		if (runNumber < 4) {
			CurrentSpirit = waterSpirits[runNumber];
		} else if (runNumber < 9) {
			CurrentSpirit = fireSpirits[runNumber - 4];
		} else if (runNumber < 13) {
			CurrentSpirit = airSpirits[runNumber - 9];
		} else if (runNumber < 18) {
			CurrentSpirit = earthSpirits[runNumber - 13];
		} else {
			CurrentSpirit = matchingSpirits[randomIndex];	
		}
		
		CurrentSpirit = matchingSpirits[randomIndex];
		Debug.Log($"Spirit: {CurrentSpirit.Name}");
		runNumber += 1;
		return;
    }

	public string GetFileName(string spiritName)
	{
		Debug.Log($"Spirit name: {spiritName}, color: {CurrentColor}, frame: {FrameName}");
		// return $"Spirits/{spiritName}/{CurrentColor}";
		
		// return $"{spiritName}_{CurrentColor}_{FrameName}";
		return "Assets/Resources/Ankluz_brown_feathers.webm";
	}	
	
    public void LoadSpirits()
    {
		TextAsset jsonText = Resources.Load<TextAsset>("spirits");
		SpiritList wrapper = JsonUtility.FromJson<SpiritList>(jsonText.text);
		Spirit[] allSpirits = wrapper.spirits;
		this.fireSpirits = allSpirits.Where(spirit => spirit.Element == SpiritElement.Fire).ToArray();
		this.airSpirits = allSpirits.Where(spirit => spirit.Element == SpiritElement.Air).ToArray();
		this.waterSpirits = allSpirits.Where(spirit => spirit.Element == SpiritElement.Water).ToArray();
		this.earthSpirits = allSpirits.Where(spirit => spirit.Element == SpiritElement.Earth).ToArray();
    }
}