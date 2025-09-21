using UnityEngine;
// 水没の判定
public class UnderWater : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject underWaterPlane;
    public Camera mainCamera;
    
    [Header("スカイボックス設定")]
    [Header("水中用スカイボックス")]
    public Material underwaterSkybox;  // 水中用スカイボックス
    [Header("通常用スカイボックス")]
    public Material normalSkybox;      // 通常用スカイボックス
    private Material originalSkybox;   // 元のスカイボックスを保存
    void Start()
    {
        // 現在のスカイボックスを保存
        originalSkybox = RenderSettings.skybox;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.Equals(mainCamera.gameObject))
        {
            underWaterPlane.SetActive(true);
            
            // 水中用スカイボックスに切り替え
            if (underwaterSkybox != null)
            {
                RenderSettings.skybox = underwaterSkybox;
                DynamicGI.UpdateEnvironment();
            }
        }
     }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.Equals(mainCamera.gameObject))
        {
            underWaterPlane.SetActive(false);
            
            // 通常用スカイボックスに戻す
            if (normalSkybox != null)
            {
                RenderSettings.skybox = normalSkybox;
            }
            else if (originalSkybox != null)
            {
                // normalSkyboxが設定されていない場合は元のスカイボックスに戻す
                RenderSettings.skybox = originalSkybox;
            }
            DynamicGI.UpdateEnvironment();
        }
    }
}
