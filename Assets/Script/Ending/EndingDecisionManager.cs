using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingDecisionManager : MonoBehaviour
{
    [Header("True Ending")]
    [SerializeField] private int trueEndingGold = 50000;
    [SerializeField] private int trueEndingInfluence = 50000;

    [Header("Good Ending")]
    [SerializeField] private int goodEndingGold = 30000;
    [SerializeField] private int goodEndingInfluence = 30000;

    [Header("Bad Ending 1")]
    [SerializeField] private int bad1EndingInfluence = 30000;

    [Header("Bad Ending 2")]
    [SerializeField] private int bad2EndingGold = 15000;
    [SerializeField] private int bad2EndingInfluence = 15000;

    [Header("엔딩 씬 이름")]
    [SerializeField] private string endingSceneName = "Ending";

    public void CheckAndMoveToEnding(int gold, int influence)
    {
        EndingType endingType = DecideEnding(gold, influence);

        PlayerPrefs.SetInt("EndingType", (int)endingType);
        PlayerPrefs.Save();

        Debug.Log($"[EndingDecision] 엔딩 결정: {endingType} / Gold: {gold} / Influence: {influence}");

        SceneManager.LoadScene(endingSceneName);
    }

    private EndingType DecideEnding(int gold, int influence)
    {
        if (gold >= trueEndingGold && influence >= trueEndingInfluence)
            return EndingType.TrueEnding;

        if (gold >= goodEndingGold && influence >= goodEndingInfluence)
            return EndingType.GoodEnding;

        if (influence >= bad1EndingInfluence)
            return EndingType.BadEnding1;

        if (gold >= bad2EndingGold || influence >= bad2EndingInfluence)
            return EndingType.BadEnding2;

        return EndingType.BadEnding2;
    }
}