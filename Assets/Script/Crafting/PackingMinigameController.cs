using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PackingMinigameController : MonoBehaviour
{
    [Header("미니게임 패널")]
    [SerializeField] private GameObject packingMinigamePanel;

    [Header("게이지 이미지")]
    [SerializeField] private Image gaugeFillImage;

    [Header("설정")]
    [SerializeField] private int requiredPressCount = 20;

    [Header("완료 후 돌아갈 메인 씬")]
    [SerializeField] private string mainSceneName = "GwonTaeHyeon_Test";

    private IngredientSlotUI slotA;
    private IngredientSlotUI slotB;
    private IngredientSlotUI slotC;
    private Recipe currentRecipe;

    private int currentPressCount;
    private bool isPlaying;

    private void Start()
    {
        if (packingMinigamePanel != null)
            packingMinigamePanel.SetActive(false);

        SetupGaugeImage();
        SetGauge(0f);

        isPlaying = false;
    }

    private void Update()
    {
        if (!isPlaying)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
            AddPress();

        if (Input.GetKeyDown(KeyCode.Escape))
            CancelMinigame();
    }

    public void StartMinigame(IngredientSlotUI a, IngredientSlotUI b, IngredientSlotUI c, Recipe recipe)
    {
        slotA = a;
        slotB = b;
        slotC = c;
        currentRecipe = recipe;

        currentPressCount = Mathf.Clamp(
            ProductionUpgradeData.PackingStartBonus,
            0,
            requiredPressCount - 1
        );

        float progress = (float)currentPressCount / requiredPressCount;
        isPlaying = true;

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        SetupGaugeImage();
        SetGauge(progress);

        if (packingMinigamePanel != null)
            packingMinigamePanel.SetActive(true);

        Debug.Log("[PackingMinigame] 미니게임 시작");
    }

    private void SetupGaugeImage()
    {
        if (gaugeFillImage == null)
        {
            Debug.LogWarning("[PackingMinigame] Gauge Fill Image가 연결되지 않았습니다.");
            return;
        }

        if (gaugeFillImage.sprite == null)
        {
            Debug.LogWarning("[PackingMinigame] GaugeFill의 Source Image가 비어 있습니다. Image 컴포넌트에 UISprite 같은 Sprite를 넣어주세요.");
            return;
        }

        gaugeFillImage.type = Image.Type.Filled;
        gaugeFillImage.fillMethod = Image.FillMethod.Horizontal;
        gaugeFillImage.fillOrigin = 0;
        gaugeFillImage.fillAmount = 0f;
    }

    private void AddPress()
    {
        currentPressCount++;

        float progress = (float)currentPressCount / requiredPressCount;
        SetGauge(progress);

        Debug.Log($"[PackingMinigame] 연타: {currentPressCount}/{requiredPressCount}, 게이지={progress}");

        if (currentPressCount >= requiredPressCount)
            CompletePacking();
    }

    private void SetGauge(float value)
    {
        if (gaugeFillImage != null)
            gaugeFillImage.fillAmount = Mathf.Clamp01(value);
    }

    private void CompletePacking()
    {
        isPlaying = false;

        if (currentRecipe == null)
        {
            Debug.LogWarning("[PackingMinigame] currentRecipe가 없습니다.");
            return;
        }

        if (slotA == null || slotB == null || slotC == null)
        {
            Debug.LogWarning("[PackingMinigame] 재료 슬롯이 없습니다.");
            return;
        }

        if (Inventory.instance == null)
        {
            Debug.LogWarning("[PackingMinigame] Inventory.instance가 없습니다.");
            return;
        }

        if (ItemDataBase.instance == null)
        {
            Debug.LogWarning("[PackingMinigame] ItemDataBase.instance가 없습니다.");
            return;
        }

        Inventory.instance.Remove(slotA.item, slotA.count);
        Inventory.instance.Remove(slotB.item, slotB.count);
        Inventory.instance.Remove(slotC.item, slotC.count);

        Item resultItem = ItemDataBase.instance.GetById(currentRecipe.resultId);

        if (resultItem != null)
        {
            Inventory.instance.Add(resultItem, Mathf.Max(1, currentRecipe.resultCount));
            Debug.Log($"[PackingMinigame] 제작 완료: {resultItem.itemName}");
        }
        else
        {
            Debug.LogWarning($"[PackingMinigame] 결과 아이템을 찾지 못했습니다: {currentRecipe.resultId}");
        }

        if (packingMinigamePanel != null)
            packingMinigamePanel.SetActive(false);

        SceneManager.LoadScene(mainSceneName, LoadSceneMode.Single);
    }

    private void CancelMinigame()
    {
        isPlaying = false;

        if (packingMinigamePanel != null)
            packingMinigamePanel.SetActive(false);

        Debug.Log("[PackingMinigame] 포장 미니게임 취소");
    }
}