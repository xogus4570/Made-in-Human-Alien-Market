using UnityEngine;

public static class CraftingSessionData
{
    public static Item ingredientA;
    public static Item ingredientB;
    public static Item ingredientC;
    public static Recipe recipe;

    public static void Set(Item a, Item b, Item c, Recipe targetRecipe)
    {
        ingredientA = a;
        ingredientB = b;
        ingredientC = c;
        recipe = targetRecipe;
    }

    public static void Clear()
    {
        ingredientA = null;
        ingredientB = null;
        ingredientC = null;
        recipe = null;
    }
}
