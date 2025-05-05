using UnityEngine;

[System.Serializable]
public class Spirit
{
    public string Name;
    public SpiritElement Element;
    public int MinPoints;
    public int MaxPoints;
    public string Description;
	public ColorQuestion ColorQuestion;
}

[System.Serializable]
public class SpiritList
{
    public Spirit[] spirits;
}

public enum SpiritElement
{
    Water,
	Fire,
    Air,
    Earth
}
