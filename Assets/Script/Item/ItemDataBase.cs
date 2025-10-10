using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDataBase : MonoBehaviour
{
    public static ItemDataBase instance;
    private void Awake()
    {
        // 이미 인스턴스가 있다면 자신은 파괴 (중복 방지)
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시에도 유지
        }
        else
        {
            Destroy(gameObject); // 중복된 ItemDataBase가 생기지 않도록
        }
    }

    public List<Item>itemDB = new List<Item>();

    public Item GetById(string id)
    {
        return itemDB.Find(x => x.id == id);
    }
}
