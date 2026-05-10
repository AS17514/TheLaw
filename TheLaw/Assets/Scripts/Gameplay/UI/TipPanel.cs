using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public class TipPanel : PanelBase
{
    public static TipPanel Instance;

    [Header("对齐设置")]
    // 基础偏移：无论在哪个角，都会向外偏移这个距离
    public Vector2 baseOffset = new Vector2(20, 20);
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descText;
    private RectTransform _selfRect;
    private RectTransform _canvasRect;
    private Canvas _parentCanvas;

    protected override void Awake()
    {
        base.Awake();
        Instance = this;

        _selfRect = GetComponent<RectTransform>();
        _parentCanvas = GetComponentInParent<Canvas>();
        _canvasRect = _parentCanvas.GetComponent<RectTransform>();

        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }

    void Update()
    {
        if (canvasGroup.alpha > 0.01f)
        {
            FollowMouseSmartAlignment();
        }
    }

    private void FollowMouseSmartAlignment()
    {
        Vector2 mousePos = Input.mousePosition;

        // 1. 确定象限 (0~1 归一化坐标)
        float normX = mousePos.x / Screen.width;
        float normY = mousePos.y / Screen.height;

        // 2. 【核心修复】动态设置 Pivot（轴心点）
        // 鼠标在左半屏，轴心设为 0 (左边缘)；在右半屏，轴心设为 1 (右边缘)
        float pX = normX < 0.5f ? 0f : 1f;
        // 鼠标在下半屏，轴心设为 0 (下边缘)；在上半屏，轴心设为 1 (上边缘)
        float pY = normY < 0.5f ? 0f : 1f;

        // 这一步让“对应的角”变成了面板的坐标原点
        _selfRect.pivot = new Vector2(pX, pY);

        // 3. 计算偏移方向
        // 如果轴心在左(0)，向右偏移(+)；如果轴心在右(1)，向左偏移(-)
        float offsetX = (pX == 0) ? baseOffset.x : -baseOffset.x;
        float offsetY = (pY == 0) ? baseOffset.y : -baseOffset.y;

        // 4. 坐标转换并赋值
        Camera uiCamera = (_parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay) ? null : _parentCanvas.worldCamera;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRect, mousePos, uiCamera, out Vector2 localPoint))
        {
            // 此时 localPoint 正好就是鼠标位置，因为 Pivot 已经改了，
            // 赋值后，面板的“对应角”会瞬间重合在鼠标上，再加 offset 即可完成偏移。
            _selfRect.anchoredPosition = localPoint + new Vector2(offsetX, offsetY);
        }
    }

    public void ShowTooltip(string title, string desc)
    {
        if (titleText != null) titleText.text = title;
        if (descText != null) descText.text = desc;

        // 强制 UI 重新计算宽高，防止 Pivot 切换时因宽高未更新导致的跳动
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(_selfRect);

        // 立即执行一次位置修正
        FollowMouseSmartAlignment();

        canvasGroup.DOKill();
        canvasGroup.DOFade(1, 0.2f);
    }

    public void HideTooltip()
    {
        canvasGroup.DOKill();
        canvasGroup.DOFade(0, 0.2f);
    }
}