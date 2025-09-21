using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProductionStation : MonoBehaviour, IInteractable
{
    public abstract string StationName { get; }  // "프린트기" 등
    protected abstract void Produce(GameObject by); // 실제 제작 로직(혹은 미니게임 진입)

    public void OnInteract(GameObject by)
    {
        Debug.Log($"{StationName} 상호작용 시작");
        Produce(by);
    }

    public string GetInteractionName() => $"{StationName} 사용하기";

}
