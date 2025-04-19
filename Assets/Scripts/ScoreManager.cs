using UnityEngine;
using System.Linq;

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

    private void ResetPoints()
    {
        fireScore = 0;
        waterScore = 0;
        earthScore = 0;
        airScore = 0;
    }

    public Spirit GetSpirit()
    {
        Debug.Log($"RESULT: Fire: {fireScore}, Water: {waterScore}, Earth: {earthScore}, Air: {airScore}");

        int maxScore = Mathf.Max(fireScore, waterScore, earthScore, airScore);

        // Collect tied elements
        string[] tiedElements = new string[4];
        int tieCount = 0;

        if (fireScore == maxScore) tiedElements[tieCount++] = "Fire";
        if (waterScore == maxScore) tiedElements[tieCount++] = "Water";
        if (earthScore == maxScore) tiedElements[tieCount++] = "Earth";
        if (airScore == maxScore) tiedElements[tieCount++] = "Air";

        // Randomly pick one from ties
        string chosenElement = tiedElements[UnityEngine.Random.Range(0, tieCount)];
        Debug.Log($"Tie resolved randomly — selected: {chosenElement}");

        Spirit[] selectedArray = null;
        int elementScore = 0;

        if (chosenElement == "Fire")
        {
            selectedArray = fireSpirits;
            elementScore = fireScore;
        }
        else if (chosenElement == "Water")
        {
            selectedArray = waterSpirits;
            elementScore = waterScore;
        }
        else if (chosenElement == "Earth")
        {
            selectedArray = earthSpirits;
            elementScore = earthScore;
        }
        else if (chosenElement == "Air")
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
            return null;
        }

        int randomIndex = UnityEngine.Random.Range(0, matchingSpirits.Length);
        return matchingSpirits[randomIndex];
    }
    private void LoadSpirits()
    {
		TextAsset jsonText = Resources.Load<TextAsset>("spirits");
		SpiritList wrapper = JsonUtility.FromJson<SpiritList>(jsonText.text);
		Spirit[] allSpirits = wrapper.spirits;
		this.fireSpirits = allSpirits.Where(spirit => spirit.Element == "Fire").ToArray();
		this.airSpirits = allSpirits.Where(spirit => spirit.Element == "Air").ToArray();
		this.waterSpirits = allSpirits.Where(spirit => spirit.Element == "Water").ToArray();
		this.earthSpirits = allSpirits.Where(spirit => spirit.Element == "Earth").ToArray();
    }
}