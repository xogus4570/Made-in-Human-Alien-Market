using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PrintMinigameController : MonoBehaviour
{
    [Header("미니게임 패널")]
    [SerializeField] private GameObject printMinigamePanel;

    [Header("타이밍 바 UI")]
    [SerializeField] private RectTransform timingBarArea;
    [SerializeField] private RectTransform successZone;
    [SerializeField] private RectTransform movingBar;
    [SerializeField] private Slider progressSlider;

    [Header("설정")]
    [SerializeField] private int requiredSuccessCount = 6;
    [SerializeField] private float barSpeed = 600f;
    [SerializeField] private float successZoneWidth = 120f;

    [Header("실패 패널티")]
    [SerializeField] private int failSatisfactionPenalty = 3;

    [Header("완료 후 돌아갈 메인 씬")]
    [SerializeField] private string mainSceneName = "GwonTaeHyeon_Test";

    private IngredientSlotUI slotA;
    private IngredientSlotUI slotB;
    private IngredientSlotUI slotC;
    private Recipe currentRecipe;

    private int currentSuccessCount;
    private int moveDirection = 1;
    private bool isPlaying;

    private void Start()
    {
        if (printMinigamePanel != null)
            printMinigamePanel.SetActive(false);

        if (progressSlider != null)
        {
            progressSlider.minValue = 0;
            progressSlider.maxValue = requiredSuccessCount;
            progressSlider.value = 0;
        }

        isPlaying = false;
    }

    private void Update()
    {
        if (!isPlaying) return;

        MoveBar();

        if (Input.GetKeyDown(KeyCode.Space))
            CheckTiming();

        if (Input.GetKeyDown(KeyCode.Escape))
            CancelMinigame();
    }

    public void StartMinigame(IngredientSlotUI a, IngredientSlotUI b, IngredientSlotUI c, Recipe recipe)
    {
        slotA = a;
        slotB = b;
        slotC = c;
        currentRecipe = recipe;

        currentSuccessCount = Mathf.Clamp(
            ProductionUpgradeData.PrintStartBonus,
            0,
            requiredSuccessCount - 1
        );
        isPlaying = true;

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        if (progressSlider != null)
        {
            progressSlider.minValue = 0;
            progressSlider.maxValue = requiredSuccessCount;
            progressSlider.value = currentSuccessCount;
        }

        if (printMinigamePanel != null)
            printMinigamePanel.SetActive(true);

        ResetMovingBar();
        RandomizeSuccessZone();

        Debug.Log("[PrintMinigame] 미니게임 시작");
    }

    private void MoveBar()
    {
        if (timingBarArea == null || movingBar == null) return;

        float halfAreaWidth = timingBarArea.rect.width * 0.5f;
        float halfBarWidth = movingBar.rect.width * 0.5f;

        float minX = -halfAreaWidth + halfBarWidth;
        float maxX = halfAreaWidth - halfBarWidth;

        Vector2 pos = movingBar.anchoredPosition;
        pos.x += moveDirection * barSpeed * Time.deltaTime;

        if (pos.x >= maxX)
        {
            pos.x = maxX;
            moveDirection = -1;
        }
        else if (pos.x <= minX)
        {
            pos.x = minX;
            moveDirection = 1;
        }

        movingBar.anchoredPosition = pos;
    }

    private void CheckTiming()
    {
        if (IsBarInSuccessZone())
            Success();
        else
            Fail();
    }

    private bool IsBarInSuccessZone()
    {
        if (movingBar == null || successZone == null) return false;

        float barX = movingBar.anchoredPosition.x;
        float zoneCenterX = successZone.anchoredPosition.x;
        float zoneHalfWidth = successZone.rect.width * 0.5f;

        return barX >= zoneCenterX - zoneHalfWidth &&
               barX <= zoneCenterX + zoneHalfWidth;
    }

    private void Success()
    {
        currentSuccessCount++;

        if (progressSlider != null)
            progressSlider.value = currentSuccessCount;

        Debug.Log($"[PrintMinigame] 성공: {currentSuccessCount}/{requiredSuccessCount}");

        if (currentSuccessCount >= requiredSuccessCount)
        {
            CompletePrinting();
            return;
        }

        ResetMovingBar();
        RandomizeSuccessZone();
    }

    private void Fail()
    {
        Debug.Log("[PrintMinigame] 실패. 진행도 증가 + 만족도 패널티 저장");

        MinigamePenaltyData.AddPenalty(failSatisfactionPenalty);

        currentSuccessCount++;

        if (progressSlider != null)
            progressSlider.value = currentSuccessCount;

        if (currentSuccessCount >= requiredSuccessCount)
        {
            CompletePrinting();
            return;
        }

        ResetMovingBar();
        RandomizeSuccessZone();
    }

    private void ResetMovingBar()
    {
        if (movingBar == null) return;

        movingBar.anchoredPosition = new Vector2(0f, movingBar.anchoredPosition.y);
        moveDirection = Random.value < 0.5f ? -1 : 1;
    }

    private void RandomizeSuccessZone()
    {
        if (timingBarArea == null || successZone == null) return;

        Vector2 size = successZone.sizeDelta;
        size.x = successZoneWidth;
        successZone.sizeDelta = size;

        float halfAreaWidth = timingBarArea.rect.width * 0.5f;
        float halfZoneWidth = successZone.rect.width * 0.5f;

        float minX = -halfAreaWidth + halfZoneWidth;
        float maxX = halfAreaWidth - halfZoneWidth;

        float randomX = Random.Range(minX, maxX);

        successZone.anchoredPosition =
            new Vector2(randomX, successZone.anchoredPosition.y);
    }

    private void CompletePrinting()
    {
        isPlaying = false;

        if (currentRecipe == null)
        {
            Debug.LogWarning("[PrintMinigame] currentRecipe가 없습니다.");
            return;
        }

        if (slotA == null || slotB == null || slotC == null)
        {
            Debug.LogWarning("[PrintMinigame] 재료 슬롯이 없습니다.");
            return;
        }

        if (Inventory.instance == null)
        {
            Debug.LogWarning("[PrintMinigame] Inventory.instance가 없습니다.");
            return;
        }

        if (ItemDataBase.instance == null)
        {
            Debug.LogWarning("[PrintMinigame] ItemDataBase.instance가 없습니다.");
            return;
        }

        Inventory.instance.Remove(slotA.item, slotA.count);
        Inventory.instance.Remove(slotB.item, slotB.count);
        Inventory.instance.Remove(slotC.item, slotC.count);

        Item resultItem = ItemDataBase.instance.GetById(currentRecipe.resultId);

        if (resultItem != null)
        {
            Inventory.instance.Add(resultItem, Mathf.Max(1, currentRecipe.resultCount));
            Debug.Log($"[PrintMinigame] 제작 완료: {resultItem.itemName}");
        }
        else
        {
            Debug.LogWarning($"[PrintMinigame] 결과 아이템을 찾지 못했습니다: {currentRecipe.resultId}");
        }

        if (printMinigamePanel != null)
            printMinigamePanel.SetActive(false);

        SceneManager.LoadScene(mainSceneName, LoadSceneMode.Single);
    }

    private void CancelMinigame()
    {
        isPlaying = false;

        if (printMinigamePanel != null)
            printMinigamePanel.SetActive(false);

        Debug.Log("[PrintMinigame] 프린트 미니게임 취소");
    }
}