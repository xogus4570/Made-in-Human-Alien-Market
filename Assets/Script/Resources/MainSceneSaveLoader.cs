using System.Collections;
using UnityEngine;

public class MainSceneSaveLoader : MonoBehaviour
{
    [Header("로드 설정")]
    [SerializeField] private bool loadAfterOneFrame = true;

    [Header("UI 새로고침 설정")]
    [SerializeField] private bool refreshUIAfterLoad = true;
    [SerializeField] private int extraRefreshFrameCount = 2;

    private IEnumerator Start()
    {
        if (loadAfterOneFrame)
            yield return null;

        if (!SaveGameManager.ConsumeLoadRequest())
            yield break;

        Debug.Log("[이어하기] 저장 데이터 불러오기를 시작합니다.");

        SaveGameManager.Load();

        if (!refreshUIAfterLoad)
            yield break;

        // 로드 직후 1회 새로고침
        RefreshGameStatusUI("[이어하기] 로드 직후 상태 UI 새로고침");

        // 다른 Start/OnEnable/FSM 초기화가 한 박자 늦게 UI를 덮어쓸 수 있으므로
        // 몇 프레임 더 기다렸다가 다시 새로고침
        int frameCount = Mathf.Max(0, extraRefreshFrameCount);

        for (int i = 0; i < frameCount; i++)
        {
            yield return null;
            RefreshGameStatusUI($"[이어하기] 로드 후 추가 상태 UI 새로고침 {i + 1}/{frameCount}");
        }
    }

    private void RefreshGameStatusUI(string logMessage)
    {
        GameStatusUI statusUI = FindFirstObjectByType<GameStatusUI>(FindObjectsInactive.Include);

        if (statusUI == null)
        {
            Debug.LogWarning("[이어하기] GameStatusUI를 찾지 못해 상태 UI 새로고침을 생략했습니다.");
            return;
        }

        statusUI.RefreshStatusUI();

        Debug.Log(logMessage);
    }
}