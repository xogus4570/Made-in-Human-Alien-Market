public static class MinigamePenaltyData
{
    public static int pendingSatisfactionPenalty = 0;

    public static void AddPenalty(int amount)
    {
        if (amount <= 0) return;
        pendingSatisfactionPenalty += amount;
    }

    public static int ConsumePenalty()
    {
        int value = pendingSatisfactionPenalty;
        pendingSatisfactionPenalty = 0;
        return value;
    }
}