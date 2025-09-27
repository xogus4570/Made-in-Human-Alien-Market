using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingMinigame : MonoBehaviour
{
    [SerializeField] private Item inputItem;   // 재료 아이템
    [SerializeField] private int inputCount = 3;
    [SerializeField] private Item outputItem;  // 결과 아이템
    [SerializeField] private int pressNeeded = 10; // 스페이스 연타 횟수

    private int pressCount = 0;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            pressCount++;
            Debug.Log($"연타 {pressCount}/{pressNeeded}");

            if (pressCount >= pressNeeded)
            {
                TryCraft();
                pressCount = 0;
            }
        }
    }

    void TryCraft()
    {
        if (Inventory.instance.GetCount(inputItem) < inputCount)
        {
            Debug.Log("재료 부족");
            return;
        }

        Inventory.instance.Remove(inputItem, inputCount);
        Inventory.instance.Add(outputItem, 1);
        Debug.Log("제작 성공!");
    }
}
