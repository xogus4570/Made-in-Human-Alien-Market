using UnityEngine;
using UnityEngine.SceneManagement;

public class EscSaveAndReturnTitle : MonoBehaviour
{
    [Header("씬 이름")]
    [SerializeField] private string titleSceneName = "Title";

    [Header("ESC 두 번 입력 설정")]
    [SerializeField] private bool requireDoubleEsc = true;
    [SerializeField] private float doubleEscTime = 1.2f;

    [Header("타이틀 복귀 전 런타임 매니저 정리")]
    [SerializeField] private bool destroyRuntimeManagersBeforeTitle = true;

    private float lastEscTime = -999f;

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape))
            return;

        if (!requireDoubleEsc)
        {
            SaveAndReturnTitle();
            return;
        }

        if (Time.unscaledTime - lastEscTime <= doubleEscTime)
        {
            SaveAndReturnTitle();
            return;
        }

        lastEscTime = Time.unscaledTime;
        Debug.Log("[세이브] ESC를 한 번 더 누르면 자동 저장 후 타이틀로 돌아갑니다.");
    }

    private void SaveAndReturnTitle()
    {
        Debug.Log("[세이브] 자동 저장을 시작합니다.");

        SaveGameManager.Save();

        if (destroyRuntimeManagersBeforeTitle)
            DestroyRuntimeManagers();

        Debug.Log("[세이브] 타이틀 씬으로 이동합니다.");
        SceneManager.LoadScene(titleSceneName, LoadSceneMode.Single);
    }

    private void DestroyRuntimeManagers()
    {
        if (GameDataManager.Instance != null)
            Object.Destroy(GameDataManager.Instance.gameObject);

        if (Inventory.instance != null)
            Object.Destroy(Inventory.instance.gameObject);

        if (ItemDataBase.instance != null)
            Object.Destroy(ItemDataBase.instance.gameObject);

        if (OrderManager.Instance != null)
            Object.Destroy(OrderManager.Instance.gameObject);

        if (DailyResultManager.Instance != null)
            Object.Destroy(DailyResultManager.Instance.gameObject);

        Debug.Log("[세이브] 타이틀 복귀 전 런타임 매니저 정리 완료");
    }
}