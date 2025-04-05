using UnityEngine;

public class ScoreManager
{
    private int fireScore = 0;
    private int waterScore = 0;
    private int earthScore = 0;

    public void AddPoints(int firePoints, int waterPoints, int earthPoints)
    {
        fireScore += firePoints;
        waterScore += waterPoints;
        earthScore += earthPoints;
    }

    public void ResetPoints()
    {
        fireScore = 0;
        waterScore = 0;
        earthScore = 0;
    }

    public string GetFileName()
    {
        Debug.Log($"RESULT: Fire: {fireScore}, Water: {waterScore}, Earth: {earthScore}");
        if (waterScore > fireScore && waterScore > earthScore)
        {
            return "water";
        } else if (fireScore > earthScore)
        {
            return "fire";
        }
        else
        {
            return "earth";
        }
    }
}
