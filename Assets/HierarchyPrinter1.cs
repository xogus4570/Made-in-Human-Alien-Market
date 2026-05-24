using System.Text;
using UnityEngine;

public class HierarchyPrinter1 : MonoBehaviour
{
    [Header("특정 루트만 출력하고 싶으면 연결")]
    [SerializeField] private Transform targetRoot;

    [ContextMenu("Copy Hierarchy To Clipboard")]
    public void CopyHierarchyToClipboard()
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("========== Hierarchy Print Start ==========");

        if (targetRoot != null)
        {
            AppendTransform(sb, targetRoot, 0);
        }
        else
        {
            GameObject[] rootObjects = gameObject.scene.GetRootGameObjects();

            foreach (GameObject rootObject in rootObjects)
            {
                AppendTransform(sb, rootObject.transform, 0);
            }
        }

        sb.AppendLine("========== Hierarchy Print End ==========");

        GUIUtility.systemCopyBuffer = sb.ToString();

        Debug.Log("[HierarchyPrinter1] Hierarchy 내용이 클립보드에 복사되었습니다. 메모장에 Ctrl + V 하세요.");
    }

    private void AppendTransform(StringBuilder sb, Transform target, int depth)
    {
        string indent = new string(' ', depth * 2);
        sb.AppendLine(indent + "- " + target.name);

        for (int i = 0; i < target.childCount; i++)
        {
            AppendTransform(sb, target.GetChild(i), depth + 1);
        }
    }
}