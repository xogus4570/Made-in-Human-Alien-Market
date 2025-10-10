using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private GameObject shopUI; // Canvas_Shop 연결
    bool activeShop = false;

    private void Start()
    {
        shopUI.SetActive(activeShop);
    }

    public void ToggleShop()
    {
        shopUI.SetActive(!shopUI.activeSelf);
    }
}
