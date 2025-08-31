using UnityEngine;
using System.Linq;

public class RankingManager : MonoBehaviour
{
    // ★どこからでもアクセスできる唯一のインスタンス
    public static RankingManager Instance { get; private set; }

    [SerializeField]
    private RankingView rankingView;

    private RankingData rankingData = new RankingData();
    private const string RankingDataKey = "RankingData";

    // オブジェクトが生成された瞬間に呼ばれる
    void Awake()
    {
        // シングルトンの実装
        // もし他にインスタンスがなければ、自分をインスタンスとして設定
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(this.gameObject); // シーンをまたいで使いたい場合はこの行を有効にする
        }
        // もし既にインスタンスがあれば、自分自身を破棄
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        LoadRanking();
        // 最初のランキング表示（表示するViewがないシーンではエラーになるのでnullチェック）
        if (rankingView != null)
        {
            rankingView.DisplayRanking(rankingData);
        }
    }

    // 新しいスコアを追加する関数
    public void AddScore(int newScore)
    {
        ScoreData scoreData = new ScoreData { score = newScore, date = System.DateTime.Now };
        rankingData.ranking.Add(scoreData);
        rankingData.ranking = rankingData.ranking.OrderByDescending(x => x.score).ToList();

        if (rankingData.ranking.Count > 5)
        {
            rankingData.ranking.RemoveRange(5, rankingData.ranking.Count - 5);
        }

        SaveRanking();
        
        // ランキング表示を更新（表示するViewがないシーンではエラーになるのでnullチェック）
        if (rankingView != null)
        {
            rankingView.DisplayRanking(rankingData);
        }
    }

    // ランキングデータを保存する
    private void SaveRanking()
    {
        string json = JsonUtility.ToJson(rankingData);
        PlayerPrefs.SetString(RankingDataKey, json);
        PlayerPrefs.Save();
        Debug.Log("Ranking saved! Score: " + json);
    }

    // ランキングデータを読み込む
    private void LoadRanking()
    {
        string json = PlayerPrefs.GetString(RankingDataKey, "");
        if (!string.IsNullOrEmpty(json))
        {
            rankingData = JsonUtility.FromJson<RankingData>(json);
            Debug.Log("Ranking loaded!");
        }
    }

    // 外部にランキングデータを渡す関数
    public RankingData GetRankingData()
    {
        return rankingData;
    }

    // --- デバッグ用 ---
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            AddScore(Random.Range(0, 1000));
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            PlayerPrefs.DeleteKey(RankingDataKey);
            rankingData.ranking.Clear();
            Debug.Log("Ranking data deleted.");
            // ★追記: データ削除後にランキング表示を更新
            rankingView.DisplayRanking(rankingData);
        }
    }
}