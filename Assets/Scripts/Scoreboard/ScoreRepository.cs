using System;
using System.Collections.Generic;
using System.IO;
using SQLite;
using UnityEngine;

/// <summary>
/// Provides synchronous access to the local SQLite database that stores score rankings.
/// </summary>
public static class ScoreRepository
{
    private const string DatabaseFileName = "scores.db";
    public const int MaxEntries = 1000;

    private static readonly object SyncRoot = new object();

    private static SQLiteConnection _connection;
    private static bool _initialized;
    private static bool _quittingHooked;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeOnLoad()
    {
        try
        {
            EnsureInitialized();
        }
        catch (Exception ex)
        {
            Debug.LogError($"ScoreRepository initialization failed: {ex.Message}\n{ex}");
        }
    }

    public static void EnsureInitialized()
    {
        lock (SyncRoot)
        {
            if (_initialized && _connection != null)
            {
                return;
            }

            var path = Path.Combine(Application.persistentDataPath, DatabaseFileName);
            Directory.CreateDirectory(Path.GetDirectoryName(path));

            _connection = CreateConnection(path);
            CreateSchema();
            _initialized = true;

            if (!_quittingHooked)
            {
                Application.quitting += Dispose;
                _quittingHooked = true;
            }
        }
    }

    public static ScoreEntry AddScore(int scoreValue)
    {
        lock (SyncRoot)
        {
            EnsureInitialized();

            var achievedAt = ScoreEntry.ToJstString(DateTimeOffset.UtcNow);
            var model = new ScoreModel
            {
                ScoreValue = scoreValue,
                AchievedAtJst = achievedAt
            };

            try
            {
                _connection.Insert(model);
                var rank = CalculateRank(model.ScoreValue, model.AchievedAtJst);
                TrimExcessEntries();

                var storedModel = _connection.Find<ScoreModel>(model.Id);
                if (storedModel == null)
                {
                    // スコア上限により即時削除された場合でも、算出済みの順位を返す
                    return new ScoreEntry(model.Id, rank, model.ScoreValue, model.AchievedAtJst, false);
                }

                return new ScoreEntry(storedModel.Id, rank, storedModel.ScoreValue, storedModel.AchievedAtJst, false);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to add score: {ex.Message}\n{ex}");
                throw;
            }
        }
    }

    public static IReadOnlyList<ScoreEntry> GetTopScores(int limit)
    {
        lock (SyncRoot)
        {
            EnsureInitialized();

            var rows = _connection.Query<ScoreModel>(
                "SELECT Id, ScoreValue, AchievedAtJst FROM Scores ORDER BY ScoreValue DESC, AchievedAtJst ASC LIMIT ?",
                limit);

            var results = new List<ScoreEntry>(rows.Count);
            for (var i = 0; i < rows.Count; i++)
            {
                var model = rows[i];
                var rank = i + 1;
                results.Add(new ScoreEntry(model.Id, rank, model.ScoreValue, model.AchievedAtJst, false));
            }

            return results;
        }
    }

    public static ScoreEntry? GetEntryById(long id)
    {
        lock (SyncRoot)
        {
            EnsureInitialized();
            var model = _connection.Find<ScoreModel>(id);
            if (model == null)
            {
                return null;
            }

            var rank = CalculateRank(model.ScoreValue, model.AchievedAtJst);
            return new ScoreEntry(model.Id, rank, model.ScoreValue, model.AchievedAtJst, false);
        }
    }

    public static int GetRankEstimate(int scoreValue, string achievedAtJst)
    {
        lock (SyncRoot)
        {
            EnsureInitialized();
            return CalculateRank(scoreValue, achievedAtJst);
        }
    }

    public static void Dispose()
    {
        lock (SyncRoot)
        {
            _connection?.Close();
            _connection?.Dispose();
            _connection = null;
            _initialized = false;
        }
    }

    private static SQLiteConnection CreateConnection(string path)
    {
        try
        {
            return new SQLiteConnection(path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Failed to open database at {path}: {ex.Message}. Recreating file.");
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch (Exception cleanupEx)
            {
                Debug.LogError($"Failed to remove corrupt database at {path}: {cleanupEx.Message}\n{cleanupEx}");
                throw;
            }

            return new SQLiteConnection(path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);
        }
    }

    private static void CreateSchema()
    {
        _connection.CreateTable<ScoreModel>();
    }

    private static void TrimExcessEntries()
    {
        _connection.Execute(
            "DELETE FROM Scores WHERE Id NOT IN (SELECT Id FROM Scores ORDER BY ScoreValue DESC, AchievedAtJst ASC LIMIT ?)",
            MaxEntries);
    }

    private static int CalculateRank(int scoreValue, string achievedAtJst)
    {
        var betterCount = _connection.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM Scores WHERE ScoreValue > ? OR (ScoreValue = ? AND AchievedAtJst < ?)",
            scoreValue,
            scoreValue,
            achievedAtJst);

        return betterCount + 1;
    }

    [Table("Scores")]
    private sealed class ScoreModel
    {
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }

        [Indexed]
        public int ScoreValue { get; set; }

        [Indexed]
        public string AchievedAtJst { get; set; }
    }
}
