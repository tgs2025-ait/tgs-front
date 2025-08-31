using System; // Serializable属性を使うために必要

// 1つのスコアデータを表すクラス
[Serializable]
public class ScoreData
{
    public int score;
    public DateTime date; // スコアを獲得した日時などを保存したい場合
}

// ランキング全体（スコアデータのリスト）を表すクラス
[Serializable]
public class RankingData
{
    public System.Collections.Generic.List<ScoreData> ranking = new System.Collections.Generic.List<ScoreData>();
}
