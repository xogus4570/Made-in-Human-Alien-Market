using UnityEngine;
using UnityEngine.SceneManagement;

public class PackingTable : ProductionStation
{
    public override string StationName => "포장대";

    [SerializeField] private string minigameSceneName = "Minigame_Packing";

    protected override void Produce(GameObject by)
    {
        SceneManager.LoadSceneAsync(minigameSceneName, LoadSceneMode.Single);
    }
}