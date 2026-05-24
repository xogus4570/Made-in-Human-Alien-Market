using UnityEngine;
using UnityEngine.UI;

public class ShopSlotLevelLock : MonoBehaviour
{
    [Header("필요 레벨")]
    [SerializeField] private int requiredLevel = 1;

    [Header("잠글 구매 버튼")]
    [SerializeField] private Button buyButton;

    [Header("잠금 표시 UI")]
    [SerializeField] private GameObject lockOverlay;
    [SerializeField] private Image lockIcon;
    [SerializeField] private Text lockText;

    private void OnEnable()
    {
        RefreshLockState();
    }

    public void RefreshLockState()
    {
        int currentLevel = 1;

        if (GameDataManager.Instance != null)
            currentLevel = GameDataManager.Instance.level;

        bool unlocked = currentLevel >= requiredLevel;

        if (buyButton != null)
            buyButton.interactable = unlocked;

        if (lockOverlay != null)
            lockOverlay.SetActive(!unlocked);

        if (lockIcon != null)
            lockIcon.enabled = !unlocked;

        if (lockText != null)
        {
            lockText.gameObject.SetActive(!unlocked);
            lockText.text = "Lv." + requiredLevel + " 필요";
        }
    }
}