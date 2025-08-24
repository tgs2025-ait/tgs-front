using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class CountdownTimer : MonoBehaviour
{
    [SerializeField] private float countdownTime = 60f; // カウントダウンの初期時間（秒）
    [SerializeField] private TMP_Text timeText; // TextMeshProのテキストコンポーネント
    [SerializeField] private string nextSceneName = "NextScene"; // 遷移先のシーン名
    [SerializeField] private Color normalColor = Color.white; // 通常時の色
    [SerializeField] private Color warningColor = Color.red; // 警告時の色（残り10秒以下）

    private bool isCountingDown = true;

    void Start()
    {
        if (timeText == null)
        {
            Debug.LogError("Time Text (TMP_Text) is not assigned!");
            return;
        }

        // 初期表示
        UpdateDisplay();
        
        StartCoroutine(Countdown());
    }

    IEnumerator Countdown()
    {
        while (countdownTime > 0 && isCountingDown)
        {
            countdownTime -= Time.deltaTime;
            UpdateDisplay();

            yield return null;
        }

        // カウントダウン終了時
        if (countdownTime <= 0)
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    void UpdateDisplay()
    {
        int minutes = Mathf.FloorToInt(countdownTime / 60);
        int seconds = Mathf.FloorToInt(countdownTime % 60);
        string timeString = string.Format("{0:00}:{1:00}", minutes, seconds);

        timeText.text = timeString;

        // 残り10秒未満なら警告色に変更
        if (countdownTime < 10f)
        {
            timeText.color = warningColor;
        }
        else
        {
            timeText.color = normalColor; // 通常時は白に戻す
        }
    }

    // カウントダウンを一時停止/再開するメソッド（任意で追加）
    public void TogglePause()
    {
        isCountingDown = !isCountingDown;
    }

    // カウントダウンをリセットするメソッド（任意で追加）
    public void ResetTimer(float newTime)
    {
        countdownTime = newTime;
        UpdateDisplay();
    }
}
