using UnityEngine;
using UnityEngine.UI;

public class AugmentCardUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image iconImage;
    [SerializeField] private Text nameText;
    [SerializeField] private Text descriptionText;
    [SerializeField] private Text rankText;
    [SerializeField] private Button selectButton;

    private AugmentData currentData;
    private AugmentManager augmentManager;

    public void Setup(AugmentData data, AugmentManager manager)
    {
        currentData = data;
        augmentManager = manager;

        if (currentData == null)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);

        if (iconImage != null)
        {
            iconImage.sprite = currentData.icon;
            iconImage.enabled = currentData.icon != null;
        }

        if (nameText != null)
            nameText.text = currentData.augmentName;

        if (descriptionText != null)
            descriptionText.text = currentData.description;

        if (rankText != null)
            rankText.text = currentData.rank.ToString();

        if (selectButton != null)
        {
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(OnClickSelect);
        }
    }

    private void OnClickSelect()
    {
        if (augmentManager == null)
        {
            Debug.LogWarning("[AugmentCardUI] AugmentManager가 없습니다.");
            return;
        }

        if (currentData == null)
        {
            Debug.LogWarning("[AugmentCardUI] 선택할 AugmentData가 없습니다.");
            return;
        }

        augmentManager.SelectAugment(currentData);
    }

    public void Clear()
    {
        currentData = null;

        if (iconImage != null)
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
        }

        if (nameText != null)
            nameText.text = "";

        if (descriptionText != null)
            descriptionText.text = "";

        if (rankText != null)
            rankText.text = "";

        if (selectButton != null)
            selectButton.onClick.RemoveAllListeners();

        gameObject.SetActive(false);
    }
}