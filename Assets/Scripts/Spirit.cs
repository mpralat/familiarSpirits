using UnityEngine;

[System.Serializable]
public class Spirit
{
    public string Name;
    public string Element;
    public int MinPoints;
    public int MaxPoints;
    public string Description;
}

[System.Serializable]
public class SpiritList
{
    public Spirit[] spirits;
}
