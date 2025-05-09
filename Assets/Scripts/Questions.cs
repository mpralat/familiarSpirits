using UnityEngine;

[System.Serializable]
public class Question
{
    public int order;
    public string text;
    public Answer[] answers;
}

[System.Serializable]
public class Answer
{
    public string text;
    public int firePoints;
    public int waterPoints;
    public int earthPoints;
	public int airPoints;
}

[System.Serializable]
public class QuestionList
{
    public Question[] questions;
}

[System.Serializable]
public class ColorQuestion
{
	public string text;
	public ColorAnswer[] answers;
}

[System.Serializable]
public class ColorAnswer
{
	public string text;
	public string color;
}

