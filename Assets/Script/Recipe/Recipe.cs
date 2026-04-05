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

    [Header("³³Ç° º¸»ó")]
    public int rewardGold = 0;
    public int rewardExp = 0;
    public int rewardInfluence = 0;
    public int rewardSatisfaction = 0;



    public bool Matches(string a, string b, string c)
    {
        List<string> input = new List<string> { a, b, c };
        List<string> recipe = new List<string> { ingredientA, ingredientB, ingredientC };

        input.Sort();
        recipe.Sort();

        for (int i = 0; i < input.Count; i++)
        {
            if (input[i] != recipe[i])
                return false;
        }

        return true;
    }
}
