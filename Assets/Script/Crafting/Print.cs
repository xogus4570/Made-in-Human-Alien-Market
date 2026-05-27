using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Print : ProductionStation
{
    public override string StationName => "프린트기";

    [SerializeField] private string minigameSceneName = "Minigame_Print";

    [Header("강화 상태")]
    [SerializeField] private int upgradeLevel = 0;

    private readonly HashSet<string> purchasedUpgrades = new HashSet<string>();

    public bool ApplyUpgrade(string upgradeId)
    {
        if (string.IsNullOrEmpty(upgradeId))
            return false;

        if (purchasedUpgrades.Contains(upgradeId))
            return false;

        purchasedUpgrades.Add(upgradeId);
        upgradeLevel++;

        ProductionUpgradeData.PrintStartBonus = upgradeLevel;

        Debug.Log($"[Print] 강화 완료. 시작 진행도 보너스: {ProductionUpgradeData.PrintStartBonus}");

        return true;
    }

    protected override void Produce(GameObject by)
    {
        SceneManager.LoadSceneAsync(minigameSceneName, LoadSceneMode.Single);
    }
}