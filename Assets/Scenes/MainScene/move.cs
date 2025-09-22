using UnityEngine;
using System.Collections;
//移動を制御するスクリプト
public class Move : MonoBehaviour
{
    [HideInInspector]
    public bool isThrowing = false;
    private Vector3 throwVelocity = Vector3.zero;
    private float gravity = -20f; // 重力加速度
    [Header("息継ぎの時に水面から出る勢い(これに現在の水深を反映させたウェイトと足されます)")]
    public float throwPower; // 投射初速度
    [Header("斜方投射の角度（度）")]
    public float throwAngle = 45f; // 斜方投射の角度
    private Vector3 basePosition = Vector3.zero;   // 現在のジャンプ基準位置
    private Vector3 throwStartPosition = Vector3.zero; // 斜方投射開始時の位置
    public GameObject orca;

    // 回転補間用
    private Quaternion targetRotation = Quaternion.identity;
    private float rotationLerpSpeed = 3f; // 補間速度
    [Header("水面のY座標(移動時に水面から出ないようにします)")]
    public float waterY = 0f;

    [Header("移動速度(基準)")]
    [SerializeField] private float baseMoveSpeed = 5f;
    [Header("しきい値ちょうどでの最小倍率(>0で\"動き出し\")")]
    [Range(0f, 1f)] [SerializeField] private float minSpeedMultiplier = 0.25f;
    [Header("センサー最大時の速度倍率(基準×この値)")]
    [SerializeField] private float maxSpeedMultiplier = 2.0f;
    [Header("ピッチ角|ロール角がこの絶対値で最大速に達する(度)")]
    [SerializeField] private float pitchAbsForFullSpeed = 70f;
    [SerializeField] private float rollAbsForFullSpeed = 70f;
    [Header("速度カーブ(1=直線, 2=ゆっくり立ち上がり)")]
    [SerializeField] private float speedCurveExponent = 1.0f;

    private Vector3 originalPosition;

    void Start()
    {
        if (orca != null)
        {
            targetRotation = orca.transform.localRotation;
        }
        else
        {
            targetRotation = Quaternion.identity;
        }
        originalPosition = transform.position;
    }

    void Update()
    {
        // 入力の符号(方向)と速度倍率を別々に扱う
        float horizontalSign = 0f;
        float vertical = 0f;     // 前後はキーボードのみ(現状)
        float upDownSign = 0f;
        float horizontalMul = 0f; // 0=停止, 1=基準速, >1=加速
        float upDownMul = 0f;
        if((Input.GetKeyDown(KeyCode.L) && !isThrowing)|| (SerialReceive.ayAcceleration >= SensorThresholds.AyAccelerationForBreath && !isThrowing)) ThrowOblique();
        HandleThrow();
        Quaternion rotation = Quaternion.Euler(0, 0, 0);
        if (Input.GetKey(KeyCode.W)) vertical += 1f;
        if (Input.GetKey(KeyCode.S)) vertical -= 1f;

        // 左右: キー or センサーしきい値
        bool leftByKey = Input.GetKey(KeyCode.A);
        bool rightByKey = Input.GetKey(KeyCode.D);
        bool leftBySensor = SerialReceive.pitchAngle <= SensorThresholds.PitchLeftThreshold;
        bool rightBySensor = SerialReceive.pitchAngle >= SensorThresholds.PitchRightThreshold;

        if (leftByKey || leftBySensor)
        {
            horizontalSign -= 1f;
            float mul = leftByKey ? 1f : MapAngleToMultiplier(Mathf.Abs(SerialReceive.pitchAngle), Mathf.Abs(SensorThresholds.PitchLeftThreshold), pitchAbsForFullSpeed);
            horizontalMul = Mathf.Max(horizontalMul, mul);
            rotation = Quaternion.Euler(0, 0, -SerialReceive.pitchAngle);
        }
        if (rightByKey || rightBySensor)
        {
            horizontalSign += 1f;
            float mul = rightByKey ? 1f : MapAngleToMultiplier(SerialReceive.pitchAngle, SensorThresholds.PitchRightThreshold, pitchAbsForFullSpeed);
            horizontalMul = Mathf.Max(horizontalMul, mul);
            rotation = Quaternion.Euler(0, 0, -SerialReceive.pitchAngle);
        }

        // 上下: キー or センサーしきい値
        bool upByKey = Input.GetKey(KeyCode.UpArrow);
        bool downByKey = Input.GetKey(KeyCode.DownArrow);
        bool upBySensor = SerialReceive.rollAngle >= SensorThresholds.RollUpThreshold;
        bool downBySensor = SerialReceive.rollAngle <= SensorThresholds.RollDownThreshold;

        if (upByKey || upBySensor)
        {
            upDownSign += 1f;
            float mul = upByKey ? 1f : MapAngleToMultiplier(SerialReceive.rollAngle, SensorThresholds.RollUpThreshold, rollAbsForFullSpeed);
            upDownMul = Mathf.Max(upDownMul, mul);
            rotation = Quaternion.Euler(-SerialReceive.rollAngle, 0, 0);
        }
        if (downByKey || downBySensor)
        {
            upDownSign -= 1f;
            float mul = downByKey ? 1f : MapAngleToMultiplier(Mathf.Abs(SerialReceive.rollAngle), Mathf.Abs(SensorThresholds.RollDownThreshold), rollAbsForFullSpeed);
            upDownMul = Mathf.Max(upDownMul, mul);
            rotation = Quaternion.Euler(-SerialReceive.rollAngle, 0, 0);
        }

        // 速度ベクトル（正規化しない: 倍率で速度を変化させる）
        Vector3 velocity = new Vector3(
            horizontalSign * baseMoveSpeed * horizontalMul,
            upDownSign * baseMoveSpeed * upDownMul,
            vertical * baseMoveSpeed
        );
        // 斜方投射中は通常移動を無効化
        if (!isThrowing)
        {
            orca.transform.localRotation = rotation;
            transform.Translate(velocity * Time.deltaTime, Space.World);
            //水面から出そうになったら強制的に戻す
            if(transform.position.y > waterY - 3f){
                transform.position = new Vector3(transform.position.x, waterY - 3f, transform.position.z);
            }
        }

        // 回転を線形補間でorcaのlocalRotationに適用
        if (orca != null)
        {
            orca.transform.localRotation = Quaternion.Lerp(orca.transform.localRotation, targetRotation, Time.deltaTime * rotationLerpSpeed);
        }
    }

