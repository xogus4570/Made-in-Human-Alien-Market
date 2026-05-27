using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CraftingMinigameController : MonoBehaviour
{
    [Header("미니게임 패널")]
    [SerializeField] private GameObject arrowMinigamePanel;

    [Header("화살표 이미지 4개")]
    [SerializeField] private Image[] arrowImages;

    [Header("화살표 스프라이트")]
    [SerializeField] private Sprite upSprite;
    [SerializeField] private Sprite downSprite;
    [SerializeField] private Sprite leftSprite;
    [SerializeField] private Sprite rightSprite;

    [Header("진행도")]
    [SerializeField] private Slider progressSlider;
    [SerializeField] private int maxGauge = 6;
    [SerializeField] private int arrowsPerGauge = 4;

    [Header("실패 패널티")]
    [SerializeField] private int failSatisfactionPenalty = 3;

    [Header("완료 후 돌아갈 메인 씬")]
    [SerializeField] private string mainSceneName = "GwonTaeHyeon_Test";

    private readonly List<ArrowDirection> currentPattern = new List<ArrowDirection>();

    private IngredientSlotUI slotA;
    private IngredientSlotUI slotB;
    private IngredientSlotUI slotC;
    private Recipe currentRecipe;

    private int currentInputIndex;
    private int currentGauge;
    private bool isPlaying;

    private void Start()
    {
        if (arrowMinigamePanel != null)
            arrowMinigamePanel.SetActive(false);

        if (progressSlider != null)
        {
            progressSlider.minValue = 0;
            progressSlider.maxValue = maxGauge;
            progressSlider.value = 0;
        }

        isPlaying = false;
    }

    private void Update()
    {
        if (!isPlaying)
            return;

        if (Input.GetKeyDown(KeyCode.UpArrow))
            CheckInput(ArrowDirection.Up);

        if (Input.GetKeyDown(KeyCode.DownArrow))
            CheckInput(ArrowDirection.Down);

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            CheckInput(ArrowDirection.Left);

        if (Input.GetKeyDown(KeyCode.RightArrow))
            CheckInput(ArrowDirection.Right);

        if (Input.GetKeyDown(KeyCode.Escape))
            CancelMinigame();
    }

    public void StartMinigame(IngredientSlotUI a, IngredientSlotUI b, IngredientSlotUI c, Recipe recipe)
    {
        slotA = a;
        slotB = b;
        slotC = c;
        currentRecipe = recipe;

        currentGauge = Mathf.Clamp(
            ProductionUpgradeData.CraftingStartBonus,
            0,
            maxGauge - 1
        );
        currentInputIndex = 0;
        isPlaying = true;

        if (progressSlider != null)
        {
            progressSlider.minValue = 0;
            progressSlider.maxValue = maxGauge;
            progressSlider.value = currentGauge;
        }

        if (arrowMinigamePanel != null)
            arrowMinigamePanel.SetActive(true);

        GenerateNewPattern();

        Debug.Log("[CraftingMinigame] 미니게임 시작");
    }

    private void GenerateNewPattern()
    {
        currentPattern.Clear();
        currentInputIndex = 0;

        for (int i = 0; i < arrowsPerGauge; i++)
        {
            ArrowDirection randomDirection = (ArrowDirection)Random.Range(0, 4);
            currentPattern.Add(randomDirection);
        }

        RefreshArrowImages();

        Debug.Log("[CraftingMinigame] 새 패턴: " + string.Join(", ", currentPattern));
    }

    private void RefreshArrowImages()
    {
        if (arrowImages == null || arrowImages.Length < arrowsPerGauge)
        {
            Debug.LogWarning("[CraftingMinigame] Arrow Images 배열이 부족합니다. Size를 4로 설정하세요.");
            return;
        }

        for (int i = 0; i < arrowImages.Length; i++)
        {
            if (arrowImages[i] == null)
            {
                Debug.LogWarning($"[CraftingMinigame] arrowImages[{i}]가 연결되지 않았습니다.");
                continue;
            }

            if (i >= currentPattern.Count)
            {
                arrowImages[i].gameObject.SetActive(false);
                continue;
            }

            arrowImages[i].gameObject.SetActive(true);
            arrowImages[i].enabled = true;
            arrowImages[i].sprite = GetSprite(currentPattern[i]);
            arrowImages[i].color = Color.white;
        }
    }

    private Sprite GetSprite(ArrowDirection direction)
    {
        switch (direction)
        {
            case ArrowDirection.Up:
                return upSprite;
            case ArrowDirection.Down:
                return downSprite;
            case ArrowDirection.Left:
                return leftSprite;
            case ArrowDirection.Right:
                return rightSprite;
            default:
                return null;
        }
    }

    private void CheckInput(ArrowDirection input)
    {
        if (currentInputIndex >= currentPattern.Count)
            return;

        ArrowDirection correctInput = currentPattern[currentInputIndex];

        Debug.Log($"[CraftingMinigame] 입력: {input}, 정답: {correctInput}");

        if (input == correctInput)
        {
            if (currentInputIndex < arrowImages.Length && arrowImages[currentInputIndex] != null)
                arrowImages[currentInputIndex].color = Color.green;

            currentInputIndex++;

            if (currentInputIndex >= currentPattern.Count)
                SuccessOneGauge();
        }
        else
        {
            FailPattern();
        }
    }

    private void FailPattern()
    {
        Debug.Log("[CraftingMinigame] 입력 실패. 진행도 증가 + 만족도 패널티 저장");

        MinigamePenaltyData.AddPenalty(failSatisfactionPenalty);

        SuccessOneGauge();
    }

    private void SuccessOneGauge()
    {
        currentGauge++;

        if (progressSlider != null)
            progressSlider.value = currentGauge;

        Debug.Log($"[CraftingMinigame] 진행도 증가: {currentGauge}/{maxGauge}");

        if (currentGauge >= maxGauge)
            CompleteCrafting();
        else
            GenerateNewPattern();
    }

    private void CompleteCrafting()
    {
        isPlaying = false;

        if (currentRecipe == null)
        {
            Debug.LogWarning("[CraftingMinigame] currentRecipe가 없습니다.");
            return;
        }

        if (slotA == null || slotB == null || slotC == null)
        {
            Debug.LogWarning("[CraftingMinigame] 재료 슬롯이 없습니다.");
            return;
        }

        if (Inventory.instance == null)
        {
            Debug.LogWarning("[CraftingMinigame] Inventory.instance가 없습니다.");
            return;
        }

        if (ItemDataBase.instance == null)
        {
            Debug.LogWarning("[CraftingMinigame] ItemDataBase.instance가 없습니다.");
            return;
        }

        Inventory.instance.Remove(slotA.item, slotA.count);
        Inventory.instance.Remove(slotB.item, slotB.count);
        Inventory.instance.Remove(slotC.item, slotC.count);

        Item resultItem = ItemDataBase.instance.GetById(currentRecipe.resultId);

        if (resultItem != null)
        {
            Inventory.instance.Add(resultItem, Mathf.Max(1, currentRecipe.resultCount));
            Debug.Log($"[CraftingMinigame] 제작 완료: {resultItem.itemName}");
        }
        else
        {
            Debug.LogWarning($"[CraftingMinigame] 결과 아이템을 찾지 못했습니다: {currentRecipe.resultId}");
        }

        if (arrowMinigamePanel != null)
            arrowMinigamePanel.SetActive(false);

        SceneManager.LoadScene(mainSceneName, LoadSceneMode.Single);
    }

    private void CancelMinigame()
    {
        isPlaying = false;

        if (arrowMinigamePanel != null)
            arrowMinigamePanel.SetActive(false);

        Debug.Log("[CraftingMinigame] 제작 미니게임 취소");
    }
}