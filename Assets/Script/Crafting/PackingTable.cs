using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PackingTable : ProductionStation
{
    public override string StationName => "포장대";

    [SerializeField] private string minigameSceneName = "Minigame_Packing";

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

        ProductionUpgradeData.PackingStartBonus = upgradeLevel;

        Debug.Log($"[PackingTable] 강화 완료. 시작 진행도 보너스: {ProductionUpgradeData.PackingStartBonus}");

        return true;
    }

    protected override void Produce(GameObject by)
    {
        SceneManager.LoadSceneAsync(minigameSceneName, LoadSceneMode.Single);
    }
}