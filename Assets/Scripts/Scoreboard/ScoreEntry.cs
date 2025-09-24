using System;

/// <summary>
/// Represents a single row in the local score ranking board.
/// </summary>
public sealed class ScoreEntry
{
    public const string TimestampFormat = "yyyy-MM-dd HH:mm:ss";

    public long Id { get; }
    public int Rank { get; }
    public int ScoreValue { get; }
    public string AchievedAtJst { get; }
    public bool IsSelf { get; }

    public ScoreEntry(long id, int rank, int scoreValue, string achievedAtJst, bool isSelf)
    {
        Id = id;
        Rank = rank;
        ScoreValue = scoreValue;
        AchievedAtJst = achievedAtJst;
        IsSelf = isSelf;
    }

    public ScoreEntry WithRank(int rank) => new ScoreEntry(Id, rank, ScoreValue, AchievedAtJst, IsSelf);

    public ScoreEntry MarkAsSelf() => new ScoreEntry(Id, Rank, ScoreValue, AchievedAtJst, true);

    public static string ToJstString(DateTimeOffset utcNow)
    {
        var jst = utcNow.ToOffset(TimeSpan.FromHours(9));
        return jst.ToString(TimestampFormat);
    }
}
