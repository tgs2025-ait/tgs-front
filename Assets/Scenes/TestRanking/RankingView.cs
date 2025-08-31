using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class RankingView : MonoBehaviour
{
    // 外部からテキストリストを割り当てる
    [SerializeField]
    private List<TextMeshProUGUI> rankingTexts;

    // RankingManagerから呼び出される表示更新用の関数
    public void DisplayRanking(RankingData rankingData)
    {
        for (int i = 0; i < rankingTexts.Count; i++)
        {
            if (i < rankingData.ranking.Count)
            {
                // データがある場合：順位とスコアを表示
                rankingTexts[i].text = string.Format("{0}. {1} pt", i + 1, rankingData.ranking[i].score);
            }
            else
            {
                // データがない場合：ハイフンなどを表示
                rankingTexts[i].text = string.Format("{0}. ---", i + 1);
            }
        }
    }
}