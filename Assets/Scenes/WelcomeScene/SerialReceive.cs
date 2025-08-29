//参照：https://qiita.com/Ninagawa123/items/f6595dcf788dd316be8a
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SerialReceive : MonoBehaviour
{
    //https://qiita.com/yjiro0403/items/54e9518b5624c0030531
    //上記URLのSerialHandler.cのクラス
    public SerialHandler serialHandler;

    // 静的変数として加速度値と姿勢角度を保存（他のスクリプトからアクセス可能）
    public static float ayAcceleration = 0f; // ayの値を保存する静的変数
    public static float pitchAngle = 0f; // pitch角度を保存する静的変数
    public static float rollAngle = 0f; // roll角度を保存する静的変数

    void Start()
    {
        //信号を受信したときに、そのメッセージの処理を行う
        serialHandler.OnDataReceived += OnDataReceived;
        
        // 静的変数の初期化確認
        Debug.Log("SerialReceive Start - 静的変数初期化完了");
        Debug.Log($"初期値 - ay: {ayAcceleration}, pitch: {pitchAngle}, roll: {rollAngle}");
    }

    //受信した信号(message)に対する処理
    void OnDataReceived(string message)
    {
        var data = message.Split(
                new string[] { "\n" }, System.StringSplitOptions.None);
        try
        {
            // 受信した生データをログに表示
            Debug.Log($"受信データ: '{data[0]}'");
            Debug.Log($"受信データの長さ: {data[0].Length}");
            Debug.Log($"受信データのバイト配列: {System.Text.Encoding.UTF8.GetBytes(data[0]).Length}");
            
            // データフォーマット: "ay:%.3f, pitch:%.3f, roll:%.3f\n" からay、pitch、rollの値を抽出
            // 改行文字を除去してから分割
            string cleanData = data[0].Replace("\n", "").Replace("\r", "");
            Debug.Log($"改行文字除去後のデータ: '{cleanData}'");
            
            string[] parts = cleanData.Split(',');
            Debug.Log($"分割されたデータ: {string.Join(", ", parts)}");
            
            Debug.Log($"パーツ数: {parts.Length}");
            foreach (string part in parts)
            {
                // パーツの前後の空白を除去（改行文字は既に除去済み）
                string cleanPart = part.Trim();
                Debug.Log($"処理中のパート: '{cleanPart}' (長さ: {cleanPart.Length})");
                
                if (cleanPart.StartsWith("ay:"))
                {
                    string ayValue = cleanPart.Substring(3); // "ay:"を除去
                    Debug.Log($"ay部分: {cleanPart}, 抽出された値: {ayValue}");
                    
                    if (float.TryParse(ayValue, out float parsedValue))
                    {
                        ayAcceleration = parsedValue;
                        Debug.Log($"ay加速度: {ayAcceleration}");
                    }
                    else
                    {
                        Debug.LogWarning($"ayの値を数値に変換できませんでした: {ayValue}");
                    }
                }
                else if (cleanPart.StartsWith("pitch:"))
                {
                    string pitchValue = cleanPart.Substring(6); // "pitch:"を除去
                    Debug.Log($"pitch部分: {cleanPart}, 抽出された値: {pitchValue}");
                    
                    if (float.TryParse(pitchValue, out float parsedValue))
                    {
                        pitchAngle = parsedValue;
                        Debug.Log($"pitch角度: {pitchAngle}");
                    }
                    else
                    {
                        Debug.LogWarning($"pitchの値を数値に変換できませんでした: {pitchValue}");
                    }
                }
                else if (cleanPart.StartsWith("roll:"))
                {
                    string rollValue = cleanPart.Substring(5); // "roll:"を除去
                    Debug.Log($"roll部分: {cleanPart}, 抽出された値: {rollValue}");
                    
                    if (float.TryParse(rollValue, out float parsedValue))
                    {
                        rollAngle = parsedValue;
                        Debug.Log($"roll角度: {rollAngle}");
                    }
                    else
                    {
                        Debug.LogWarning($"rollの値を数値に変換できませんでした: {rollValue}");
                    }
                }
                else
                {
                    Debug.LogWarning($"認識できないパーツ形式: '{cleanPart}'");
                }
            }
            
            // 処理完了後の静的変数の値を確認
            Debug.Log($"処理完了後の値 - ay: {ayAcceleration}, pitch: {pitchAngle}, roll: {rollAngle}");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"シリアルデータ処理エラー: {e.Message}");//エラーを表示
        }
    }
}
