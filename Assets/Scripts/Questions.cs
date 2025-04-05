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
}

[System.Serializable]
public class QuestionList
{
    public Question[] questions;
}
