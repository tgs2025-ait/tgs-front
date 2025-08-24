using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Textを使う場合は必要
using TMPro;          // TextMeshProを使う場合は必要
using UnityEngine.SceneManagement;

public class CountdownController : MonoBehaviour
{
    // カウントダウン表示用のUIテキスト
    [SerializeField]
    private TextMeshProUGUI countdownText;

    // カウントダウンの秒数
    [SerializeField]
    private int countdownSeconds = 3;

    // 遷移先のシーン名
    [SerializeField]
    private string nextSceneName = "GameScene"; // ここにゲーム本体のシーン名を入力

    void Start()
    {
        // コルーチンを開始
        StartCoroutine(CountdownCoroutine());
    }

    private IEnumerator CountdownCoroutine()
    {
        // 最初はテキストを非表示にするか、空にしておく
        countdownText.text = "";
        yield return new WaitForSeconds(0.5f); // 少し待つ

        int count = countdownSeconds;

        while (count > 0)
        {
            // 数字を表示
            countdownText.text = count.ToString();
            // ここで効果音を鳴らしても良い
            // audioSource.PlayOneShot(countSound);
            yield return new WaitForSeconds(1.0f); // 1秒待つ
            count--;
        }

        // 「GO!」などのテキストを表示
        countdownText.text = "GO!";
        // ここで開始の効果音を鳴らしても良い
        // audioSource.PlayOneShot(goSound);

        yield return new WaitForSeconds(1.0f); // 1秒待つ

        // 次のシーンに遷移
        //SceneManager.LoadScene(nextSceneName);
    }
}