using System.Collections.Generic;
using UnityEngine;

public class AugmentManager : MonoBehaviour
{
    public static AugmentManager Instance;

    [Header("연결")]
    [SerializeField] private GameStatusUI gameStatusUI;
    [SerializeField] private GameObject augmentRoot;
    [SerializeField] private AugmentCardUI[] cardUIs;

    [Header("증강 데이터")]
    [SerializeField] private List<AugmentData> augmentList = new List<AugmentData>();

    [Header("랭크 확률")]
    [SerializeField] private int cRankChance = 70;
    [SerializeField] private int bRankChance = 25;
    [SerializeField] private int aRankChance = 5;

    [Header("테스트")]
    [SerializeField] private bool enableTestKey = true;
    [SerializeField] private KeyCode testKey = KeyCode.U;

    private bool isOpen = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        CloseAugment();
    }

    private void Update()
    {
        if (!enableTestKey)
            return;

        if (Input.GetKeyDown(testKey))
        {
            OpenRandomAugments();
        }
    }

    public void OpenRandomAugments()
    {
        if (augmentRoot == null)
        {
            Debug.LogWarning("[AugmentManager] augmentRoot가 연결되지 않았습니다.");
            return;
        }

        if (cardUIs == null || cardUIs.Length == 0)
        {
            Debug.LogWarning("[AugmentManager] cardUIs가 비어 있습니다.");
            return;
        }

        if (augmentList == null || augmentList.Count == 0)
        {
            Debug.LogWarning("[AugmentManager] augmentList가 비어 있습니다.");
            return;
        }

        AugmentRank selectedRank = GetRandomRank();
        List<AugmentData> candidates = GetAugmentsByRank(selectedRank);

        // 해당 랭크 증강이 카드 수보다 부족하면 전체 목록에서 뽑기
        if (candidates.Count < cardUIs.Length)
            candidates = new List<AugmentData>(augmentList);

        List<AugmentData> selectedAugments = PickRandomAugments(candidates, cardUIs.Length);

        for (int i = 0; i < cardUIs.Length; i++)
        {
            if (cardUIs[i] == null)
                continue;

            if (i < selectedAugments.Count)
                cardUIs[i].Setup(selectedAugments[i], this);
            else
                cardUIs[i].Clear();
        }

        augmentRoot.SetActive(true);
        isOpen = true;

        Debug.Log($"[AugmentManager] 증강 선택창 열림 / 선택 랭크: {selectedRank}");
    }

    public void SelectAugment(AugmentData data)
    {
        if (data == null)
        {
            Debug.LogWarning("[AugmentManager] 선택한 증강 데이터가 없습니다.");
            return;
        }

        ApplyAugment(data);
        CloseAugment();

        Debug.Log($"[AugmentManager] 증강 선택 완료: {data.augmentName}");
    }

    private void ApplyAugment(AugmentData data)
    {
        if (gameStatusUI == null)
        {
            Debug.LogWarning("[AugmentManager] GameStatusUI가 연결되지 않았습니다.");
            return;
        }

        switch (data.effectType)
        {
            case AugmentEffectType.AddGold:
                gameStatusUI.AddGold(data.value);
                break;

            case AugmentEffectType.AddExp:
                gameStatusUI.AddExp(data.value);
                break;

            case AugmentEffectType.AddInfluence:
                gameStatusUI.AddInfluence(data.value);
                break;

            case AugmentEffectType.AddSatisfaction:
                gameStatusUI.AddSatisfaction(data.value);
                break;
        }
    }

    public void CloseAugment()
    {
        if (cardUIs != null)
        {
            for (int i = 0; i < cardUIs.Length; i++)
            {
                if (cardUIs[i] != null)
                    cardUIs[i].Clear();
            }
        }

        if (augmentRoot != null)
            augmentRoot.SetActive(false);

        isOpen = false;
    }

    private AugmentRank GetRandomRank()
    {
        int total = Mathf.Max(1, cRankChance + bRankChance + aRankChance);
        int randomValue = Random.Range(0, total);

        if (randomValue < cRankChance)
            return AugmentRank.C;

        if (randomValue < cRankChance + bRankChance)
            return AugmentRank.B;

        return AugmentRank.A;
    }

    private List<AugmentData> GetAugmentsByRank(AugmentRank rank)
    {
        List<AugmentData> result = new List<AugmentData>();

        for (int i = 0; i < augmentList.Count; i++)
        {
            if (augmentList[i] == null)
                continue;

            if (augmentList[i].rank == rank)
                result.Add(augmentList[i]);
        }

        return result;
    }

    private List<AugmentData> PickRandomAugments(List<AugmentData> source, int count)
    {
        List<AugmentData> result = new List<AugmentData>();
        List<AugmentData> temp = new List<AugmentData>(source);

        int pickCount = Mathf.Min(count, temp.Count);

        for (int i = 0; i < pickCount; i++)
        {
            int randomIndex = Random.Range(0, temp.Count);
            result.Add(temp[randomIndex]);
            temp.RemoveAt(randomIndex);
        }

        return result;
    }

    public bool IsOpen()
    {
        return isOpen;
    }
}