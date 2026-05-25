using System.Collections.Generic;
using UnityEngine;

public class ProductionRecipeLimit : MonoBehaviour
{
    [Header("이 제작대에서 만들 수 있는 결과물 ID 목록")]
    [SerializeField] private List<string> craftableResultIds = new List<string>();

    public bool CanCraft(string resultId)
    {
        if (string.IsNullOrEmpty(resultId))
            return false;

        return craftableResultIds.Contains(resultId);
    }
}