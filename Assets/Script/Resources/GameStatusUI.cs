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

    [Header("설정")]
    public const int MaxDays = 30;

    private int day = 1;
    private int gold = 9999;
    private int influence = 0;

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

    public bool IsMaxDay => day >= MaxDays;
    public int RemainingDays => Mathf.Max(0, MaxDays - day);

    
    private void UpdateUI()
    {
        dayText.text = $"{day}일차";
        goldText.text = $"골드: {gold:N0}";
        influenceText.text = $"누적영향력: {influence:N0}";
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
