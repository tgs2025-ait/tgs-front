using UnityEngine;

/// <summary>
/// Handles editor/debug shortcuts on the Welcome scene for maintaining local score data.
/// </summary>
public class ScoreMaintenanceShortcuts : MonoBehaviour
{
    [Header("直前スコア削除ショートカット (例: ⌃⌫)")]
    [SerializeField, InspectorName("キー (⌫/Delete)")] private KeyCode deleteLatestKey = KeyCode.Backspace;
    [SerializeField, InspectorName("⌃ Control 必須")] private bool latestRequiresCtrl = true;
    [SerializeField, InspectorName("⇧ Shift 必須")] private bool latestRequiresShift = false;
    [SerializeField, InspectorName("⌥ Option 必須")] private bool latestRequiresAlt = false;

    [Header("全スコア削除ショートカット (例: ⌃⇧⌫)")]
    [SerializeField, InspectorName("キー (⌫/Delete)")] private KeyCode deleteAllKey = KeyCode.Backspace;
    [SerializeField, InspectorName("⌃ Control 必須")] private bool allRequiresCtrl = true;
    [SerializeField, InspectorName("⇧ Shift 必須")] private bool allRequiresShift = true;
    [SerializeField, InspectorName("⌥ Option 必須")] private bool allRequiresAlt = false;

    [Header("ログ出力")]
    [SerializeField, InspectorName("コンソールに出力")] private bool logToConsole = true;
    [SerializeField, InspectorName("デバッグ: キー入力ログ")] private bool debugKeyLogging = false;

    private void Update()
    {
        if (debugKeyLogging && Input.anyKeyDown)
        {
            Debug.Log($"ScoreMaintenanceShortcuts(Debug): DeleteLatest={Input.GetKeyDown(deleteLatestKey)}, DeleteAll={Input.GetKeyDown(deleteAllKey)}, Ctrl={Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)}, Shift={Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)}, Alt={Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)}");
        }

        if (IsShortcutPressed(deleteAllKey, allRequiresCtrl, allRequiresShift, allRequiresAlt))
        {
            HandleDeleteAll();
            return;
        }

        if (IsShortcutPressed(deleteLatestKey, latestRequiresCtrl, latestRequiresShift, latestRequiresAlt))
        {
            HandleDeleteLatest();
        }
    }

    private bool IsShortcutPressed(KeyCode key, bool requireCtrl, bool requireShift, bool requireAlt)
    {
        if (!Input.GetKeyDown(key))
        {
            return false;
        }

        if (!CheckModifier(requireCtrl, KeyCode.LeftControl, KeyCode.RightControl))
        {
            return false;
        }

        if (requireShift && !(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
        {
            return false;
        }

        if (requireAlt && !(Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)))
        {
            return false;
        }

        return true;
    }

    private bool CheckModifier(bool required, KeyCode leftKey, KeyCode rightKey)
    {
        var pressed = Input.GetKey(leftKey) || Input.GetKey(rightKey);

        if (required)
        {
            return pressed;
        }

        return true;
    }

    private void HandleDeleteLatest()
    {
        var deleted = ScoreRepository.DeleteMostRecentScore();
        if (deleted)
        {
            ScoreSessionData.Clear();
            if (logToConsole)
            {
                Debug.Log("ScoreMaintenanceShortcuts: 最新スコアを削除しました。");
            }
        }
        else if (logToConsole)
        {
            Debug.Log("ScoreMaintenanceShortcuts: 削除対象のスコアがありませんでした。");
        }
    }

    private void HandleDeleteAll()
    {
        var removedCount = ScoreRepository.DeleteAllScores();
        ScoreSessionData.Clear();

        if (logToConsole)
        {
            if (removedCount > 0)
            {
                Debug.Log($"ScoreMaintenanceShortcuts: 全スコアを削除しました (件数: {removedCount})。");
            }
            else
            {
                Debug.Log("ScoreMaintenanceShortcuts: 削除対象のスコアがありませんでした。");
            }
        }
    }
}
