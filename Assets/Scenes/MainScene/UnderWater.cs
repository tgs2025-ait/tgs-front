using UnityEngine;
// 水没の判定
public class UnderWater : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject underWaterPlane;
    public Camera mainCamera;
    void Start()
    {
        
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
        }
     }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.Equals(mainCamera.gameObject))
        {
            underWaterPlane.SetActive(false);
        }
    }
}
