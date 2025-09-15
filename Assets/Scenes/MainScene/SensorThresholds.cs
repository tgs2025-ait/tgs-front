using UnityEngine;

// センサー関連の閾値を集約する定数クラス
public static class SensorThresholds
{
    // 息継ぎ（ジャンプ）判定の加速度閾値
    public const float AyAccelerationForBreath = 1.5f;

    // 左右のヨー（ここではpitchAngle）での旋回閾値（度）
    public const float PitchLeftThreshold = -35f;
    public const float PitchRightThreshold = 35f;

    // 上下のロールでの昇降閾値（度）
    public const float RollUpThreshold = 25f;
    public const float RollDownThreshold = -25f;
}


