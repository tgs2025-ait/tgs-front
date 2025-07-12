using UnityEngine;

public class PhysicalControl : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // SerialReceiveから取得したZ軸加速度値を使用
        float zAcceleration = SerialReceive.zAcceleration;
        float zAccelerationFixed = zAcceleration - 0.98f;
        float sensitivity = 0.6f; // 感度を調整するための変数
        float rotationSpeed = 100f; // 回転速度を調整するための変数
        
        // 加速度が一定以上ある場合に回転を適用
        // 0.1fは調整可能なしきい値です。微細な動きに反応しないようにするため
        if (Mathf.Abs(zAccelerationFixed) > 0.1f)
        {
            
            // 加速度の方向にX軸周りの回転を計算
            // Time.deltaTimeを掛けることでフレームレートに依存しない動きにする
            // sensitivityを掛けることで感度を調整する
            float rotationAmount = -zAccelerationFixed * rotationSpeed * sensitivity * Time.deltaTime;

            // 現在のX軸回転角度を取得
            float currentRotationX = transform.eulerAngles.x;
            
            // 角度を-180から180の範囲に正規化
            if (currentRotationX > 180f)
            {
                currentRotationX -= 360f;
            }
            
            // 新しい回転角度を計算
            float newRotationX = currentRotationX + rotationAmount;
            
            // -60度から60度の範囲に制限
            newRotationX = Mathf.Clamp(newRotationX, -60f, 60f);
            
            // 制限された角度でオブジェクトを回転させる
            transform.rotation = Quaternion.Euler(newRotationX, transform.eulerAngles.y, transform.eulerAngles.z);
            
            Debug.Log("rotationAmount: " + rotationAmount + ", currentRotationX: " + currentRotationX + ", newRotationX: " + newRotationX);
        }
    }
}
