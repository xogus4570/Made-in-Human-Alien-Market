using System.Collections;
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

    public bool HasUpgrade(string upgradeId)
    {
        if (string.IsNullOrEmpty(upgradeId))
            return false;

        return purchasedUpgrades.Contains(upgradeId);
    }

    public bool ApplyUpgrade(string upgradeId)
    {
        if (string.IsNullOrEmpty(upgradeId))
        {
            Debug.LogWarning("[CraftingTable] upgradeId가 비어 있습니다.");
            return false;
        }

        if (purchasedUpgrades.Contains(upgradeId))
        {
            Debug.Log($"[CraftingTable] 이미 구매한 강화입니다: {upgradeId}");
            return false;
        }

        purchasedUpgrades.Add(upgradeId);
        upgradeLevel++;

        Debug.Log($"[CraftingTable] 강화 적용 완료: {upgradeId}, 현재 강화 레벨: {upgradeLevel}");

        // TODO:
        // 나중에 upgradeId 별 실제 강화 효과를 여기서 분기해서 적용하면 됨
        // 예)
        // if (upgradeId == "craft_speed_1") { ... }
        // else if (upgradeId == "craft_quality_1") { ... }

        return true;
    }

    protected override void Produce(GameObject by)
    {
        SceneManager.LoadSceneAsync(minigameSceneName, LoadSceneMode.Single);
    }
}
