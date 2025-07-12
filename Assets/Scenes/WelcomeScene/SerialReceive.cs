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
    public static float yAcceleration = 0f;
    public static float zAcceleration = 0f; // AZの値を保存する静的変数を追加

    void Start()
    {
        //信号を受信したときに、そのメッセージの処理を行う
        serialHandler.OnDataReceived += OnDataReceived;
    }

    //受信した信号(message)に対する処理
    void OnDataReceived(string message)
    {
        var data = message.Split(
                new string[] { "\n" }, System.StringSplitOptions.None);
        try
        {
            // 受信した生データをログに表示
            Debug.Log($"受信データ: {data[0]}");
            
            // データフォーマット: "AX:%.2f AY:%.2f AZ:%.2f" からAYとAZの値を抽出
            string[] parts = data[0].Split(' ');
            Debug.Log($"分割されたデータ: {string.Join(", ", parts)}");
            
            foreach (string part in parts)
            {
                if (part.StartsWith("AY:"))
                {
                    string yValue = part.Substring(3); // "AY:"を除去
                    Debug.Log($"AY部分: {part}, 抽出された値: {yValue}");
                    
                    if (float.TryParse(yValue, out float parsedValue))
                    {
                        yAcceleration = parsedValue;
                        Debug.Log($"Y軸加速度: {yAcceleration}");
                    }
                    else
                    {
                        Debug.LogWarning($"AYの値を数値に変換できませんでした: {yValue}");
                    }
                }
                else if (part.StartsWith("AZ:"))
                {
                    string zValue = part.Substring(3); // "AZ:"を除去
                    Debug.Log($"AZ部分: {part}, 抽出された値: {zValue}");
                    
                    if (float.TryParse(zValue, out float parsedValue))
                    {
                        zAcceleration = parsedValue;
                        Debug.Log($"Z軸加速度: {zAcceleration}");
                    }
                    else
                    {
                        Debug.LogWarning($"AZの値を数値に変換できませんでした: {zValue}");
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"シリアルデータ処理エラー: {e.Message}");//エラーを表示
        }
    }
}
