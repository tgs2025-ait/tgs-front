/// <summary>
/// Stores per-session information about the most recent score submission.
/// </summary>
public static class ScoreSessionData
{
    public static bool HasLastEntry => _hasLastEntry;
    public static bool IsLastEntryStored => _lastEntryStored;
    public static long? LastEntryId => _lastEntryId;
    public static int LastScoreValue => _lastScoreValue;
    public static string LastAchievedAtJst => _lastAchievedAtJst;

    private static bool _hasLastEntry;
    private static bool _lastEntryStored;
    private static long? _lastEntryId;
    private static int _lastScoreValue;
    private static string _lastAchievedAtJst;

    public static void SetLastEntry(ScoreEntry entry, bool storedInDatabase)
    {
        _hasLastEntry = entry != null;
        _lastEntryStored = _hasLastEntry && storedInDatabase;
        _lastEntryId = _lastEntryStored ? entry.Id : null;
        _lastScoreValue = entry?.ScoreValue ?? 0;
        _lastAchievedAtJst = entry?.AchievedAtJst;
    }

    public static void Clear()
    {
        _hasLastEntry = false;
        _lastEntryStored = false;
        _lastEntryId = null;
        _lastScoreValue = 0;
        _lastAchievedAtJst = null;
    }
}
