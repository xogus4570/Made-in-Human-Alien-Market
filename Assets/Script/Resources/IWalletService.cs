using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWalletService
{
    bool TryPay(int amount);
    void Add(int amount);
}

public class WalletFromGameStatus : IWalletService
{
    private readonly GameStatusUI _ui;
    public WalletFromGameStatus(GameStatusUI ui) { _ui = ui; }
    public bool TryPay(int amount) => _ui.TrySpendGold(amount);
    public void Add(int amount) => _ui.EarnGold(amount);
}