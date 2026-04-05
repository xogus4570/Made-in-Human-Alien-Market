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

    private int day = 1;
    private int gold = 9999;
    private int influence = 9999;
    private int satisfaction = 100;

    [Header("레벨 / 경험치 데이터")]
    [SerializeField] private int level = 1;
    [SerializeField] private int currentExp = 0;
    [SerializeField] private int maxExp = 100;
    [SerializeField] private int expIncreasePerLevel = 50;

    void Start()
    {
        UpdateUI();
    }

    public bool NextDay()
    {
        if (day >= MaxDays) return false; // 이미 최대
        day++;
        UpdateUI();
        return true; // 증가 성공
    }

    public void SetDay(int value)
    {
        day = Mathf.Clamp(value, 1, MaxDays);
        UpdateUI();
    }

    public void AddGold(int amount)
    {
        gold = Mathf.Max(0, gold + amount);
        UpdateUI();
    }

    public void AddInfluence(int amount)
    {
        influence = Mathf.Max(0, influence + amount);
        UpdateUI();
    }
    public void AddSatisfaction(int amount)
    {
        satisfaction = Mathf.Clamp(satisfaction + amount, 0, 100);
        UpdateUI();
    }

    public void ReduceSatisfaction(int amount)
    {
        if (amount <= 0) return;
        satisfaction = Mathf.Clamp(satisfaction - amount, 0, 100);
        UpdateUI();
    }
    public void AddExp(int amount)
    {
        if (amount <= 0) return;

        currentExp += amount;

        while (currentExp >= maxExp)
        {
            currentExp -= maxExp;
            LevelUp();
        }

        UpdateUI();
    }
    private void LevelUp()
    {
        level++;
        maxExp += expIncreasePerLevel;

        Debug.Log($"[GameStatusUI] 레벨업! Lv.{level}");
    }

    public int CurrentSatisfaction => satisfaction;
    public int CurrentDay => day;
    public bool IsMaxDay => day >= MaxDays;
    public int RemainingDays => Mathf.Max(0, MaxDays - day);
    public int CurrentLevel => level;
    public int CurrentExp => currentExp;
    public int MaxExpValue => maxExp;


    private void UpdateUI()
    {
        dayText.text = $"{day}일차";
        goldText.text = $"골드: {gold:N0}";
        influenceText.text = $"누적영향력: {influence:N0}";

        if (satisfactionText != null)
            satisfactionText.text = $"고객만족도: {satisfaction}";

        if (levelText != null)
            levelText.text = $"Lv. {level}";

        if (expText != null)
            expText.text = $"EXP: {currentExp} / {maxExp}";

        if (expSlider != null)
        {
            expSlider.minValue = 0;
            expSlider.maxValue = maxExp;
            expSlider.value = currentExp;
        }
    }

    public bool TrySpendGold(int amount)
    {
        if (amount <= 0) return true;
        if (gold < amount) return false;
        gold -= amount;
        UpdateUI();
        return true;
    }

    public void EarnGold(int amount)
    {
        AddGold(amount); 
    }
}
