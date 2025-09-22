using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RollAngleSceneTransition : MonoBehaviour
{
    [Header("シーン遷移設定")]
    [SerializeField] private string mainGameSceneName = "MainGameScene";
    [SerializeField] private float checkInterval = 0.1f; // チェック間隔（秒）
    [SerializeField] private float rollAngleThreshold = 35f; // roll角度の閾値
    
    [Header("デバッグ設定")]
    [SerializeField] private bool enableDebugLog = true;
    [SerializeField] private bool enableSpaceKeyDebugTransition = true;
    
    private bool hasTransitioned = false; // 既にシーン遷移したかどうかのフラグ

    void Start()
    {
        if (enableDebugLog)
        {
            Debug.Log("RollAngleSceneTransition: 開始 - roll角度監視を開始します");
        }
        
        // 定期的にroll角度をチェックするコルーチンを開始
        StartCoroutine(CheckRollAngle());
    }

    void Update()
    {
        if (hasTransitioned || !enableSpaceKeyDebugTransition)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (enableDebugLog)
            {
                Debug.Log("RollAngleSceneTransition: スペースキー入力を検出しました。MainGameSceneに遷移します。");
            }

            hasTransitioned = true;
            TransitionToMainGameScene();
        }
    }
    
    /// <summary>
    /// roll角度を定期的にチェックするコルーチン
    /// </summary>
    private IEnumerator CheckRollAngle()
    {
        while (!hasTransitioned)
        {
            // SerialReceive.rollAngleの値を取得
            float currentRollAngle = SerialReceive.rollAngle;
            
            if (enableDebugLog)
            {
                Debug.Log($"RollAngleSceneTransition: 現在のroll角度: {currentRollAngle:F2}° (閾値: {rollAngleThreshold}°)");
            }
            
            // roll角度が閾値を超えた場合、シーン遷移を実行
            if (currentRollAngle >= rollAngleThreshold)
            {
                if (enableDebugLog)
                {
                    Debug.Log($"RollAngleSceneTransition: roll角度 {currentRollAngle:F2}° が閾値 {rollAngleThreshold}° を超えました。MainGameSceneに遷移します。");
                }
                
                // シーン遷移フラグを設定
                hasTransitioned = true;
                
                // MainGameSceneに遷移
                TransitionToMainGameScene();
                
                // コルーチンを終了
                yield break;
            }
            
            // 指定された間隔でチェック
            yield return new WaitForSeconds(checkInterval);
        }
    }
    
    /// <summary>
    /// MainGameSceneに遷移する
    /// </summary>
    private void TransitionToMainGameScene()
    {
        try
        {
            if (enableDebugLog)
            {
                Debug.Log($"RollAngleSceneTransition: {mainGameSceneName} へのシーン遷移を開始します");
            }
            
            // シーン遷移を実行
            SceneManager.LoadScene(mainGameSceneName);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"RollAngleSceneTransition: シーン遷移中にエラーが発生しました: {e.Message}");
        }
    }
    
    /// <summary>
    /// 手動でシーン遷移をトリガーする（デバッグ用）
    /// </summary>
    [ContextMenu("手動でシーン遷移")]
    public void ManualTransition()
    {
        if (enableDebugLog)
        {
            Debug.Log("RollAngleSceneTransition: 手動シーン遷移が実行されました");
        }
        
        hasTransitioned = true;
        TransitionToMainGameScene();
    }
    
    /// <summary>
    /// 現在のroll角度を取得する（デバッグ用）
    /// </summary>
    [ContextMenu("現在のroll角度を表示")]
    public void ShowCurrentRollAngle()
    {
        float currentRollAngle = SerialReceive.rollAngle;
        Debug.Log($"RollAngleSceneTransition: 現在のroll角度: {currentRollAngle:F2}°");
    }
}
