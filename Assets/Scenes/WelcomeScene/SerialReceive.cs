//参照：https://qiita.com/Ninagawa123/items/f6595dcf788dd316be8a
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SerialReceive : MonoBehaviour
{
    //https://qiita.com/yjiro0403/items/54e9518b5624c0030531
    //上記URLのSerialHandler.cのクラス
    public SerialHandler serialHandler;

    // 静的変数として加速度値を保存（他のスクリプトからアクセス可能）
    public static float xAcceleration = 0f; // aXの値を保存する静的変数を追加
    public static float yAcceleration = 0f;
    public static float zAcceleration = 0f; // azの値を保存する静的変数を追加

    void Start()
    {
        //信号を受信したときに、そのメッセージの処理を行う
        serialHandler.OnDataReceived += OnDataReceived;
        
        // 静的変数の初期化確認
        Debug.Log("SerialReceive Start - 静的変数初期化完了");
        Debug.Log($"初期値 - X: {xAcceleration}, Y: {yAcceleration}, Z: {zAcceleration}");
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
            
            // データフォーマット: "ax:%.3f, ay:%.3f, az:%.3f\n" からax、ay、azの値を抽出
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
                
                if (cleanPart.StartsWith("ax:"))
                {
                    string xValue = cleanPart.Substring(3); // "ax:"を除去
                    Debug.Log($"ax部分: {cleanPart}, 抽出された値: {xValue}");
                    
                    if (float.TryParse(xValue, out float parsedValue))
                    {
                        xAcceleration = parsedValue;
                        Debug.Log($"X軸加速度: {xAcceleration}");
                    }
                    else
                    {
                        Debug.LogWarning($"axの値を数値に変換できませんでした: {xValue}");
                    }
                }
                else if (cleanPart.StartsWith("ay:"))
                {
                    string yValue = cleanPart.Substring(3); // "ay:"を除去
                    Debug.Log($"ay部分: {cleanPart}, 抽出された値: {yValue}");
                    
                    if (float.TryParse(yValue, out float parsedValue))
                    {
                        yAcceleration = parsedValue;
                        Debug.Log($"Y軸加速度: {yAcceleration}");
                    }
                    else
                    {
                        Debug.LogWarning($"ayの値を数値に変換できませんでした: {yValue}");
                    }
                }
                else if (cleanPart.StartsWith("az:"))
                {
                    string zValue = cleanPart.Substring(3); // "az:"を除去
                    Debug.Log($"az部分: {cleanPart}, 抽出された値: {zValue}");
                    
                    if (float.TryParse(zValue, out float parsedValue))
                    {
                        zAcceleration = parsedValue;
                        Debug.Log($"Z軸加速度: {zAcceleration}");
                    }
                    else
                    {
                        Debug.LogWarning($"azの値を数値に変換できませんでした: {zValue}");
                    }
                }
                else
                {
                    Debug.LogWarning($"認識できないパーツ形式: '{cleanPart}'");
                }
            }
            
            // 処理完了後の静的変数の値を確認
            Debug.Log($"処理完了後の加速度値 - X: {xAcceleration}, Y: {yAcceleration}, Z: {zAcceleration}");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"シリアルデータ処理エラー: {e.Message}");//エラーを表示
        }
    }
}
