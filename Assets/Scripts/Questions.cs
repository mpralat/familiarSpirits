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
	public FrameUrls frameUrls;
}

[System.Serializable]
public class FrameUrls
{
	public string branchesUrl;
	public string feathersUrl;
	public string flowersUrl;
	public string mushroomUrl;
}

[System.Serializable]
public class FrameAnswer
{
	public string text;
	public string frameName;

	public FrameAnswer(string text, string frameName)
	{
		this.text = text;
		this.frameName = frameName;
	}
}

[System.Serializable]
public class FrameQuestion
{
	public string text;
	public FrameAnswer[] answers;

	public FrameQuestion(string text, FrameAnswer[] answers)
	{
		this.text = text;
		this.answers = answers;
	}
}