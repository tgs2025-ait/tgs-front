using System.Collections;
using System.IO;
using System.IO.Ports;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BluetoothConnectionIndicator : MonoBehaviour
{
    [Header("デバイス設定")]
    [SerializeField] private string devicePath = "/dev/cu.orca-m5stick-c-plus-dev";
    [SerializeField] private float checkIntervalSeconds = 1.0f;

    [Header("表示設定")]
    [SerializeField] private string connectedMessage = "device connected";
    [SerializeField] private int fontSize = 18;
    [SerializeField] private Color textColor = Color.white;
    [SerializeField] private Color backgroundColor = new Color(0f, 0f, 0f, 0.5f);
    [SerializeField] private Vector2 padding = new Vector2(12f, 6f);
    [SerializeField] private Vector2 margin = new Vector2(16f, 16f);
    [SerializeField] private bool enableDebugLog = true;
    [SerializeField] private bool clampToScreen = true;

    private bool isConnected;
    [SerializeField] private Canvas targetCanvas;
    private Canvas uiCanvas;
    private RectTransform panelRect;
    private Image panelImage;
    private Text messageText;

    void Awake()
    {
        // UGUI生成は行わない（OnGUIで描画）
        SetActiveIndicator(false);
    }

    void OnEnable()
    {
        StartCoroutine(CheckConnectionRoutine());
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator CheckConnectionRoutine()
    {
        var wait = new WaitForSeconds(checkIntervalSeconds);
        while (true)
        {
            bool exists = false;
            try
            {
                if (!string.IsNullOrEmpty(devicePath))
                {
                    exists = File.Exists(devicePath);
                }
                if (enableDebugLog)
                {
                    Debug.Log($"BluetoothConnectionIndicator: exists={exists} at '{devicePath}'");
                }
            }
            catch (System.Exception ex)
            {
                if (enableDebugLog)
                {
                    Debug.LogWarning($"BluetoothConnectionIndicator: 例外: {ex.Message}");
                }
            }

            isConnected = exists;
            yield return wait;
        }
    }

	void Update()
	{
		SetActiveIndicator(isConnected);
		if (!isConnected) return;
		UpdateMessageAndLayout();
	}

	void OnGUI()
	{
		if (!isConnected) return;

		GUIStyle style = new GUIStyle(GUI.skin.label);
		style.fontSize = fontSize;
		style.normal.textColor = textColor;
		style.alignment = TextAnchor.MiddleCenter;

		GUIContent content = new GUIContent("ON");
		Vector2 textSize = style.CalcSize(content);
		float width = textSize.x + padding.x * 2f;
		float height = textSize.y + padding.y * 2f;
		float x = Screen.width - width - margin.x;
		float y = Screen.height - height - margin.y;
		Rect textRect = new Rect(x + padding.x, y + padding.y, width - padding.x * 2f, height - padding.y * 2f);
		GUI.Label(textRect, content, style);
	}

	private void EnsureCanvasAndWidgets()
	{
		uiCanvas = targetCanvas != null ? targetCanvas : GetComponentInChildren<Canvas>();
		if (uiCanvas == null)
		{
			GameObject canvasObj = new GameObject("BTIndicatorCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
			canvasObj.transform.SetParent(transform, false);
			uiCanvas = canvasObj.GetComponent<Canvas>();
			uiCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
			CanvasScaler scaler = canvasObj.GetComponent<CanvasScaler>();
			scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
			scaler.referenceResolution = new Vector2(1920, 1080);
			scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
			scaler.matchWidthOrHeight = 1f;
		}

		if (panelRect == null)
		{
			GameObject panelObj = new GameObject("BTIndicatorPanel", typeof(RectTransform), typeof(Image));
			panelObj.transform.SetParent(uiCanvas.transform, false);
			panelRect = panelObj.GetComponent<RectTransform>();
			panelImage = panelObj.GetComponent<Image>();
			panelRect.anchorMin = new Vector2(1f, 0f);
			panelRect.anchorMax = new Vector2(1f, 0f);
			panelRect.pivot = new Vector2(1f, 0f);

			GameObject textObj = new GameObject("Label", typeof(RectTransform), typeof(Text));
			textObj.transform.SetParent(panelRect, false);
			RectTransform textRect = textObj.GetComponent<RectTransform>();
			textRect.anchorMin = new Vector2(0f, 0f);
			textRect.anchorMax = new Vector2(1f, 1f);
			textRect.pivot = new Vector2(0.5f, 0.5f);
			messageText = textObj.GetComponent<Text>();
			messageText.alignment = TextAnchor.MiddleCenter;
			messageText.horizontalOverflow = HorizontalWrapMode.Overflow;
			messageText.verticalOverflow = VerticalWrapMode.Overflow;
		}
	}

	private void ApplyStyleToWidgets()
	{
		if (messageText != null)
		{
			messageText.fontSize = fontSize;
			messageText.color = textColor;
			messageText.text = connectedMessage;
			if (messageText.font == null)
			{
				messageText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
			}
		}

		if (panelImage != null)
		{
			panelImage.color = backgroundColor;
		}
	}

	private void UpdateMessageAndLayout()
	{
		if (messageText == null || panelRect == null) return;

		messageText.text = connectedMessage;
		messageText.fontSize = fontSize;
		messageText.color = textColor;
		if (panelImage != null)
		{
			panelImage.color = backgroundColor;
		}

		LayoutRebuilder.ForceRebuildLayoutImmediate(messageText.rectTransform);
		float textW = messageText.preferredWidth;
		float textH = messageText.preferredHeight;
		float width = textW + padding.x * 2f;
		float height = textH + padding.y * 2f;
		panelRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
		panelRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);

		Vector2 anchored = new Vector2(-margin.x, margin.y);
		panelRect.anchoredPosition = anchored;
	}

	private void SetActiveIndicator(bool active)
	{
		if (panelRect != null)
		{
			if (panelRect.gameObject.activeSelf != active)
			{
				panelRect.gameObject.SetActive(active);
			}
		}
	}
}


