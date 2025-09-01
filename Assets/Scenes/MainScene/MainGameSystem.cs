using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MainGameSystem : MonoBehaviour
{
    public TMP_Text scoreText;
    [Header("ゲームの指示やメッセージを表示するテキスト")]
    public TMP_Text instructionText;
    [Header("BoidControllerのPrefab")]
    public GameObject boidController;

    [Header("爆発のアニメーションPrefab")]
    public GameObject explosion;
    private bool isAttacking = false;
    private Vector3 originalPosition;
    private Vector3 parentOriginalPosition;

    [Header("息継ぎメーターUIをここにセットしてください")]
    public Slider breathingMeterSlider;
    // BoidControllerの移動速度（Unityインスペクターから指定可能）
    [Header("BoidControllerの移動速度（z軸負方向）")]
    public float boidControllerMoveSpeed;
    [Header("攻撃にかかる時間(攻撃しているとゲームが判断している時間です)")]
    public float attackDelay;
    [Header("何秒ごとに息を減らすか")]
    public float decreaseBreathingTime;

    //残っている息の量
    [HideInInspector]
    public float breathing = 1f;
    // 生成されたBoidControllerのリスト
    private List<GameObject> spawnedBoidControllers = new List<GameObject>();
    //自動攻撃対象のBoidControllerのリスト
    // nullのものは攻撃対象から除外されていることを示す
    private List<GameObject> activeBoidControllers = new List<GameObject>();
    [Header("シャチオブジェクトの親オブジェクトであるMoveGroupをここにセットしてください")]
    public GameObject moveGroup;

    void Start()
    {
        PointMemory.point = 0;
        originalPosition = transform.position;
        parentOriginalPosition = transform.parent.position;
        // コルーチンでBoidControllerの自動スポーンを開始
        StartCoroutine(SpawnBoidControllerCoroutine());
        // 5秒ごとにbreathingを0.1-0.3減らすコルーチンを開始
        StartCoroutine(BreathingDecreaseCoroutine());
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        // 手動スポーン（デバッグ用、必要なら残す）
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y + 3f, transform.position.z + 30f);
            GameObject obj = Instantiate(boidController, spawnPos, transform.rotation);
            spawnedBoidControllers.Add(obj);
            activeBoidControllers.Add(obj);
        }

        // スペースキーが押されたらシーンを切り替える
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("FinishScene");
        }
        // スコアを表示
        scoreText.text = PointMemory.point.ToString();

        // リターンキーを押している間だけローカル座標を基準からz軸方向に10だけ動かす
        // リターンキーを押している間、滑らかにz軸方向に5だけ移動し、離したら元の位置に滑らかに戻る

        
        // デバッグ用：加速度値と姿勢角度を定期的に表示
        if (Time.frameCount % 60 == 0) // 60フレームごとに表示
        {
            Debug.Log($"値 - ay: {SerialReceive.ayAcceleration}, pitch: {SerialReceive.pitchAngle}, roll: {SerialReceive.rollAngle}");
        }
        

        // if(!isAttacking) {
        //     Attack();  
        //     isAttacking = true;
        //     StartCoroutine(UpdateAttackingStateCoroutine());
        // };   
        if(breathing <= 0.3f){
            instructionText.text =  Mathf.Floor(breathing * 100f) + "% breath left. Breathe!";
        }else{
            instructionText.text = "";
        }
        if(breathing < 0.1f){
            Debug.Log(GetComponent<CountdownTimer>().countdownTime);
            setBreathing(1f);
            GetComponent<CountdownTimer>().countdownTime /= 2f;
        }


        // BoidControllerの移動と破棄処理
        for (int i = spawnedBoidControllers.Count - 1; i >= 0; i--)
        {
            GameObject obj = spawnedBoidControllers[i];
            if (obj == null)
            {
                spawnedBoidControllers.RemoveAt(i);
                activeBoidControllers.RemoveAt(i);
                continue;
            }
            // z軸負方向に移動
            obj.transform.position += new Vector3(0, 0, -boidControllerMoveSpeed * Time.deltaTime);

            // 自分のz座標を超えたら破棄
            if (obj.transform.position.z + 20 <= transform.position.z)
            {
                Destroy(obj);
                spawnedBoidControllers.RemoveAt(i);
                activeBoidControllers.RemoveAt(i);
            }
        }
    }

    // 5秒ごとにbreathingを0.1-0.3の間でランダムに減らすコルーチン
    // 指定した量だけ呼吸量を減らしてメーターに反映する処理
    private void DecreaseBreathing(float decrease)
    {
        breathing -= decrease;
        breathing = Mathf.Clamp01(breathing); // 0未満にならないように
        if (breathingMeterSlider != null)
        {
            breathingMeterSlider.value = breathing;
        }
        Debug.Log($"breathingが{decrease:F2}減少。現在値: {breathing:F2}");
    }
    public void setBreathing(float breathing){
        this.breathing = breathing;
        if (breathingMeterSlider != null)
        {
            breathingMeterSlider.value = breathing;
        }
        Debug.Log($"breathingを{breathing:F2}に設定。現在値: {breathing:F2}");
    }

    // 5秒ごとに呼吸量を減らすコルーチン
    private IEnumerator BreathingDecreaseCoroutine()
    {
        yield return new WaitForSeconds(decreaseBreathingTime); // ゲーム開始後5秒待つ
        while (true)
        {
            if(moveGroup.GetComponent<Move>().isThrowing) yield return new WaitForSeconds(decreaseBreathingTime);
            float decrease = Random.Range(0.1f, 0.3f);
            DecreaseBreathing(decrease);
            yield return new WaitForSeconds(decreaseBreathingTime);
        }
    }

    // 本来は毎フレーム呼び出すべきですが、不具合があるため一度だけ呼び出します(Update関数参照)
    void Attack(){
        // アニメーションを一回再生する
        Animator animator = GetComponent<Animator>();
        animator.SetTrigger("trigger");
        float targetZ = 5f;
        Vector3 targetPosition = originalPosition + new Vector3(0, 0, targetZ);
        Vector3 parentTargetPosition = parentOriginalPosition + new Vector3(0, 0, targetZ);
        transform.parent.position = Vector3.Lerp(transform.parent.position, parentTargetPosition, Time.deltaTime * 8f);
        transform.position = Vector3.Lerp(transform.position, targetPosition - new Vector3(0, 0, transform.parent.position.z), Time.deltaTime * 2f);

        DecreaseBreathing(0.1f);

    }

    //BoidControllerを自動で生成する
    IEnumerator SpawnBoidControllerCoroutine()
    {

        while (true)
        {
            float waitTime = Random.Range(1f, 5f);
            yield return new WaitForSeconds(waitTime);
            if(!moveGroup.GetComponent<Move>().isThrowing){//息継ぎしてない時にBoidControllerを生成
                Vector3 spawnPos = new Vector3(moveGroup.transform.position.x, moveGroup.transform.position.y + 3f, moveGroup.transform.position.z + 30f);
                GameObject obj = Instantiate(boidController, spawnPos, transform.rotation);
                obj.SetActive(true);
                spawnedBoidControllers.Add(obj);
                activeBoidControllers.Add(obj);
            }
        }
    }

    private IEnumerator UpdateAttackingStateCoroutine(){
        yield return new WaitForSeconds(attackDelay);
        isAttacking = false;
        Debug.Log("攻撃フラグOFF（" + attackDelay + "秒経過）");
    }

    void OnTriggerEnter(Collider collision)
    {
        // 衝突したオブジェクトの名前を表示
        // Debug.Log("衝突しました！対象: " + collision.gameObject.name);


        // 群全体を攻撃対象から除外
        if(collision.gameObject.name == "BoidController(Clone)"){
            if(!isAttacking){
            Attack();  
            StartCoroutine(UpdateAttackingStateCoroutine());
            isAttacking = true;
            }
            activeBoidControllers.Remove(collision.gameObject);
            Debug.Log("攻撃対象から除外:" + collision.gameObject.name);
            //衝突判定用のColliderを無効化
            collision.gameObject.GetComponent<Collider>().enabled = false;
        }

        if (collision.gameObject.name == "Bone" && isAttacking)
        {
            GameObject toRemove = collision.gameObject.transform.parent.parent.gameObject;
            // explositionオブジェクトをtoremoveの位置に生成
            Instantiate(explosion, toRemove.transform.position, Quaternion.identity);
            Debug.Log("消去:" + toRemove.name);
            PointMemory.point += 1;
            Destroy(toRemove);
        }

    }
}
