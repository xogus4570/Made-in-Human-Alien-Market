using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Recipe 
{
    public string ingredientA;
    public string ingredientB;
    public string ingredientC;
    public string resultId;
    public int resultCount = 1;

    public bool Matches(string a, string b, string c)
    {
        var set = new HashSet<string>(new[] { a, b, c });
        return set.SetEquals(new[] { ingredientA, ingredientB, ingredientC });
    }
}