using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStatusUI : MonoBehaviour
{
    [Header("UI 연결 (Legacy Text)")]
    [SerializeField] private Text dayText;
    [SerializeField] private Text goldText;
    [SerializeField] private Text influenceText;
    [SerializeField] private Text satisfactionText;

    [Header("레벨 / 경험치 UI")]
    [SerializeField] private Text levelText;
    [SerializeField] private Text expText;
    [SerializeField] private Slider expSlider;

    [Header("설정")]
    public const int MaxDays = 30;

    [Header("레벨 / 경험치 데이터")]
    [SerializeField] private int maxExp = 100;
    [SerializeField] private int expIncreasePerLevel = 50;

    private GameDataManager Data => GameDataManager.Instance;

    void Start()
    {
        UpdateUI();
    }

    private void OnEnable()
    {
        UpdateUI();
    }

    public bool NextDay()
    {
        if (Data == null) return false;
        if (Data.day >= MaxDays) return false;

        Data.day++;
        UpdateUI();
        return true;
    }

    public void SetDay(int value)
    {
        if (Data == null) return;

        Data.day = Mathf.Clamp(value, 1, MaxDays);
        UpdateUI();
    }

    public void AddGold(int amount)
    {
        if (Data == null) return;

        Data.gold = Mathf.Max(0, Data.gold + amount);
        UpdateUI();
    }

    public void AddInfluence(int amount)
    {
        if (Data == null) return;

        Data.influence = Mathf.Max(0, Data.influence + amount);
        UpdateUI();
    }

    public void AddSatisfaction(int amount)
    {
        if (Data == null) return;

        Data.satisfaction = Mathf.Clamp(Data.satisfaction + amount, 0, 100);
        UpdateUI();
    }

    public void ReduceSatisfaction(int amount)
    {
        if (Data == null) return;
        if (amount <= 0) return;

        Data.satisfaction = Mathf.Clamp(Data.satisfaction - amount, 0, 100);
        UpdateUI();
    }

    public void AddExp(int amount)
    {
        if (Data == null) return;
        if (amount <= 0) return;

        Data.currentExp += amount;

        while (Data.currentExp >= maxExp)
        {
            Data.currentExp -= maxExp;
            LevelUp();
        }

        UpdateUI();
    }

    private void LevelUp()
    {
        if (Data == null) return;

        Data.level++;
        maxExp += expIncreasePerLevel;

        Debug.Log($"[GameStatusUI] 레벨업! Lv.{Data.level}");
    }

    public int CurrentSatisfaction => Data != null ? Data.satisfaction : 0;
    public int CurrentDay => Data != null ? Data.day : 1;
    public bool IsMaxDay => Data != null && Data.day >= MaxDays;
    public int RemainingDays => Data != null ? Mathf.Max(0, MaxDays - Data.day) : MaxDays;
    public int CurrentLevel => Data != null ? Data.level : 1;
    public int CurrentExp => Data != null ? Data.currentExp : 0;
    public int MaxExpValue => maxExp;

    public int CurrentGold => Data != null ? Data.gold : 0;
    public int CurrentInfluence => Data != null ? Data.influence : 0;

    public void ApplyLoadedStatus(
        int loadedDay,
        int loadedGold,
        int loadedInfluence,
        int loadedSatisfaction,
        int loadedLevel,
        int loadedCurrentExp)
    {
        if (Data == null)
        {
            Debug.LogWarning("[GameStatusUI] 저장 데이터 반영 실패: GameDataManager.Instance가 없습니다.");
            return;
        }

        Data.day = Mathf.Clamp(loadedDay, 1, MaxDays);
        Data.gold = Mathf.Max(0, loadedGold);
        Data.influence = Mathf.Max(0, loadedInfluence);
        Data.satisfaction = Mathf.Clamp(loadedSatisfaction, 0, 100);

        Data.level = Mathf.Max(1, loadedLevel);
        Data.currentExp = Mathf.Max(0, loadedCurrentExp);

        UpdateUI();

        Debug.Log($"[GameStatusUI] 저장 데이터 반영 완료: {Data.day}일차 / 골드 {Data.gold} / 영향력 {Data.influence} / 만족도 {Data.satisfaction} / Lv.{Data.level}");
    }

    public void RefreshStatusUI()
    {
        UpdateUI();
        Debug.Log("[GameStatusUI] 상태 UI 새로고침 완료");
    }

    private void UpdateUI()
    {
        if (Data == null)
        {
            Debug.LogWarning("[GameStatusUI] GameDataManager.Instance가 없습니다.");
            return;
        }

        if (dayText != null)
            dayText.text = $"{Data.day}";

        if (goldText != null)
            goldText.text = $"{Data.gold:N0}";

        if (influenceText != null)
            influenceText.text = $"{Data.influence:N0}";

        if (satisfactionText != null)
            satisfactionText.text = $"{Data.satisfaction}";

        if (levelText != null)
            levelText.text = $"Lv.{Data.level}";

        if (expText != null)
            expText.text = $"{Data.currentExp} / {maxExp}";

        if (expSlider != null)
        {
            expSlider.minValue = 0;
            expSlider.maxValue = maxExp;
            expSlider.value = Data.currentExp;
        }
    }

    public bool TrySpendGold(int amount)
    {
        if (Data == null) return false;
        if (amount <= 0) return true;
        if (Data.gold < amount) return false;

        Data.gold -= amount;
        UpdateUI();
        return true;
    }

    public void EarnGold(int amount)
    {
        AddGold(amount);
    }
}