using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class BlinkingText : MonoBehaviour
{
    [SerializeField]
    private TMP_Text targetText;

    [SerializeField, Min(0.05f)]
    private float blinkFrequency = 1f; // 回/秒

    [SerializeField, Range(0f, 1f)]
    private float minimumAlpha = 0f;

    [SerializeField, Range(0f, 1f)]
    private float maximumAlpha = 1f;

    [SerializeField, Range(0f, 1f)]
    private float phaseOffsetNormalized = 0f;

    [SerializeField]
    private bool useUnscaledTime = true;

    private void Awake()
    {
        CacheTextReference();
        ApplyCurrentAlpha();
    }

    private void OnEnable()
    {
        ApplyCurrentAlpha();
    }

    private void Update()
    {
        if (targetText == null)
        {
            return;
        }

        ApplyCurrentAlpha();
    }

    private void ApplyCurrentAlpha()
    {
        float alpha = EvaluateAlpha(useUnscaledTime ? Time.unscaledTime : Time.time);
        ApplyAlpha(alpha);
    }

    private float EvaluateAlpha(float timeSource)
    {
        float safeFrequency = Mathf.Max(0.05f, blinkFrequency);
        float phase = (timeSource * safeFrequency + phaseOffsetNormalized) * Mathf.PI * 2f;
        float normalized = (Mathf.Cos(phase) + 1f) * 0.5f; // 0~1 の滑らかな波形
        return Mathf.Lerp(minimumAlpha, maximumAlpha, normalized);
    }

    private void ApplyAlpha(float alpha)
    {
        if (targetText == null)
        {
            return;
        }

        targetText.alpha = Mathf.Clamp01(alpha);
    }

    private void CacheTextReference()
    {
        if (targetText == null)
        {
            targetText = GetComponent<TMP_Text>();
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        CacheTextReference();
        blinkFrequency = Mathf.Max(0.05f, blinkFrequency);
        minimumAlpha = Mathf.Clamp01(minimumAlpha);
        maximumAlpha = Mathf.Clamp(maximumAlpha, minimumAlpha, 1f);
        phaseOffsetNormalized = Mathf.Repeat(phaseOffsetNormalized, 1f);
        if (!Application.isPlaying)
        {
            ApplyCurrentAlpha();
        }
    }

    private void Reset()
    {
        CacheTextReference();
        blinkFrequency = 1f;
        minimumAlpha = 0f;
        maximumAlpha = 1f;
        phaseOffsetNormalized = 0f;
        useUnscaledTime = true;
    }
#endif
}
