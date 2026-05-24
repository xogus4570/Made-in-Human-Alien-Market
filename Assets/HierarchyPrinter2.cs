using System.Text;
using UnityEngine;

public class HierarchyPrinter2 : MonoBehaviour
{
    [Header("특정 루트만 출력하고 싶으면 연결")]
    [SerializeField] private Transform targetRoot;

    [ContextMenu("Print Lines And Copy Filtered Console Style")]
    public void PrintLinesAndCopyFilteredConsoleStyle()
    {
        StringBuilder sb = new StringBuilder();

        PrintAndAppendFilteredConsoleStyle(sb, "========== Hierarchy Print Start ==========");

        if (targetRoot != null)
        {
            PrintTransform(targetRoot, 0, sb);
        }
        else
        {
            GameObject[] rootObjects = gameObject.scene.GetRootGameObjects();

            foreach (GameObject rootObject in rootObjects)
            {
                PrintTransform(rootObject.transform, 0, sb);
            }
        }

        PrintAndAppendFilteredConsoleStyle(sb, "========== Hierarchy Print End ==========");

        GUIUtility.systemCopyBuffer = sb.ToString();

        Debug.Log("[HierarchyPrinter2] 불필요한 UnityEngine 내부 2줄을 제외한 Console 스타일 텍스트가 클립보드에 복사되었습니다.");
    }

    private void PrintTransform(Transform target, int depth, StringBuilder sb)
    {
        string indent = new string(' ', depth * 2);
        string line = indent + "- " + target.name;

        PrintAndAppendFilteredConsoleStyle(sb, line);

        for (int i = 0; i < target.childCount; i++)
        {
            PrintTransform(target.GetChild(i), depth + 1, sb);
        }
    }

    private void PrintAndAppendFilteredConsoleStyle(StringBuilder sb, string line)
    {
        Debug.Log(line);

        sb.AppendLine(line);

        string stackTrace = StackTraceUtility.ExtractStackTrace();
        string[] stackLines = stackTrace.Split('\n');

        foreach (string stackLine in stackLines)
        {
            string trimmedLine = stackLine.Trim();

            if (string.IsNullOrEmpty(trimmedLine))
                continue;

            if (trimmedLine.StartsWith("UnityEngine.Debug:ExtractStackTraceNoAlloc"))
                continue;

            if (trimmedLine.StartsWith("UnityEngine.StackTraceUtility:ExtractStackTrace"))
                continue;

            sb.AppendLine(trimmedLine);
        }

        sb.AppendLine();
    }
}