using UnityEngine;
using UnityEngine.SceneManagement;

public class Print : ProductionStation
{
    public override string StationName => "프린트기";

    [SerializeField] private string minigameSceneName = "Minigame_Print";

    protected override void Produce(GameObject by)
    {
        SceneManager.LoadSceneAsync(minigameSceneName, LoadSceneMode.Single);
    }
}