    // 角度(しきい値以上)を速度倍率にマップ
    // valueAbsOrSigned: 右上(正方向)は符号付き, 左下(負方向)は絶対値を渡す実装にしています
    // startAt: 動き出しの角度(しきい値)
    // fullAtAbs: この絶対角で最大倍率に到達
    private float MapAngleToMultiplier(float valueAbsOrSigned, float startAt, float fullAtAbs)
    {
        // startAt と fullAtAbs は同符号 or 正値として扱う
        float startAbs = Mathf.Abs(startAt);
        float vAbs = Mathf.Abs(valueAbsOrSigned);
        float fullAbs = Mathf.Abs(fullAtAbs);
        // vAbs を [startAbs, fullAbs] にクランプして 0..1 に正規化
        float vClamped = Mathf.Clamp(vAbs, startAbs, fullAbs);
        float t = Mathf.InverseLerp(startAbs, fullAbs, vClamped);
        // カーブ適用(1=直線)
        t = Mathf.Pow(t, Mathf.Max(0.0001f, speedCurveExponent));
        // 最小/最大倍率へ補間
        return Mathf.Lerp(minSpeedMultiplier, maxSpeedMultiplier, t);
    }

    // 息継ぎ用関数(斜方投射ジャンプ)
    public void ThrowOblique()
    {
        isThrowing = true;
        //これやらないとシャチが回転してくれません
        orca.GetComponent<Animator>().enabled = false;

        basePosition = transform.position;
        throwStartPosition = transform.position;



        float throwPowerWeight = (basePosition.y - waterY)/(originalPosition.y - waterY);

        orca.GetComponent<MainGameSystem>().setBreathing(1f);
        // 進行方向（前方）を基準に斜方投射
        // 水平成分（XZ平面）をtransform.forwardで取得
        Vector3 forwardXZ = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
        if (forwardXZ == Vector3.zero) forwardXZ = Vector3.forward; // 万一ゼロベクトルならZ+方向

        float rad = throwAngle * Mathf.Deg2Rad;
        // 速度ベクトルを分解
        float v = throwPower + throwPowerWeight;
        float vy = v * Mathf.Sin(rad);
        float vh = v * Mathf.Cos(rad);

        throwVelocity = forwardXZ * vh + Vector3.up * vy;
    }

    // Update内で呼び出す処理(斜方投射ジャンプ)
    void HandleThrow()
    {
        if (isThrowing)
        {
            throwVelocity += Vector3.up * gravity * Time.deltaTime;
            Vector3 pos = throwStartPosition;

            // y座標のみを更新
            float deltaY = throwVelocity.y * Time.deltaTime;
            float newY = transform.position.y + deltaY;

            // 地面（ジャンプ開始位置）に戻ったら終了
            if (newY <= basePosition.y)
            {
                newY = basePosition.y;
                StartCoroutine(EnableOrcaAnimatorAfterDelay(rotationLerpSpeed));//線形補間のための時間を待ってからアニメーションを再開
                
                isThrowing = false;
                throwVelocity = Vector3.zero;
            }
            Vector3 currentPos = transform.position;
            currentPos.y = newY;
            transform.position = currentPos;

            // 速度ベクトルの向きに合わせてx軸周りに回転
            // XZ平面上の速度ベクトル
            Vector3 velocityXZ = new Vector3(throwVelocity.x, 0, throwVelocity.z);
            float speedXZ = velocityXZ.magnitude;
            float speedY = throwVelocity.y;

            // x軸回転角度（上向きが正、下向きが負）
            float angleX = 0f;
            if (speedXZ > 0.0001f)
            {
                angleX = Mathf.Atan2(speedY, speedXZ) * Mathf.Rad2Deg;
            }
            // y軸回転は進行方向に合わせる
            float angleY = 0f;
            if (velocityXZ != Vector3.zero)
            {
                angleY = Mathf.Atan2(velocityXZ.x, velocityXZ.z) * Mathf.Rad2Deg;
            }
            // 回転を適用（線形補間用にtargetRotationを更新）
            targetRotation = Quaternion.Euler(-angleX, angleY, 0f);
        }
    }

    // 線形補完のための時間を待ってからアニメーションを再開
    IEnumerator EnableOrcaAnimatorAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (orca != null)
        {
            Animator animator = orca.GetComponent<Animator>();
            if (animator != null)
            {
                animator.enabled = true;
                animator.Rebind();
                animator.Update(0f);
            }
        }
    }
}
