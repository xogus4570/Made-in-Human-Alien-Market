using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDataBase : MonoBehaviour
{

    public static ItemDataBase instance;
    private void Awake()
    {
        instance = this;
    }

    public List<Item>itemDB = new List<Item>();

    public Item GetById(string id)
    {
        return itemDB.Find(x => x.id == id);
    }

    public Item FindById(string id)
{
    return itemDB.Find(x => x.id == id);  // 복사 x, 원본 그대로 반환
}
}
