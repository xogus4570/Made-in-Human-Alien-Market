using UnityEngine;

public enum AugmentRank
{
    C,
    B,
    A
}

public enum AugmentEffectType
{
    AddGold,
    AddExp,
    AddInfluence,
    AddSatisfaction
}

[System.Serializable]
public class AugmentData
{
    [Header("기본 정보")]
    public string augmentId;
    public string augmentName;

    [TextArea(2, 4)]
    public string description;

    [Header("표시")]
    public Sprite icon;
    public AugmentRank rank;

    [Header("효과")]
    public AugmentEffectType effectType;
    public int value;
}