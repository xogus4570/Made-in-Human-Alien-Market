using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CraftingTable : ProductionStation
{
    public override string StationName => "제작대";
    [SerializeField] string minigameSceneName = "Minigame_Crafting table";

    protected override void Produce(GameObject by)
    {
        SceneManager.LoadSceneAsync(minigameSceneName, LoadSceneMode.Single);
    }
}
