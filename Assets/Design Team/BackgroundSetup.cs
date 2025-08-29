using UnityEngine;

public class BackgroundSetup : MonoBehaviour
{
    public Camera bgCamera;      // 背景用カメラ
    public RenderTexture bgRT;   // 作成した RenderTexture
    public Material planeMat;    // Plane のマテリアル

    void Start() {
        // 背景カメラの出力先を RenderTexture に設定
        bgCamera.targetTexture = bgRT;

        // Plane のマテリアルに RenderTexture をセット
        planeMat.SetTexture("_MainTex", bgRT);
    }
}
