using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Script : MonoBehaviour
{
    [Header("今回のスコアを表示するテキスト")]
    public TMP_Text scoreText;

    [Header("ランキング一覧を表示するテキスト")]
    public TMP_Text rankingText;

    [Header("ランキングに表示する件数")]
    public int leaderboardDisplayCount = 10;

    private void Start()
    {
        RefreshScoreboard();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("WelcomeScene");
        }
    }

    private void RefreshScoreboard()
    {
        var hasLastEntry = ScoreSessionData.HasLastEntry;
        var currentScore = hasLastEntry ? ScoreSessionData.LastScoreValue : PointMemory.point;

        try
        {
            var displayCount = Mathf.Max(1, leaderboardDisplayCount);
            var entries = ScoreRepository.GetTopScores(displayCount);
            var selfId = ScoreSessionData.IsLastEntryStored ? ScoreSessionData.LastEntryId : null;
            ScoreEntry? selfEntry = null;
            if (selfId.HasValue)
            {
                selfEntry = ScoreRepository.GetEntryById(selfId.Value);
            }

            int? calculatedRank = null;
            if (selfEntry == null && hasLastEntry && !string.IsNullOrEmpty(ScoreSessionData.LastAchievedAtJst))
            {
                try
                {
                    calculatedRank = ScoreRepository.GetRankEstimate(currentScore, ScoreSessionData.LastAchievedAtJst);
                }
                catch (System.Exception rankEx)
                {
                    Debug.LogWarning($"順位の推定に失敗しました: {rankEx.Message}");
                }
            }

            if (rankingText != null)
            {
                rankingText.text = BuildRankingDisplay(entries, selfId);
            }

            if (scoreText != null)
            {
                if (selfEntry != null)
                {
                    scoreText.text = $"{selfEntry.Rank}: {selfEntry.ScoreValue.ToString("D3")}";
                }
                else if (calculatedRank.HasValue)
                {
                    scoreText.text = $"{calculatedRank.Value}: {currentScore.ToString("D3")}";
                }
                else if (hasLastEntry)
                {
                    scoreText.text = $"--: {currentScore.ToString("D3")}";
                }
                else
                {
                    scoreText.text = currentScore.ToString();
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"ランキングの取得に失敗しました: {ex.Message}");
            if (rankingText != null)
            {
                rankingText.text = "ランキングを読み込めませんでした。";
            }
            if (scoreText != null)
            {
                scoreText.text = currentScore.ToString();
            }
        }
    }

    private string BuildRankingDisplay(IReadOnlyList<ScoreEntry> entries, long? selfId)
    {
        if (entries.Count == 0)
        {
            return "まだスコアが登録されていません";
        }

        var builder = new StringBuilder();
        
        foreach (var entry in entries)
        {
            var isSelf = selfId.HasValue && entry.Id == selfId.Value;
            builder.Append(entry.ScoreValue.ToString("D3"));
            if (isSelf)
            {
                builder.Append(" <= YOU");
            }
            builder.Append('\n');
        }

        return builder.ToString().TrimEnd('\n');
    }
}
