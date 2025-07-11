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
        // Y軸周りの回転を適用するための変数
        // ここでは仮の値を設定しています。実際にはデバイスからの入力を取得する必要があります。
        float yAcceleration = 0.98f;
        float sensitivity = 1f; // 感度を調整するための変数
        float rotationSpeed = 100f; // 回転速度を調整するための変数
        // 加速度が一定以上ある場合に回転を適用
        // 0.1fは調整可能なしきい値です。微細な動きに反応しないようにするため
        if (Mathf.Abs(yAcceleration) > 0.1f)
        {
            // 加速度の方向にX軸周りの回転を計算
            // Time.deltaTimeを掛けることでフレームレートに依存しない動きにする
            // sensitivityを掛けることで感度を調整する
            float rotationAmount = -yAcceleration * rotationSpeed * sensitivity * Time.deltaTime;

            // オブジェクトをX軸周りに回転させる
            // Space.Worldを指定することで、オブジェクトのローカル軸ではなくワールド軸を基準に回転させる
            transform.Rotate(rotationAmount, 0, 0, Space.World);
        }
    }
}
