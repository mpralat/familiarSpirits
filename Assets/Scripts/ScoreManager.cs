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
        
		// TODO - real logic for choosing the right spirit should come here
		// for now, just choose a random spirit of the correct element
		int maxScore = Mathf.Max(fireScore, waterScore, earthScore, airScore);
		
		Spirit[] selectedArray = null;

    	if (maxScore == fireScore)
    	{
       	 	selectedArray = fireSpirits;
    	}
    	else if (maxScore == waterScore)
    	{
       		selectedArray = waterSpirits;
    	}
    	else if (maxScore == earthScore)
    	{
        	selectedArray = earthSpirits;
    	}
    	else if (maxScore == airScore)
    	{
        	selectedArray = airSpirits;
    	}
		
		int randomIndex = UnityEngine.Random.Range(0, selectedArray.Length);
    	return selectedArray[randomIndex];
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