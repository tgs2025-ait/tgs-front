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

    [Header("自分の順位を表示するテキスト")]
    public TMP_Text selfRankText;

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
        if (scoreText != null)
        {
            scoreText.text = currentScore.ToString();
        }

        try
        {
            var displayCount = Mathf.Max(1, leaderboardDisplayCount);
            var entries = ScoreRepository.GetTopScores(displayCount);
            var selfId = ScoreSessionData.IsLastEntryStored ? ScoreSessionData.LastEntryId : null;

            if (rankingText != null)
            {
                rankingText.text = BuildRankingDisplay(entries, selfId);
            }

            if (selfRankText != null)
            {
                if (selfId.HasValue)
                {
                    var selfEntry = ScoreRepository.GetEntryById(selfId.Value);
                    if (selfEntry != null)
                    {
                        selfRankText.text = $"現在の順位: {selfEntry.Rank} 位";
                    }
                    else
                    {
                        selfRankText.text = "今回のスコアはランキング圏外でした";
                    }
                }
                else if (hasLastEntry)
                {
                    selfRankText.text = "今回のスコアはランキング圏外でした";
                }
                else
                {
                    selfRankText.text = "現在の順位: -";
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
            if (selfRankText != null)
            {
                selfRankText.text = "現在の順位: -";
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
            builder.AppendFormat("{0,2}位 : {1}", entry.Rank, entry.ScoreValue);
            if (isSelf)
            {
                builder.Append(" ← 今回");
            }
            builder.Append('\n');
            builder.Append("      ").Append(entry.AchievedAtJst);
            builder.Append('\n');
        }

        return builder.ToString().TrimEnd('\n');
    }
}
