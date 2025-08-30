using UnityEngine;
using UnityEngine.UI; // Sliderを扱うために必要
using TMPro;          // TextMeshProを扱うためにこの行を追加

public class HPBar : MonoBehaviour
{
    // インスペクターから操作したいSliderを割り当てる
    [SerializeField]
    private Slider hpSlider;

    // 表示用のテキストをインスペクターから割り当てる
    [SerializeField]
    private TextMeshProUGUI hpText; // この行を追記

    void Start()
    {
        // ゲーム開始時にSliderの値を最大に設定する
        hpSlider.maxValue = 100;
        hpSlider.minValue = 0;
        hpSlider.value = 100;
        
        // ★追記: 起動時のテキストを更新
        UpdateHPText(hpSlider.value);

        // ★追記: スライダーの値が変更された時にテキストを更新するようイベントを登録
        hpSlider.onValueChanged.AddListener(UpdateHPText);
    }

    void Update()
    {

        /*
        テスト用
        スペースキーを押すとHPが1づつ減少する
        */

        // もしスペースキーが「押された瞬間」なら
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Sliderの値を1減らす
            hpSlider.value -= 2;
        }
    }

    // ★追記: テキストを更新するための関数
    private void UpdateHPText(float currentValue)
    {
        // テキストを「HP: {現在の値}」という形式で更新する
        hpText.text = "HP: " + currentValue;
    }
    
    // ★追記: オブジェクトが破棄される時にイベントを解除（お作法）
    private void OnDestroy()
    {
        if (hpSlider != null)
        {
            hpSlider.onValueChanged.RemoveListener(UpdateHPText);
        }
    }
}