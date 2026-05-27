using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CraftingTable : ProductionStation
{
    public override string StationName => "제작대";

    [SerializeField] private string minigameSceneName = "Minigame_Crafting table";

    [Header("강화 상태")]
    [SerializeField] private int upgradeLevel = 0;

    private readonly HashSet<string> purchasedUpgrades = new HashSet<string>();

    public int UpgradeLevel => upgradeLevel;

    public bool ApplyUpgrade(string upgradeId)
    {
        if (string.IsNullOrEmpty(upgradeId))
            return false;

        if (purchasedUpgrades.Contains(upgradeId))
            return false;

        purchasedUpgrades.Add(upgradeId);
        upgradeLevel++;

        ProductionUpgradeData.CraftingStartBonus = upgradeLevel;

        Debug.Log($"[CraftingTable] 강화 완료. 시작 진행도 보너스: {ProductionUpgradeData.CraftingStartBonus}");

        return true;
    }

    protected override void Produce(GameObject by)
    {
        SceneManager.LoadSceneAsync(minigameSceneName, LoadSceneMode.Single);
    }
}