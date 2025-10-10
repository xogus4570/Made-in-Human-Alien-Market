using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeDB : MonoBehaviour
{
    public List<Recipe> recipes = new List<Recipe>();

    public Recipe Find(string a, string b, string c)
    {
        foreach (var r in recipes)
            if (r.Matches(a, b, c)) return r;
        return null;
    }
}
