using UnityEngine;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class UrlItem
{
    public string name;
    public string url;
}

[System.Serializable]
public class UrlItemListWrapper
{
    public List<UrlItem> items;
}

public class UrlManager
{
    private Dictionary<string, string> urlsDict = new Dictionary<string, string>();
    
    public void LoadLinks()
    {
        TextAsset jsonText = Resources.Load<TextAsset>("urls");
        UrlItemListWrapper wrapper = JsonUtility.FromJson<UrlItemListWrapper>(jsonText.text);
        
        foreach (var item in wrapper.items)
        {
            urlsDict[item.name] = item.url;
        }
    }

    public string GetUrlByName(string name)
    {
        if (urlsDict.TryGetValue(name, out string url))
            return url;

        Debug.LogWarning("Name not found: " + name);
        return null;
    }
}